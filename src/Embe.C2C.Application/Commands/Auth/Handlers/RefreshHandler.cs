using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class RefreshHandler : CommandHandler<RefreshCommand, TypedResult<RefreshFailureReason, Credentials>>
{
    private readonly IAuthService _authService;

    public RefreshHandler
    (
        IAuthService authService,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
    }

    protected override async Task<CommandResult<TypedResult<RefreshFailureReason, Credentials>>> HandleAsync
    (
        ISparseRepository context,
        RefreshCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.RefreshAsync(command.RefreshToken, cancellationToken);
        return new CommandResult<TypedResult<RefreshFailureReason, Credentials>>(result.IsSuccess, result);
    }
}