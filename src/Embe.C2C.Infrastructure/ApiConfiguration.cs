using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure;

public record Settings(IConfiguration Configuration)
{
    public JwtSettings JwtSettings => new(Configuration);
    public AzureAIContentSafetySettings AzureAIContentSafetySettings => new(Configuration);
}

public record JwtSettings(IConfiguration Configuration)
{
    public string Audience => Configuration["Jwt:Audience"]!;
    public string Issuer => Configuration["Jwt:Issuer"]!;
    public string AccessTokenSecret => Configuration["Jwt:Secrets:AccessToken"]!;
    public string RefreshTokenSecret => Configuration["Jwt:Secrets:RefreshToken"]!;
}

public record AzureAIContentSafetySettings(IConfiguration Configuration)
{
    public string Url => Configuration["AzureAIContentSafety:Url"] ?? throw new InvalidOperationException("missing configuration key value 'AzureAIContentSafety:Url'");
    public string ApiKey => Configuration["AzureAIContentSafety:ApiKey"] ?? throw new InvalidOperationException("missing configuration key value 'AzureAIContentSafety:ApiKey'");
}