using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;

namespace Embe.C2C.Application.Commands;

public record CommandResult<T>(bool CommitChanges, T Value);

public abstract class CommandHandler<T_Command, T_Result>
{
    private readonly DomainEventStore _domainEventStore;
    private readonly IRepository _context;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly IntegrationEventHandler _integrationEventHandler;

    public CommandHandler
    (
        DomainEventStore domainEventStore,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler
    )
    {
        _domainEventStore = domainEventStore;
        _context = context;
        _domainEventHandler = domainEventHandler;
        _integrationEventHandler = integrationEventHandler;
    }

    public async Task<T_Result> HandleAsync
    (
        T_Command command, 
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        var result = await HandleAsync(new SparseRepository(_context), command, cancellationToken);

        foreach (var domainEvent in _context.DomainEvents.Union(_domainEventStore.DomainEvents))
        {
            await _domainEventHandler.HandleAsync(domainEvent, cancellationToken);
        }

        if (result.CommitChanges)
        {
            await _context.SaveChangesAsync(cancellationToken);
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

    protected abstract Task<CommandResult<T_Result>> HandleAsync(ISparseRepository context, T_Command command, CancellationToken cancellationToken = default);
}