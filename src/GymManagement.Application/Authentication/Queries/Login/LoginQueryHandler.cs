using ErrorOr;

using MediatR;
using GymManagement.Application.Authentication.Common;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Common.Interfaces;

namespace GymManagement.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;


    public LoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(query.Email);

        return user is null || !user.IsCorrectPasswordHash(query.Password, _passwordHasher)
            ? AuthenticationErrors.InvalidCredentials
            : new AuthenticationResult(user, _jwtTokenGenerator.GenerateToken(user));
    }
}