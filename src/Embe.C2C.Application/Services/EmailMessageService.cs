using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Settings;
using Embe.C2C.Application.Events.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Candidates;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications.Messages;

namespace Embe.C2C.Application.Services;

public class EmailComposerService(ISettings settings, INotificationRepository notificationRepository)
{
    private readonly ISettings _settings = settings;
    private readonly INotificationRepository _notificationRepository = notificationRepository;

    public async Task<(string Subject, string HtmlContent, string PlainText)> CreateMessageAsync
    (
        NotificationCreatedIntegrationEvent created,
        CancellationToken cancellationToken
    )
    {
        var notification = await _notificationRepository.GetByIdAsync(created.NotificationId, cancellationToken);
        return notification switch
        {
            PositivelyJudgedNotification positivelyJudged => CreatePositivelyJudgedMessage(positivelyJudged),
            MessageCreatedNotification messageCreated => CreateMessageCreatedMessage(messageCreated),
            MatchingCreatedNotification matchingCreated => CreateMatchingCreatedMessage(matchingCreated),
            _ => throw new NotImplementedException()
        };
    }

    private (string Subject, string HtmlContent, string PlainText) CreateMatchingCreatedMessage(MatchingCreatedNotification matchingCreated)
    {
        var subject = $"{_settings.Application.Name} | you've got a new match!";
        var link = $"{_settings.Site.Url}/protected/matches";
        var htmlContent = Emails.MatchingCreatedMessage(_settings.Application.Name, link);
        var plainText =
        $$"""
            You've got a new match! Click the link below to check it out.

            {{link}}
        """;

        return (subject, htmlContent, plainText);
    }

    private (string Subject, string HtmlContent, string PlainText) CreateMessageCreatedMessage
    (
        MessageCreatedNotification messageCreated
    )
    {
        var subject = $"{_settings.Application.Name} | you've got a new message!";
        var link = $"{_settings.Site.Url}/protected/matches";
        var htmlContent = Emails.MatchingCreatedMessage(_settings.Application.Name, link);
        var plainText =
        $$"""
            You've got a new message! Click the link below to check it out.

            {{link}}
        """;

        return (subject, htmlContent, plainText);

    }

    private (string Subject, string HtmlContent, string PlainText) CreatePositivelyJudgedMessage(PositivelyJudgedNotification positivelyJudged)
    {
        var subject = $"{_settings.Application.Name} | you've got a new like!";
        var link = $"{_settings.Site.Url}/protected/likes";
        var htmlContent = Emails.MatchingCreatedMessage(_settings.Application.Name, link);
        var plainText =
        $$"""
            You've got a new like! Click the link below to check it out.

            {{link}}
        """;

        return (subject, htmlContent, plainText);
    }
}