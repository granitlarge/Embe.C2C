using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SignInHandler(IAuthService authService)
{
    private readonly IAuthService _authService = authService;

    public async Task<TypedResult<SignInFailureReason, Credentials>> HandleAsync
    (
        SignInCommand command, 
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.SignInAsync(command.Email, command.Password, cancellationToken);
        return result;
    }
}