using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries;

public abstract class TransactionalQueryHandler<T_Query, T_Result>
{
    private readonly IRepository _repository;

    public TransactionalQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<T_Result> HandleAsync(T_Query command, CancellationToken cancellationToken = default)
    {
        using var transaction = await _repository.BeginTransactionAsync(serializable: false, cancellationToken);
        var result = await ExecuteAsync(command, cancellationToken);
        return result;
    }

    protected abstract Task<T_Result> ExecuteAsync
    (
        T_Query query,
        CancellationToken cancellationToken = default
    );
}