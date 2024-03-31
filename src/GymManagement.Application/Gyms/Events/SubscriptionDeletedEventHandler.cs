using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins.Events;
using MediatR;

namespace GymManagement.Application.Gyms.Events
{
    public class SubscriptionDeletedEventHandler : INotificationHandler<SubscriptionDeleteEvent>
    {

        private readonly IGymsRepository _gymsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionDeletedEventHandler(IGymsRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _gymsRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task Handle(SubscriptionDeleteEvent notification, CancellationToken cancellationToken)
        {
            var gyms = await _gymsRepository.ListBySubscriptionIdAsync(notification.SubscriptionId);

            await _gymsRepository.RemoveRangeAsync(gyms);
            await _unitOfWork.CommitChangesAsync();
        }
    }
}
