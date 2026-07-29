using Embe.C2C.Application.Abstractions.Settings;
using Embe.C2C.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure;

public record Settings(IConfiguration Configuration) : ISettings
{
    public JwtSettings Jwt => new(Configuration);
    public AzureAIContentSafetySettings AzureAIContentSafety => new(Configuration);
    public SiteSettings Site => GetSiteSettings(Configuration);
    public CorsSettings Cors => new(Configuration);
    public EmailSettings Email => new(Configuration);

    public ApplicationSettings Application => GetApplicationSettings(Configuration);

    private static ApplicationSettings GetApplicationSettings(IConfiguration configuration)
    {
        var nameKey = "Application:Name";
        var name = configuration[nameKey] ?? throw new MissingConfigurationKeyException(nameKey);

        return new ApplicationSettings(name);
    }

    private static SiteSettings GetSiteSettings(IConfiguration configuration)
    {
        var urlKey = "Site:Url";
        return new SiteSettings
        (
            configuration[urlKey] ?? throw new MissingConfigurationKeyException(urlKey)
        );
    }

}

public record JwtSettings(IConfiguration Configuration)
{
    private static readonly string AudienceKey = "Jwt:Audience";
    private static readonly string IssuerKey = "Jwt:Issuer";
    private static readonly string AccessTokenKey = "Jwt:Secrets:AccessToken";
    private static readonly string RefreshTokenKey = "Jwt:Secrets:RefreshToken";
    private static readonly string ResetPasswordTokenKey = "Jwt:Secrets:ResetPasswordToken";

    public string Audience => Configuration[AudienceKey] ?? throw new MissingConfigurationKeyException(AudienceKey);
    public string Issuer => Configuration[IssuerKey] ?? throw new MissingConfigurationKeyException(IssuerKey);
    public string AccessTokenSecret => Configuration[AccessTokenKey] ?? throw new MissingConfigurationKeyException(AccessTokenKey);
    public string RefreshTokenSecret => Configuration[RefreshTokenKey] ?? throw new MissingConfigurationKeyException(RefreshTokenKey);
    public string ResetPasswordTokenSecret => Configuration[ResetPasswordTokenKey] ?? throw new MissingConfigurationKeyException(ResetPasswordTokenKey);
}

public record AzureAIContentSafetySettings(IConfiguration Configuration)
{
    private static readonly string UrlKey = "AzureAIContentSafety:Url";
    private static readonly string ApiKeyKey = "AzureAIContentSafety:ApiKey";
    public string Url => Configuration[UrlKey] ?? throw new MissingConfigurationKeyException(UrlKey);
    public string ApiKey => Configuration[ApiKeyKey] ?? throw new MissingConfigurationKeyException(ApiKeyKey);
}


public record CorsSettings(IConfiguration Configuration)
{
    private static readonly string AllowedOriginsKey = "Cors:AllowedOrigins";

    public IEnumerable<string> AllowedOrigins => Configuration[AllowedOriginsKey]?.Split(",") ?? throw new MissingConfigurationKeyException(AllowedOriginsKey);
}

public record EmailSettings(IConfiguration Configuration)
{
    private static readonly string SenderKey = "Email:Sender";
    public string Sender => Configuration[SenderKey] ?? throw new MissingConfigurationKeyException(SenderKey);
}