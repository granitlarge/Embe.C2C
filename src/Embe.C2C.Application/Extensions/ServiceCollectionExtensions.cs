using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Queries.Auth.Handlers;
using Embe.C2C.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<Commands.Users.Handlers.RegisterHandler>();
        services.AddScoped<Commands.Users.Handlers.DeleteHandler>();
        services.AddScoped<Commands.Users.Handlers.UpdateHandler>();

        services.AddScoped<Commands.Matching.Handlers.UnmatchHandler>();
        services.AddScoped<Commands.Judgements.Handlers.JudgeHandler>();

        services.AddScoped<Commands.Auth.Handlers.SignInHandler>();
        services.AddScoped<Commands.Auth.Handlers.SignOutHandler>();
        services.AddScoped<Commands.Auth.Handlers.RefreshHandler>();

        services.AddScoped<JudgementAuthorizationPolicy>();
        services.AddScoped<MatchingAuthorizationPolicy>();
        services.AddScoped<UserAuthorizationPolicy>();

        services.AddScoped<DomainEventHandler>();
        services.AddScoped<IntegrationEventHandler>();

        services.AddScoped<UserService>();
        services.AddScoped<MatchingService>();
        services.AddScoped<JudgementService>();

        services.AddScoped<AccountExistsHandler>();
        return services;
    }
}