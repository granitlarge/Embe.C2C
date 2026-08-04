using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class ChangeEmailHandler
(
    IAuthenticatedUserService authenticatedUserService,
    IRepository context,
    IAuthService authService,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore
) : CommandHandler<ChangeEmailCommand, ErrorOr<Success>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{
    private readonly IAuthService _authService = authService;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        ChangeEmailCommand command, 
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var newEmail = Email.Create(command.NewEmail);
        if (newEmail.IsError)
            return new(false, newEmail.Errors);
        var result = await _authService.ChangeEmailAsync(userId, newEmail.Value, cancellationToken);
        return new(true, result);
    }
}