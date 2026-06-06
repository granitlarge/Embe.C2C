using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SignOutHandler(IAuthService authService)
{
    private readonly IAuthService _authService = authService;

    public async Task<TypedResult<SignOutFailureReason, bool>> HandleAsync
    (
        SignOutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        return await _authService.SignOutAsync(command.RefreshToken, cancellationToken);
    }
}