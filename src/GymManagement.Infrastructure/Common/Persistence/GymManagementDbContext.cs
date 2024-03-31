using System.Collections.Generic;
using System.Reflection;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Common.Persistence;

public class GymManagementDbContext : DbContext, IUnitOfWork
{
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Gym> Gyms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;

    public GymManagementDbContext(
        DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
    }

    public async Task CommitChangesAsync()
    {
        // get hold of all domain events
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(l => l)
            .ToList();

        // store them in the http context for later if user is waiting online
        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(_publisher, domainEvents);
        }

        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }


    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;


    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // fetch queue from http context or create new queue if it doesn't exist
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
            .TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new Queue<IDomainEvent>();

        // add the domain events to the end of the queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // store the queue in the http context
        _httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;
    }


    private static async Task PublishDomainEvents(IPublisher _publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}