using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Notifications.Handlers;

public class MarkAsReadHandler : CommandHandler<MarkAsReadCommand, ErrorOr<Success>>
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

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync(MarkAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(command.NotificationId, cancellationToken);
        if (notification is null)
        {
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());
        }

        notification.Remove();
        _notificationRepository.Set.Remove(notification);

        return new(Save: true, Result.Success);
    }
}