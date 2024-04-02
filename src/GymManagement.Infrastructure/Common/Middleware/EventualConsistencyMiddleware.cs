﻿using GymManagement.Domain.Common;
using GymManagement.Infrastructure.Common.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GymManagement.Infrastructure.Common.Middleware
{
    public class EventualConsistencyMiddleware
    {

        private readonly RequestDelegate _next;

        public EventualConsistencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPublisher publisher, GymManagementDbContext dbContext)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();

            context.Response.OnCompleted(async () =>
            {
                try
                {
                    if (context.Items.TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> domainEventsQueue)
                    {
                        while (domainEventsQueue!.TryDequeue(out var domainEvent))
                        {
                            await publisher.Publish(domainEvent);
                        }
                    }

                    await transaction.CommitAsync();
                } catch(Exception)
                {
                    // notify client that even they got good response changes didn't take place due to unexpected error
                }
                finally
                {
                    await transaction.DisposeAsync();
                }                             

            });

            await _next(context);
        }
    }
}
