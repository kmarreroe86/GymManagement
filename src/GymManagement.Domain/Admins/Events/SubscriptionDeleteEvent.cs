using GymManagement.Domain.Common;

namespace GymManagement.Domain.Admins.Events
{
    public record SubscriptionDeleteEvent(Guid SubscriptionId) : IDomainEvent;
    
}
