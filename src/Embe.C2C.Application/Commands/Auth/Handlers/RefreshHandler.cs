using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class RefreshHandler(IAuthService authService)
{
    private readonly IAuthService _authService = authService;

    public async Task<TypedResult<RefreshFailureReason, Credentials>> HandleAsync
    (
        RefreshCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.RefreshAsync(command.RefreshToken, cancellationToken);
        return result;
    }
}