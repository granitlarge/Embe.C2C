using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public abstract class AuthorizationFactGenerator(IAuthenticatedUserService authenticatedUserService)
{
    public Guid? CurrentUserId { get; } = authenticatedUserService.UserId;
}