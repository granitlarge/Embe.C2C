using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Auth.Handlers;

public class AccountExistsHandler
{
    private readonly IAuthService _authService;

    public AccountExistsHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ErrorOr<bool>> HandleAsync(AccountExistsQuery query, CancellationToken cancellationToken = default)
    {
        var exists = await _authService.AccountExistsAsync(query.Email, cancellationToken);
        return exists;
    }
}