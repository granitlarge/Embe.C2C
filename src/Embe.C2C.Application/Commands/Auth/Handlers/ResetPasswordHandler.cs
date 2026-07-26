using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class ResetPasswordHandler
(
    IAuthService authService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler
) : CommandHandler<ResetPasswordCommand, ErrorOr<Success>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthService _authService = authService;

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authService.ResetPasswordAsync(command.NewPassword, cancellationToken);
        return new(true, result);
    }
}