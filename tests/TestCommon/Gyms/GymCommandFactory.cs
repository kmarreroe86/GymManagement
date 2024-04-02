using GymManagement.Application.Gyms.Commands.CreateGym;
using TestCommon.TestConstants;

namespace TestCommon.Gyms
{
    public static class GymCommandFactory
    {

        public static CreateGymCommand CreateCreateGymCommand(
            string name = Constants.Gym.Name,
            Guid? subsriptionId = null)
        {
            return new CreateGymCommand(name, subsriptionId ?? Constants.Subscriptions.Id);
        }
    }
}
