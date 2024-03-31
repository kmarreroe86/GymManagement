using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Subscriptions;
using TestCommon.Gyms;
using TestCommon.Subscriptions;

namespace GymManagement.Domain.UnitTests.Subscriptions
{
    public class SubscriptionTests
    {

        [Fact]
        void AddGym_WhenMoreThanSubscriptionAllow_ShouldFail()
        {
            // Arrange
            var subscription = SubscriptionFactory.CreateSubscription();

            var gyms = Enumerable.Range(0, subscription.GetMaxGyms() + 1)
                .Select(_ => GymsFactory.CreateGym(id: Guid.NewGuid()))
                .ToList();


            // Act
            var addGymResults = gyms.ConvertAll(subscription.AddGym);


            // Assert
            var allButLastGymResults = addGymResults.SkipLast(1).ToList(); // Get all results but the last one

            allButLastGymResults.Should().AllSatisfy(addGymResult => addGymResult.Value.Should().Be(Result.Success)); // All gyms but last one should be added successfully

            var lastAddGymResult = addGymResults.Last();
            lastAddGymResult.IsError.Should().BeTrue();
            lastAddGymResult.FirstError.Should().Be(SubscriptionErrors.CannotHaveMoreGymsThanTheSubscriptionAllows);

        }
    }
}
