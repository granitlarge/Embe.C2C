using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class DeleteHandler : CommandHandler<DeleteCommand, Result>
{
    private readonly IAccountRepository _accountRepo;
    private readonly IUserRepository _userRepo;
    private readonly UserAuthorizationService _authorizationPolicy;
    private readonly UserService _userService;
    private readonly IAuthService _authService;

    public DeleteHandler
    (
        IAccountRepository accountRepo,
        IUserRepository userRepo,
        IRepository context,
        UserAuthorizationService authorizationPolicy,
        UserService userService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _userService = userService;
        _authService = authService;
        _userRepo = userRepo;
        _accountRepo = accountRepo;
    }

    protected override async Task<CommandResult<Result>> HandleAsync(ISparseRepository context, DeleteCommand command, CancellationToken cancellationToken = default)
    {
        var (permissions, variant) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Delete))
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You are not authorized to delete this user."));
        }

        var user = await _userRepo.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "User not found."));
        }

        var deleteIdentityUserResult = await _authService.DeleteUserAsync(user.IdentityUserId, cancellationToken);
        if (!deleteIdentityUserResult.IsSuccess)
        {
            return new CommandResult<Result>(false, Result.Failure(FailureReason.Unknown, deleteIdentityUserResult.Message!));
        }

        var accounts = await _accountRepo.GetByUserIdAsync(command.UserId, cancellationToken);

        _userService.Delete(user, [.. accounts]);
        _userRepo.Set.Remove(user);
        foreach (var account in accounts)
        {
            _accountRepo.Set.Remove(account);
        }

        return new CommandResult<Result>(true, Result.Success());
    }
}