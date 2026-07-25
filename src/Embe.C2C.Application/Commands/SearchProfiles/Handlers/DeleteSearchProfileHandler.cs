using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Errors;
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
        var (permissions, _) = await _searchProfileAuthorizationService.GetAsync(command.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.Delete))
        {
            return new
            (
                Save: false,
                ApplicationErrors.Forbidden.ToNotFoundErrorOr()
            );
        }

        var searchProfile = await _searchProfileRepository.GetByIdAsync(command.Id, cancellationToken);
        if (searchProfile is null)
        {
            return new
            (
                Save: false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        searchProfile.Remove();
        _searchProfileRepository.Set.Remove(searchProfile);

        return new
        (
            Save: true,
            Result.Success
        );
    }
}