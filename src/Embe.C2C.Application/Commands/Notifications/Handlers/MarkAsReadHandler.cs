using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Notifications.Handlers;

public class MarkAsReadHandler : TransactionalCommandHandler<MarkAsReadCommand, Result>
{
    public MarkAsReadHandler
    (
        IC2CContext context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler
    )
        : base(context, domainEventHandler, integrationEventHandler)
    {

    }

    protected async override Task<TransactionalCommandResult<Result>> HandleAsync(ISparseC2CContext context, MarkAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == command.NotificationId, cancellationToken);
        if (notification is null)
        {
            return new TransactionalCommandResult<Result>(CommitChanges: false, Result.Failure(FailureReason.NotFound, "Notification not found."));
        }

        notification.MarkAsRead(command.IsRead);

        return new TransactionalCommandResult<Result>(CommitChanges: true, Result.Success());
    }
}