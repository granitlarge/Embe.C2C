using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class DeleteSearchProfileHandler
(
    ISearchProfileRepository searchProfileRepository,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    IAuthenticatedUserService authenticatedUserService,
    SearchProfileAuthorizationService searchProfileAuthorizationService
) : CommandHandler<DeleteSearchProfileCommand, ErrorOr<Success>>
    (
        domainEventStore,
        context,
        domainEventHandler,
        integrationEventHandler
    )
{
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync(DeleteSearchProfileCommand command, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("Authenticated user ID is null.");
        var (permissions, variant) = await _searchProfileAuthorizationService.GetAsync(command.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.Delete))
        {
            return new CommandResult<ErrorOr<Success>>
            (
                Save: false,
                Error.Forbidden("forbidden", "Authenticated user does not have permission to delete this search profile.")
            );
        }

        var searchProfile = await _searchProfileRepository.GetByIdAsync(command.Id, cancellationToken);
        if (searchProfile is null)
        {
            return new CommandResult<ErrorOr<Success>>
            (
                Save: false,
                Error.NotFound("search_profile_not_found", "Search profile not found.")
            );
        }

        searchProfile.Remove();
        _searchProfileRepository.Set.Remove(searchProfile);

        return new CommandResult<ErrorOr<Success>>
        (
            Save: true,
            Result.Success
        );
    }
}