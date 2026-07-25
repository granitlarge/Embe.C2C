namespace Embe.C2C.Application.Errors;

public static class ApplicationErrors
{
    public static readonly ApplicationError NotFound = new("not_found", "The requested resource was not found.");
    public static readonly ApplicationError Forbidden = new("forbidden", "The authenticated user does not have access to the requested resource.");
    public static readonly ApplicationError Unexpected = new("unexpected", "An unexpected error occurred.");
    public static readonly ApplicationError Unauthorized = new("unauthorized", "The requestor is not authenticated.");

    public static readonly ApplicationError PasswordMismatch = new("auth.password_mismatch", "The supplied password is invalid.");
    public static readonly ApplicationError InvalidToken = new("auth.invalid_token", "Invalid or expired token.");
    public static readonly ApplicationError RecoveryCodeRedemptionFailed = new("auth.recovery_code_redemption_failed", "Invalid recovery code specified.");
    public static readonly ApplicationError LoginAlreadyAssociated = new("auth.login_already_associated", "An external logoin is already associated with the account");
    public static readonly ApplicationError InvalidUserName = new("auth.invalid_username", "Invalid username");
    public static readonly ApplicationError InvalidEmail = new("auth.invalid_email", "Invalid e-mail");
    public static readonly ApplicationError DuplicateUserName = new("auth.duplicate_username", "An account with the specified username already exists.");
    public static readonly ApplicationError DuplicateEmail = new("auth.duplicate_email", "An account with the specified e-mail already exists.");
    public static readonly ApplicationError InvalidRoleName = new("auth.invalid_role_name", "The specified role name is invalid.");
    public static readonly ApplicationError DuplicateRoleName = new("auth.duplicate_role_name", "The specified role name already exists.");
    public static readonly ApplicationError UserAlreadyHasPassword = new("auth.user_already_has_password", "The user already has a password.");
    public static readonly ApplicationError UserLockoutNotEnabled = new("auth.user_lockout_not_enabled", "User lockout is not enabled.");
    public static readonly ApplicationError UserAlreadyInRole = new("auth.user_already_in_role", "The user is already part of the specified role.");
    public static readonly ApplicationError UserNotInRole = new("auth.user_not_in_role", "The user is not part of the specified role.");
    public static readonly ApplicationError PasswordTooShort = new("auth.password_too_short", "The specified password is too short.");
    public static readonly ApplicationError PasswordRequiresUniqueChars = new("auth.password_requires_unique_chars", "The password does not meet the minimum number of unique characters.");
    public static readonly ApplicationError PasswordRequiresNonAlphanumeric = new("auth.password_requires_non_alphanumeric", "The password requires non-alphanumeric characters, but none were specified.");
    public static readonly ApplicationError PasswordRequiresDigit = new("auth.password_requires_digit", "The password requires digits, but none were specified.");
    public static readonly ApplicationError PasswordRequiresLower = new("auth.password_requires_lower", "The password requires lowercase characters, but none were specified.");
    public static readonly ApplicationError PasswordRequiresUpper = new("auth.password_requires_upper", "The password requires uppercase characters, but none were specified.");

    public static readonly ApplicationError NoUserWithSuppliedEmail = new("auth.no_user_with_supplied_email", "There's no registered account associated with the supplied e-mail.");
    public static readonly ApplicationError InvalidCredentials = new("auth.invalid_credentials", "The supplied credentials are invalid.");
    public static readonly ApplicationError InvalidRefreshToken = new("auth.invalid_refresh_token", "Invalid refresh token");
    public static readonly ApplicationError LockedOut = new("auth.locked_out", "The user has had too many failed login attempts and is now locked out.");
}