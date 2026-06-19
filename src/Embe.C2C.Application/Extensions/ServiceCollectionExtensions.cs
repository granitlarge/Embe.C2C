using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Conversations;
using Embe.C2C.Application.Authorizations.FactStores.Judgements;
using Embe.C2C.Application.Authorizations.FactStores.Matches;
using Embe.C2C.Application.Authorizations.FactStores.Messages;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.EventHandlers;
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
        services.AddScoped<Commands.Users.Handlers.GenerateCandidatesHandler>();

        services.AddScoped<Commands.Matching.Handlers.UnmatchHandler>();
        services.AddScoped<Commands.Judgements.Handlers.JudgeHandler>();

        services.AddScoped<Commands.Auth.Handlers.SignInHandler>();
        services.AddScoped<Commands.Auth.Handlers.SignOutHandler>();
        services.AddScoped<Commands.Auth.Handlers.RefreshHandler>();

        services.AddScoped<Commands.Messages.Handlers.CreateMessageHandler>();
        services.AddScoped<Commands.Messages.Handlers.DeleteMessageHandler>();
        services.AddScoped<Commands.Messages.Handlers.EditMessageHandler>();
        services.AddScoped<Commands.Messages.Handlers.MarkMessagesAsSeenHandler>();

        services.AddScoped<Commands.Notifications.Handlers.MarkAsReadHandler>();

        services.AddScoped<Queries.Auth.Handlers.AccountExistsHandler>();

        services.AddScoped<Queries.Notifications.Handlers.GetNotificationsHandler>();
        services.AddScoped<Queries.Notifications.Handlers.HasUnreadHandler>();

        services.AddScoped<Queries.Matchings.Handlers.GetMatchingsHandler>();
        services.AddScoped<Queries.Matchings.Handlers.GetMatchingByIdHandler>();

        services.AddScoped<Queries.Messages.Handlers.GetMessagesByMatchingIdHandler>();

        services.AddScoped<Queries.Judgements.Handlers.GetPositiveJudgementsHandler>();

        services.AddScoped<ConversationAuthorizationFactStore>();
        services.AddScoped<UserAuthorizationFactStore>();
        services.AddScoped<MatchingAuthorizationFactStore>();
        services.AddScoped<MessageAuthorizationFactStore>();
        services.AddScoped<JudgementAuthorizationFactStore>();

        services.AddScoped<UserFactGenerator>();
        services.AddScoped<MatchingFactGenerator>();
        services.AddScoped<MessageFactGenerator>();

        services.AddScoped<MatchingAuthorizationPolicy>();
        services.AddScoped<UserAuthorizationPolicy>();
        services.AddScoped<MessageAuthorizationPolicy>();
        services.AddScoped<JudgementAuthorizationPolicy>();

        services.AddScoped<DomainEventHandler>();
        services.AddScoped<IntegrationEventHandler>();

        services.AddScoped<UserService>();
        services.AddScoped<MatchingService>();
        services.AddScoped<JudgementService>();

        return services;
    }
}