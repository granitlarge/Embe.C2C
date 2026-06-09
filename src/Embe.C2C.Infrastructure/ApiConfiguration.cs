using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure;

public record Settings(IConfiguration Configuration)
{
    public JwtSettings JwtSettings => new(Configuration);
}

public record JwtSettings(IConfiguration Configuration)
{
    public string Audience => Configuration["Jwt:Audience"]!;
    public string Issuer => Configuration["Jwt:Issuer"]!;
    public string AccessTokenSecret => Configuration["Jwt:Secrets:AccessToken"]!;
    public string RefreshTokenSecret => Configuration["Jwt:Secrets:RefreshToken"]!;
}