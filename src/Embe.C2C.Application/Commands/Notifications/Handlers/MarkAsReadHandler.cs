using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Commands.Notifications.Handlers;

public class MarkAsReadHandler : CommandHandler<MarkAsReadCommand, Result>
{
    public MarkAsReadHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    )
        : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {

    }

    protected async override Task<CommandResult<Result>> HandleAsync(ISparseRepository context, MarkAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await context.NotificationsQuery.FirstOrDefaultAsync(n => n.Id == command.NotificationId, cancellationToken);
        if (notification is null)
        {
            return new CommandResult<Result>(Commit: false, Result.Failure(FailureReason.NotFound, "Notification not found."));
        }

        notification.MarkAsRead(command.IsRead);

        return new CommandResult<Result>(Commit: true, Result.Success());
    }
}