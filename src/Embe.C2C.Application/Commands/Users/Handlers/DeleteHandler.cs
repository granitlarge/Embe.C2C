using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class DeleteHandler : CommandHandler<DeleteCommand, ErrorOr<Success>>
{
    private readonly IAccountRepository _accountRepo;
    private readonly IUserRepository _userRepo;
    private readonly UserAuthorizationService _authorizationPolicy;
    private readonly UserService _userService;

    public DeleteHandler
    (
        IAccountRepository accountRepo,
        IUserRepository userRepo,
        IRepository context,
        UserAuthorizationService authorizationPolicy,
        UserService userService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
        _userRepo = userRepo;
        _accountRepo = accountRepo;
    }

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync(DeleteCommand command, CancellationToken cancellationToken = default)
    {
        var (permissions, _) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Delete))
        {
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        }

        var user = await _userRepo.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());
        }

        var accounts = await _accountRepo.GetByUserIdAsync(command.UserId, cancellationToken);

        _userService.Delete(user, [.. accounts]);
        _userRepo.Set.Remove(user);
        foreach (var account in accounts)
        {
            _accountRepo.Set.Remove(account);
        }

        return new(true, Result.Success);
    }
}