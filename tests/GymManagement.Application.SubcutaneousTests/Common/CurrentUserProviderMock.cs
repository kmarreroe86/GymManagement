using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Common.Models;
using TestCommon.TestConstants;

namespace GymManagement.Application.SubcutaneousTests.Common
{
    internal class CurrentUserProviderMock : ICurrentUserProvider
    {
        public CurrentUser GetCurrentUser()
        {
            return new CurrentUser(Constants.Admin.Id, new List<string> { "gyms:create" }, new List<string> { "Admin" });
        }
    }
}
