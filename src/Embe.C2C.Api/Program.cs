using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Embe.C2C.Api.EndPoints;
using Embe.C2C.Api.OpenApi;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Infrastructure;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Embe.C2C.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var settings = new Settings(builder.Configuration);

builder.Services.AddOpenApiConfiguration();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetValue<string>("Cors:AllowedOrigins")?.Split(",") ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
app.MapOpenApiEndpoints();
app.MapSignalREndPoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<C2CContext>();
    await context.Database.MigrateAsync();

    #region setup cors on azurite
    var blobServiceClient = new BlobServiceClient(app.Configuration.GetConnectionString("AzureStorageBlobs"));
    var props = await blobServiceClient.GetPropertiesAsync();

    props.Value.Cors.Clear();
    props.Value.Cors.Add(new BlobCorsRule
    {
        AllowedOrigins = "http://frontend-embe.c2c.aspire.dev.localhost:51649",
        AllowedMethods = "GET,PUT,HEAD,OPTIONS,DELETE",
        AllowedHeaders = "*",
        ExposedHeaders = "*",
        MaxAgeInSeconds = 3600
    });

    await blobServiceClient.SetPropertiesAsync(props.Value);
    #endregion
}

app.Run();