using Embe.C2C.Application.Abstractions.Events;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users.Events;

namespace Embe.C2C.Application.EventHandlers;

public class DomainEventHandler : ApplicationEventCollector
{
    public async Task HandleAsync(C2CContext context, DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        switch (domainEvent)
        {
            case UserCreatedEvent userCreatedEvent:
                await HandleUserCreatedEventAsync(context, userCreatedEvent, cancellationToken);
                break;
            case MatchingCreatedEvent matchingCreatedEvent:
                await HandleMatchingCreatedEventAsync(context, matchingCreatedEvent, cancellationToken);
                break;
            case MatchingRemovedEvent matchingRemovedEvent:
                await HandleMatchingRemovedEventAsync(context, matchingRemovedEvent, cancellationToken);
                break;
            default:
                break;
        }
    }

    private async Task HandleUserCreatedEventAsync
    (
        C2CContext context,
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        return;
    }

    private Task HandleMatchingCreatedEventAsync
    (
        C2CContext context,
        MatchingCreatedEvent matchingCreatedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingCreatedEvent.Matching;
        var userIdToNotify = matchingCreatedEvent.LastJudgeUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;

        var notification = new MatchingCreated(userIdToNotify, matching.Id);
        context.Add(notification);
        AddApplicationEvent(new NotificationCreatedEvent(notification));
        return Task.CompletedTask;
    }

    private Task HandleMatchingRemovedEventAsync
    (
        C2CContext context,
        MatchingRemovedEvent matchingRemovedEvent,
        CancellationToken cancellationToken
    )
    {
        var matching = matchingRemovedEvent.Matching;
        var userIdToNotify = matchingRemovedEvent.RemoverUserId == matching.UserId1 ? matching.UserId2 : matching.UserId1;

        var notification = new MatchingRemoved(userIdToNotify, matching.Id);
        context.Add(notification);
        AddApplicationEvent(new NotificationCreatedEvent(notification));
        return Task.CompletedTask;
    }
}