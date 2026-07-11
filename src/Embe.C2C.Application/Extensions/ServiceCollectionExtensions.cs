using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Conversations;
using Embe.C2C.Application.Authorizations.FactStores.Judgements;
using Embe.C2C.Application.Authorizations.FactStores.Matches;
using Embe.C2C.Application.Authorizations.FactStores.Messages;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
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

        services.AddScoped<Commands.SearchProfiles.Handlers.CreateSearchProfileHandler>();
        services.AddScoped<Commands.SearchProfiles.Handlers.UpdateSearchProfileHandler>();
        services.AddScoped<Commands.SearchProfiles.Handlers.DeleteSearchProfileHandler>();

        services.AddScoped<Queries.Auth.Handlers.AccountExistsHandler>();

        services.AddScoped<Queries.Notifications.Handlers.GetNotificationsHandler>();
        services.AddScoped<Queries.Notifications.Handlers.HasUnreadHandler>();

        services.AddScoped<Queries.Matchings.Handlers.GetMatchingsHandler>();
        services.AddScoped<Queries.Matchings.Handlers.GetMatchingByIdHandler>();

        services.AddScoped<Queries.Messages.Handlers.GetMessagesByMatchingIdHandler>();
        services.AddScoped<Queries.Messages.Handlers.GetMessageByIdHandler>();

        services.AddScoped<Queries.Judgements.Handlers.GetPositiveJudgementsHandler>();

        services.AddScoped<Queries.Users.Handlers.GetUserByIdHandler>();
        services.AddScoped<Queries.Users.Handlers.GetMeHandler>();
        services.AddScoped<Queries.Users.Handlers.GetHasSearchProfileHandler>();

        services.AddScoped<Queries.Geography.Handlers.SearchAdminAreaHandler>();
        services.AddScoped<Queries.Geography.Handlers.GetAdminAreaByIdHandler>();
        services.AddScoped<Queries.Geography.Handlers.GetCountryAdminAreaHandler>();
        services.AddScoped<Queries.Geography.Handlers.ReverseGeocodeHandler>();

        services.AddScoped<Queries.SearchProfiles.Handlers.GetSearchProfileHandler>();
        services.AddScoped<Queries.SearchProfiles.Handlers.GetAllSearchProfilesHandler>();

        services.AddScoped<ConversationAuthorizationFactStore>();
        services.AddScoped<UserAuthorizationFactStore>();
        services.AddScoped<MatchingAuthorizationFactStore>();
        services.AddScoped<MessageAuthorizationFactStore>();
        services.AddScoped<JudgementAuthorizationFactStore>();
        services.AddScoped<SearchProfileAuthorizationFactStore>();

        services.AddScoped<UserFactGenerator>();
        services.AddScoped<MatchingFactGenerator>();
        services.AddScoped<MessageFactGenerator>();
        services.AddScoped<SearchProfileFactGenerator>();
        services.AddScoped<JudgementAuthorizationFactGenerator>();

        services.AddScoped<MatchingAuthorizationService>();
        services.AddScoped<UserAuthorizationService>();
        services.AddScoped<MessageAuthorizationService>();
        services.AddScoped<JudgementAuthorizationService>();
        services.AddScoped<SearchProfileAuthorizationService>();

        services.AddScoped<DomainEventHandler>();
        services.AddScoped<IntegrationEventHandler>();

        services.AddScoped<DomainEventStore>();
        services.AddScoped<UserService>();
        services.AddScoped<MatchingService>();
        services.AddScoped<JudgementService>();

        services.AddScoped<UserDtoMapper>();
        services.AddScoped<ImageDtoMapper>();
        services.AddScoped<JudgementDtoMapper>();
        services.AddScoped<MessageDtoMapper>();
        services.AddScoped<ConversationDtoMapper>();
        services.AddScoped<MatchingDtoMapper>();
        services.AddScoped<SearchProfileDtoMapper>();

        services.AddScoped<IFileUrlGenerator, FileUrlGenerator>((services) =>
        {
            var service = services.GetRequiredService<IFileService>();
            var sasDuration = TimeSpan.FromMinutes(30);
            return new FileUrlGenerator(service, sasDuration);
        });

        return services;
    }
}