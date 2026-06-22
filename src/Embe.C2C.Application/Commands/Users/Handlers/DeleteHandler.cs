using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class DeleteHandler : TransactionalCommandHandler<DeleteCommand, Result>
{
    private readonly UserAuthorizationPolicy _authorizationPolicy;
    private readonly UserService _userService;
    private readonly IAuthService _authService;

    public DeleteHandler
    (
        IRepository context,
        UserAuthorizationPolicy authorizationPolicy,
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
    }

    protected override async Task<TransactionalCommandResult<Result>> HandleAsync(ISparseRepository context, DeleteCommand command, CancellationToken cancellationToken = default)
    {
        var (permissions, variant) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Delete))
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.Forbidden, "You are not authorized to delete this user."));
        }

        var user = await context.DomainUsersQuery.SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "User not found."));
        }

        var deleteIdentityUserResult = await _authService.DeleteUserAsync(user.IdentityUserId, cancellationToken);
        if (!deleteIdentityUserResult.IsSuccess)
        {
            return new TransactionalCommandResult<Result>(false, Result.Failure(FailureReason.Unknown, deleteIdentityUserResult.Message!));
        }

        var accounts = await context.AccountsQuery.Where(a => a.UserId == command.UserId).ToListAsync(cancellationToken);

        _userService.Delete(user, [.. accounts]);

        context.DomainUsers.Remove(user);
        foreach (var account in accounts)
        {
            context.Accounts.Remove(account);
        }

        return new TransactionalCommandResult<Result>(true, Result.Success());
    }
}