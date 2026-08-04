namespace Embe.C2C.Application.Commands.Users;

public record ChangeEmailCommand(string NewEmail, string VerificationCode);