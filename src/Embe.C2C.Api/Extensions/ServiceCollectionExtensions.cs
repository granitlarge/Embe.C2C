using Embe.C2C.Api.OpenApi;
using Embe.C2C.Infrastructure.Extensions;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Infrastructure;
namespace Embe.C2C.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices
    (
        this IServiceCollection services,
        Settings settings,
        IHostEnvironment environment
    )
    {

        services.AddOpenApiConfiguration();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins([.. settings.Cors.AllowedOrigins])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddHttpContextAccessor();

        services.AddAuthorization();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.RespectNullableAnnotations = true;
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddInfrastructure(settings.Configuration, environment);
        services.AddApplication();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = settings.Jwt.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Jwt.Issuer,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                    (
                        System.Text.Encoding.UTF8.GetBytes(settings.Jwt.AccessTokenSecret)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true
                };

                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer("Refresh", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = settings.Jwt.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Jwt.Issuer,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                    (
                        System.Text.Encoding.UTF8.GetBytes(settings.Jwt.RefreshTokenSecret)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true
                };
            })
            .AddJwtBearer("ResetPassword", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = settings.Jwt.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Jwt.Issuer,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                    (
                        System.Text.Encoding.UTF8.GetBytes(settings.Jwt.ResetPasswordTokenSecret)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true
                };
            });

        return services;
    }
}