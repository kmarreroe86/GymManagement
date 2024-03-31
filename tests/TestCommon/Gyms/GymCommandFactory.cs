using GymManagement.Application.Gyms.Commands.CreateGym;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
