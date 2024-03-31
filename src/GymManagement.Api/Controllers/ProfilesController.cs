using MediatR;

using Microsoft.AspNetCore.Mvc;

using GymManagement.Application.Profiles.ListProfiles;
using GymManagement.Contracts.Profiles;
using GymManagement.Application.Profiles.Commands.CreateAdminProfile;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.Api.Controllers;

[Route("users/{userId:guid}/profiles")]
public class ProfilesController : ApiController
{

    private readonly ISender _mediator;

    public ProfilesController(ISender mediator)
    {
        _mediator = mediator;
    }



    [HttpPost("admin")]
    [Authorize]
    public async Task<IActionResult> CreateAdminProfile(Guid userId)
    {
        var command = new CreateAdminProfileCommand(userId);

        var createProfileResult = await _mediator.Send(command);

        return createProfileResult.Match(
            id => Ok(new ProfileResponse(id)),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> ListProfiles(Guid userId)
    {
        var listProfilesQuery = new ListProfilesQuery(userId);

        var listProfilesResult = await _mediator.Send(listProfilesQuery);

        return listProfilesResult.Match(
            profiles => Ok(new ListProfilesResponse(
                profiles.AdminId,
                profiles.ParticipantId,
                profiles.TrainerId)),
            Problem);
    }
}