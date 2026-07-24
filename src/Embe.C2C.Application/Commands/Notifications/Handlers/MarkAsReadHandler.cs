using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;

namespace Embe.C2C.Application.Commands.Notifications.Handlers;

public class MarkAsReadHandler : CommandHandler<MarkAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkAsReadHandler
    (
        INotificationRepository notificationRepository,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    )
        : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _notificationRepository = notificationRepository;
    }

    protected async override Task<CommandResult<Result>> HandleAsync(ISparseRepository context, MarkAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(command.NotificationId, cancellationToken);
        if (notification is null)
        {
            return new CommandResult<Result>(Commit: false, Result.Failure(FailureReason.NotFound, "Notification not found."));
        }

        notification.MarkAsRead(command.IsRead);

        return new CommandResult<Result>(Commit: true, Result.Success());
    }
}