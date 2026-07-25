using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Errors.ValueObjects;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;
using ErrorOr;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class UpdateSearchProfileHandler
(
    ISearchProfileRepository searchProfileRepository,
    IUserRepository userRepo,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper,
    SearchProfileService searchProfileService,
    IAuthenticatedUserService authenticatedUserService

) : CommandHandler<UpdateSearchProfileCommand, ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;
    private readonly SearchProfileService _searchProfileService = searchProfileService;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;

    protected async override Task<CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>> InternalHandleAsync
    (
        UpdateSearchProfileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return new
            (
                false,
                ApplicationErrors.Forbidden.ToForbiddenErrorOr()
            );
        }

        var searchProfile = await _searchProfileRepository.GetByIdAsync(command.Id, cancellationToken);
        if (searchProfile is null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var (permissions, _) = await _searchProfileAuthorizationService.GetAsync(command.Id, cancellationToken);
        if (!permissions.Contains(SearchProfilePermission.Modify))
        {
            return new
            (
                false,
                ApplicationErrors.Forbidden.ToForbiddenErrorOr()
            );
        }

        var errors = new List<Error>();
        var engagement = Engagement.Create
        (
            command.Engagement.Medium,
            command.Engagement.Boundedness,
            command.Engagement.Frequency,
            command.Engagement.StartDate,
            command.Engagement.EndDate
        ).ElseDo(e => errors.AddRange(e.WithPropertyName("Engagement")));

        var genders = command.Genders.Count == 0 ? [.. Enum.GetValues<Gender>()] : command.Genders;
        var ageRangeMin = command.AgeRangeMin is not null ? Age.Create(command.AgeRangeMin.Value).ElseDo(e => errors.AddRange(e.WithPropertyName("AgeRangeMin"))) : default;
        var ageRangeMax = command.AgeRangeMax is not null ? Age.Create(command.AgeRangeMax.Value).ElseDo(e => errors.AddRange(e.WithPropertyName("AgeRangeMax"))) : default;
        var distance = command.MaximumDistanceKm is not null ? Distance.Create(command.MaximumDistanceKm.Value, LengthUnit.Kilometers).ElseDo(e => errors.AddRange(e.WithPropertyName("MaximumDistanceKm"))) : default;
        var active = command.Active;

        _searchProfileService.Update
        (
            user,
            searchProfile,
            command.Name,
            command.Description,
            command.RelationshipType,
            engagement.Value,
            genders,
            ageRangeMin.Value,
            ageRangeMax.Value,
            distance.Value,
            active
        );

        await _searchProfileRepository.SaveChangesAsync(cancellationToken);

        var dto = await searchProfile.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);
        if (dto is null)
        {
            return new
            (
                false,
                ApplicationErrors.Forbidden.ToForbiddenErrorOr()
            );
        }

        return new
        (
            true,
            ErrorOrFactory.From(dto)
        );
    }
}