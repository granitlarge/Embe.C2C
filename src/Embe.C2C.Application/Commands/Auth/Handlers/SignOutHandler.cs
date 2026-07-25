using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SignOutHandler : CommandHandler<SignOutCommand, ErrorOr<bool>>
{
    private readonly IAuthService _authService;

    public SignOutHandler
    (
        DomainEventStore domainEventStore,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
    }

    protected override async Task<CommandResult<ErrorOr<bool>>> InternalHandleAsync
    (
        SignOutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _authService.SignOutAsync(command.RefreshToken, cancellationToken);
        return new CommandResult<ErrorOr<bool>>(result.IsSuccess, result);
    }
}