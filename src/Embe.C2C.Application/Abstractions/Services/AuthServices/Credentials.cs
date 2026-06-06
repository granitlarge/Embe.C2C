namespace Embe.C2C.Application.Abstractions.Services.AuthServices;

public record Credentials(AccessToken AccessToken, RefreshToken RefreshToken);
public record RefreshToken(Guid Id, string Token, DateTimeOffset ExpiresAt);
public record AccessToken(string Token, DateTimeOffset ExpiresAt);