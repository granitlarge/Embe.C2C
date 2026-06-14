using Embe.C2C.Api.EndPoints;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Infrastructure;
using Embe.C2C.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var settings = new Settings(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.RespectNullableAnnotations = true;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapMatchingEndPoints();
app.MapJudgementEndPoints();
app.MapNotificationEndPoints();

app.Run();