using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class RefreshHandler : TransactionalCommandHandler<RefreshCommand, TypedResult<RefreshFailureReason, Credentials>>
{
    private readonly IAuthService _authService;

    public RefreshHandler
    (
        IAuthService authService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
    }

    protected override async Task<TransactionalCommandResult<TypedResult<RefreshFailureReason, Credentials>>> HandleAsync
    (
        ISparseRepository context,
        RefreshCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.RefreshAsync(command.RefreshToken, cancellationToken);
        return new TransactionalCommandResult<TypedResult<RefreshFailureReason, Credentials>>(result.IsSuccess, result);
    }
}