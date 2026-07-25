using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class MarkMessagesAsSeenHandler
(
    IMessageRepository messageRepo,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    MessageAuthorizationService messageAuthoriztionPolicy,
    DomainEventStore domainEventStore
) : CommandHandler<MarkMessagesAsSeenCommand, ErrorOr<Success>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{
    private readonly MessageAuthorizationService _messageAuthorizationPolicy = messageAuthoriztionPolicy;
    private readonly IMessageRepository _messageRepo = messageRepo;

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        MarkMessagesAsSeenCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var messages = await _messageRepo.GetMessagesByMessageIdsAsync([.. command.MessageIds], cancellationToken);
        if (messages.Count != command.MessageIds.Length)
        {
            return new CommandResult<ErrorOr<Success>>(false, Error.NotFound("message_not_found", "One or more messages not found."));
        }

        foreach (var message in messages)
        {
            var permissions = await _messageAuthorizationPolicy.GetPermissionsAsync(message.Id, cancellationToken);
            if (!permissions.Contains(MessagePermission.MarkAsSeen))
            {
                return new CommandResult<ErrorOr<Success>>(false, Error.Forbidden("forbidden", "Authenticated user does not have permission to mark this message as seen."));
            }

            message.MarkAsSeen(seen: true);
        }
        return new CommandResult<ErrorOr<Success>>(Save: true, Result.Success);
    }
}