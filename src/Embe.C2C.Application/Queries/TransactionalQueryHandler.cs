using Embe.C2C.Application.Abstractions.Repos;

namespace Embe.C2C.Application.Queries;

public abstract class TransactionalQueryHandler<T_Command, T_Result>
{
    private readonly IRepository _repository;

    public TransactionalQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<T_Result> HandleAsync(T_Command command, CancellationToken cancellationToken = default)
    {
        using var transaction = await _repository.BeginTransactionAsync(cancellationToken);
        var result = await ExecuteAsync(command, new SparseRepository(_repository), cancellationToken);
        return result;
    }

    protected abstract Task<T_Result> ExecuteAsync
    (
        T_Command command,
        ISparseRepository repository,
        CancellationToken cancellationToken = default
    );
}