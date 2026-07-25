using Embe.C2C.Application.Errors;
using Microsoft.AspNetCore.Identity;

namespace Embe.C2C.Infrastructure.Extensions;

public static class IdentityErrorExtensions
{
    public static ApplicationError ToApplicationError(this IdentityError error)
    {
        return error.Code switch
        {
            "DefaultError" => ApplicationErrors.Unexpected,
            "ConcurrencyFailure" => ApplicationErrors.Unexpected,
            nameof(ApplicationErrors.PasswordMismatch) => ApplicationErrors.PasswordMismatch,
            nameof(ApplicationErrors.InvalidToken) => ApplicationErrors.InvalidToken,
            nameof(ApplicationErrors.RecoveryCodeRedemptionFailed) => ApplicationErrors.RecoveryCodeRedemptionFailed,
            nameof(ApplicationErrors.LoginAlreadyAssociated) => ApplicationErrors.LoginAlreadyAssociated,
            nameof(ApplicationErrors.InvalidUserName) => ApplicationErrors.InvalidUserName,
            nameof(ApplicationErrors.InvalidEmail) => ApplicationErrors.InvalidEmail,
            nameof(ApplicationErrors.DuplicateUserName) => ApplicationErrors.DuplicateUserName,
            nameof(ApplicationErrors.DuplicateEmail) => ApplicationErrors.DuplicateEmail,
            nameof(ApplicationErrors.InvalidRoleName) => ApplicationErrors.InvalidRoleName,
            nameof(ApplicationErrors.DuplicateRoleName) => ApplicationErrors.DuplicateRoleName,
            nameof(ApplicationErrors.UserAlreadyHasPassword) => ApplicationErrors.UserAlreadyHasPassword,
            nameof(ApplicationErrors.UserLockoutNotEnabled) => ApplicationErrors.UserLockoutNotEnabled,
            nameof(ApplicationErrors.UserAlreadyInRole) => ApplicationErrors.UserAlreadyInRole,
            nameof(ApplicationErrors.UserNotInRole) => ApplicationErrors.UserNotInRole,
            nameof(ApplicationErrors.PasswordTooShort) => ApplicationErrors.PasswordTooShort,
            nameof(ApplicationErrors.PasswordRequiresUniqueChars) => ApplicationErrors.PasswordRequiresUniqueChars,
            nameof(ApplicationErrors.PasswordRequiresDigit) => ApplicationErrors.PasswordRequiresDigit,
            nameof(ApplicationErrors.PasswordRequiresLower) => ApplicationErrors.PasswordRequiresLower,
            nameof(ApplicationErrors.PasswordRequiresUpper) => ApplicationErrors.PasswordRequiresUpper,
            _ => ApplicationErrors.Unexpected
        };
    }

    public static IEnumerable<ApplicationError> ToApplicationErrors(this IEnumerable<IdentityError> error)
    {
        return error.Select(e => e.ToApplicationError());
    }
}