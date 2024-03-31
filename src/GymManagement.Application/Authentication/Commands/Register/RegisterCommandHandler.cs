using ErrorOr;

using MediatR;
using GymManagement.Application.Authentication.Common;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Common.Interfaces;
using GymManagement.Domain.Users;

namespace GymManagement.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
   

    public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _usersRepository.ExistsByEmailAsync(command.Email))
        {
            return Error.Conflict(description: "User already exists");
        }

        var hashPasswordResult = _passwordHasher.HashPassword(command.Password);

        if (hashPasswordResult.IsError)
        {
            return hashPasswordResult.Errors;
        }

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email,
            hashPasswordResult.Value);

        await _usersRepository.AddUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}