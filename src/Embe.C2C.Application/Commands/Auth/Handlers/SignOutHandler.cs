using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SignOutHandler : TransactionalCommandHandler<SignOutCommand, TypedResult<SignOutFailureReason, bool>>
{
    private readonly IAuthService _authService;

    public SignOutHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
    }

    protected override async Task<TransactionalCommandResult<TypedResult<SignOutFailureReason, bool>>> HandleAsync
    (
        ISparseRepository context,
        SignOutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.SignOutAsync(command.RefreshToken, cancellationToken);
        return new TransactionalCommandResult<TypedResult<SignOutFailureReason, bool>>(result.IsSuccess, result);
    }
}