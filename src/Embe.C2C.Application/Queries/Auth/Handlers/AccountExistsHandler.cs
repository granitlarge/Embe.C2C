using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;

namespace Embe.C2C.Application.Queries.Auth.Handlers;

public class AccountExistsHandler
{
    private readonly IAuthService _authService;

    public AccountExistsHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<bool>> HandleAsync(AccountExistsQuery query)
    {
        try
        {
            var exists = await _authService.AccountExistsAsync(query.Email);
            return Result<bool>.Success(exists);
        }
        catch (Exception)
        {
            return Result<bool>.Failure(FailureReason.Unknown, "An error occurred while checking if the account exists.");
        }
    }
}