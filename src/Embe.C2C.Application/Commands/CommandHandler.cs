using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;

namespace Embe.C2C.Application.Commands;

public record CommandResult<T>
(
    bool Save,
    T Value
);

public abstract class CommandHandler<T_Command, T_Result>
(
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler
)
{
    private readonly DomainEventStore _domainEventStore = domainEventStore;
    private readonly IRepository _repo = context;
    private readonly DomainEventHandler _domainEventHandler = domainEventHandler;
    private readonly IntegrationEventHandler _integrationEventHandler = integrationEventHandler;

    public async Task<T_Result> HandleAsync
    (
        T_Command command,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _repo.BeginTransactionAsync(cancellationToken);
        var result = await InternalHandleAsync(command, cancellationToken);

        foreach (var domainEvent in _repo.DomainEvents.Union(_domainEventStore.DomainEvents).OrderBy(de => de.Timestamp))
        {
            await _domainEventHandler.HandleAsync(domainEvent, cancellationToken);
        }

        if (result.Save)
        {
            await _repo.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
#warning What should we do if this fails? We have already committed the transaction, but failed to publish the events. This could lead to an inconsistent state.
            await _integrationEventHandler.HandleAsync(_domainEventHandler, cancellationToken);
        }
        else
        {
            await transaction.RollbackAsync(cancellationToken);
        }

        return result.Value;
    }

    protected abstract Task<CommandResult<T_Result>> InternalHandleAsync(T_Command command, CancellationToken cancellationToken = default);
}