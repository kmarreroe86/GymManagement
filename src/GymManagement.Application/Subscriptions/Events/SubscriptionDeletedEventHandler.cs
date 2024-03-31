using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins.Events;
using MediatR;

namespace GymManagement.Domain.Subscriptions.Events
{
    public class SubscriptionDeletedEventHandler : INotificationHandler<SubscriptionDeleteEvent>
    {

        private readonly ISubscriptionsRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionDeletedEventHandler(ISubscriptionsRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SubscriptionDeleteEvent notification, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(notification.SubscriptionId);

            if (subscription is null)
            {
                // resilient error handling
                throw new InvalidOperationException();
            }

            await _subscriptionRepository.RemoveSubscriptionAsync(subscription);
            await _unitOfWork.CommitChangesAsync();
        }
    }
}
