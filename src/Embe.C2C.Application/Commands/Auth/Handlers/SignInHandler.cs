using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SignInHandler
(
    IAuthService authService, 
    IRepository context, 
    DomainEventHandler domainEventHandler, 
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore
) : CommandHandler<SignInCommand, TypedResult<SignInFailureReason, Credentials>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{
    private readonly IAuthService _authService = authService;

    protected override async Task<CommandResult<TypedResult<SignInFailureReason, Credentials>>> HandleAsync(ISparseRepository context, SignInCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authService.SignInAsync(command.Email, command.Password, cancellationToken);
        return new CommandResult<TypedResult<SignInFailureReason, Credentials>>(result.IsSuccess, result);
    }
}