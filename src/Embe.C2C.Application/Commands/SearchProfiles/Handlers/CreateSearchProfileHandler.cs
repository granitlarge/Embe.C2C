using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;
using ErrorOr;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class CreateSearchProfileHandler
(
    ISearchProfileRepository searchProfileRepository,
    IUserRepository userRepo,
    IAuthenticatedUserService authenticatedUserService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper,
    SearchProfileService searchProfileService

) : CommandHandler<CreateSearchProfileCommand, ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;
    private readonly SearchProfileService _searchProfileService = searchProfileService;

    protected async override Task<CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>> InternalHandleAsync
    (
        CreateSearchProfileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User must be authenticated");
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return new CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                false,
                Error.Forbidden("forbidden", "Authenticated user does not exist in the system.")
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
        var maximumDistanceKm = command.MaximumDistanceKm != null ? Distance.Create(command.MaximumDistanceKm.Value, LengthUnit.Kilometers) : default;

        if (errors.Count > 0)
        {
            return new CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                false,
                errors
            );
        }

        var searchProfile = _searchProfileService.Create
        (
            user,
            command.Name,
            command.Description,
            command.RelationshipType,
            engagement.Value,
            genders,
            ageRangeMin.Value,
            ageRangeMax.Value,
            maximumDistanceKm.Value
        );

        if (searchProfile.IsError)
        {
            return new CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                false,
                searchProfile.Errors
            );
        }

        _searchProfileRepository.Set.Add(searchProfile.Value);
        await _searchProfileRepository.SaveChangesAsync(cancellationToken);

        var dto = await searchProfile.Value.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);
        if (dto is null)
        {
            return new CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                false,
                Error.Forbidden("forbidden", "Authenticated user does not have permission to view the search profile.")
            );
        }

        return new CommandResult<ErrorOr<ReadDto<SearchProfileDto, SearchProfilePermission>>>
        (
            true,
            dto
        );
    }
}