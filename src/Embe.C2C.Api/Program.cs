using Embe.C2C.Api.EndPoints;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Infrastructure;
using Embe.C2C.Infrastructure.Extensions;
using Embe.C2C.Infrastructure.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
var settings = new Settings(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.RespectNullableAnnotations = true;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = settings.JwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = settings.JwtSettings.Issuer,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
            (
                System.Text.Encoding.UTF8.GetBytes(settings.JwtSettings.AccessTokenSecret)
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
            ValidAudience = settings.JwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = settings.JwtSettings.Issuer,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
            (
                System.Text.Encoding.UTF8.GetBytes(settings.JwtSettings.RefreshTokenSecret)
            ),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapMatchingEndPoints();
app.MapJudgementEndPoints();
app.MapNotificationEndPoints();
app.MapMessageEndPoints();
app.MapGeographyEndpoints();
app.MapSearchProfileEndPoints();

app.MapHub<MainHub>("/hubs/main", options =>
{
    options.CloseOnAuthenticationExpiration = true;
});

app.Run();