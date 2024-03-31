using System.Reflection;
using ErrorOr;
using GymManagement.Application.Common.Authorization;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
{

    private readonly ICurrentUserProvider _currentUserProvider;

    public AuthorizationBehavior(ICurrentUserProvider currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }


    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizationAttributes.Count == 0)
        {
            return await next();
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var requiredPermissions = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? Array.Empty<string>())
            .ToList();

        if (requiredPermissions.Except(currentUser.Permissions).Any())  // If user is lacking any permissions in "requiredPermissions"
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? Array.Empty<string>())
            .ToList();

        if (requiredRoles.Except(currentUser.Roles).Any()) // If the user is lacking of any of the required roles in "requiredRoles"
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        return await next();
    }
}
