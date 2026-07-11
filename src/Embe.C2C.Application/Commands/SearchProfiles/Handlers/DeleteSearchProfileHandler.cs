using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class DeleteSearchProfileHandler
(
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    IAuthenticatedUserService authenticatedUserService,
    SearchProfileAuthorizationService searchProfileAuthorizationService
) : TransactionalCommandHandler<DeleteSearchProfileCommand, Result>
    (
        domainEventStore,
        context,
        domainEventHandler,
        integrationEventHandler
    )
{
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;

    protected async override Task<TransactionalCommandResult<Result>> HandleAsync(ISparseRepository context, DeleteSearchProfileCommand command, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user ID is null.");
        var (permissions, variant) = await _searchProfileAuthorizationService.GetAsync(command.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.Delete))
        {
            return new TransactionalCommandResult<Result>
            (
                CommitChanges: false,
                Result.Failure
                (
                    FailureReason.Forbidden,
                    "User does not have permission to delete this search profile."
                )
            );
        }

        var searchProfile = await context.SearchProfilesQuery.FirstOrDefaultAsync(sp => sp.Id == command.Id, cancellationToken: cancellationToken);
        if (searchProfile is null)
        {
            return new TransactionalCommandResult<Result>
            (
                CommitChanges: false,
                Result.Failure
                (
                    FailureReason.NotFound,
                    "Search profile not found."
                )
            );
        }

        context.SearchProfiles.Remove(searchProfile);

        searchProfile.Remove();
        return new TransactionalCommandResult<Result>
        (
            CommitChanges: true,
            Result.Success()
        );
    }
}