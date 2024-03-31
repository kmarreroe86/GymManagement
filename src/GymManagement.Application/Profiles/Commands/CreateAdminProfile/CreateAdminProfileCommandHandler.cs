using ErrorOr;

using MediatR;

using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;

namespace GymManagement.Application.Profiles.Commands.CreateAdminProfile;

public class CreateAdminProfileCommandHandler : IRequestHandler<CreateAdminProfileCommand, ErrorOr<Guid>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAdminsRepository _adminsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateAdminProfileCommandHandler(IUsersRepository usersRepository, IAdminsRepository adminsRepository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _usersRepository = usersRepository;
        _adminsRepository = adminsRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }


    public async Task<ErrorOr<Guid>> Handle(CreateAdminProfileCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();

        if (currentUser.Id != command.UserId)
        {
            return Error.Unauthorized(description: "User is forbidden from taking this action.");
        }

        var user = await _usersRepository.GetByIdAsync(command.UserId);


        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        var createAdminProfileResult = user.CreateAdminProfile();
        var admin = new Admin(userId: user.Id, id: createAdminProfileResult.Value);

        await _usersRepository.UpdateAsync(user);
        await _adminsRepository.AddAdminAsync(admin);
        await _unitOfWork.CommitChangesAsync();

        return createAdminProfileResult;
    }
}