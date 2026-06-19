using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Messages.Handlers;

public class MarkMessagesAsSeenHandler
(
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    MessageAuthorizationPolicy messageAuthoriztionPolicy
) : TransactionalCommandHandler<MarkMessagesAsSeenCommand, Result>(context, domainEventHandler, integrationEventHandler)
{
    private readonly MessageAuthorizationPolicy _messageAuthorizationPolicy = messageAuthoriztionPolicy;

    protected async override Task<TransactionalCommandResult<Result>> HandleAsync
    (
        ISparseRepository context,
        MarkMessagesAsSeenCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var messages = await context.MessagesQuery.Where(m => command.MessageIds.Contains(m.Id)).ToListAsync(cancellationToken: cancellationToken);
        if (messages.Count != command.MessageIds.Length)
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "One or more messages were not found."));
        }

        foreach (var message in messages)
        {
            var permissions = await _messageAuthorizationPolicy.GetPermissionsAsync(message.Id, cancellationToken);
            if (!permissions.Contains(MessagePermission.MarkAsSeen))
            {
                return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You do not have permission to mark this message as seen."));
            }

            message.MarkAsSeen(seen: true);
        }
        return new TransactionalCommandResult<Result>(true, Result.Success());
    }
}