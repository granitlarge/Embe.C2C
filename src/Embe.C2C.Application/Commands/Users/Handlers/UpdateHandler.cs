using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateHandler
(
    ISearchProfileRepository searchProfileRepository,
    IUserRepository userRepo,
    IAuthenticatedUserService user,
    IRepository context,
    UserAuthorizationService authorizationPolicy,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    DomainEventStore domainEventStore,
    UserDtoMapper userDtoMapper,
    SearchProfileService searchProfileService,
    ILoggerFactory loggerFactory
) : CommandHandler<UpdateCommand, ErrorOr<ReadDto<UserDto, UserPermission>?>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IAuthenticatedUserService _user = user;
    private readonly UserAuthorizationService _authorizationPolicy = authorizationPolicy;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;
    private readonly SearchProfileService _searchProfileService = searchProfileService;
    private readonly ILogger<UpdateHandler> _logger = loggerFactory.Create<UpdateHandler>();

    protected override async Task<CommandResult<ErrorOr<ReadDto<UserDto, UserPermission>?>>> InternalHandleAsync
    (
        UpdateCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Update))
        {
            return new
            (
                false,
                ApplicationErrors.Forbidden.ToForbiddenErrorOr()
            );
        }

        var actorId = _user.UserId ?? throw new InvalidOperationException("User is not authenticated.");

        var errors = new List<Error>();
        var alias = Alias.Create(command.Alias).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Alias))));
        var birthDate = BirthDate.Create(command.BirthDate).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.BirthDate))));
        var gender = command.Gender;
        var location = command.Location != null ?
            Location
                .Create(command.Location.Latitude, command.Location.Longitude)
                .ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Location)))) :
            default;
        var bio = string.IsNullOrWhiteSpace(command.Bio) ? null : command.Bio;
        if (errors.Count != 0)
        {
            return new(false, errors);
        }

        var user = await _userRepo.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());
        }

        var isClearingLocation = command.Location == null && user.Location != null;

        user.UpdateAlias(alias.Value);
        user.UpdateBirthDate(birthDate.Value);
        user.UpdateGender(gender);
        user.UpdateLocation(location.Value);
        user.UpdateBio(bio);

        // If the user clears his location, disable the maximum distance filter on all of his search profiles.
        if (isClearingLocation)
        {
            var searchProfilesWithDistanceFilter = await _searchProfileRepository.GetByUserIdAndHasMaximumDistanceFilter(user.Id, cancellationToken);
            foreach (var sp in searchProfilesWithDistanceFilter)
            {
                _searchProfileService.Update
                (
                    user,
                    sp,
                    sp.Name,
                    sp.Description,
                    sp.RelationshipType,
                    sp.Engagement,
                    [.. sp.Genders.Select(g => g.Gender)],
                    sp.AgeRangeMin,
                    sp.AgeRangeMax,
                    null,
                    sp.Active
                );
            }
        }

        var imagesToRemove = user.Images.Where(f => !(command.ImagesToKeep?.Any(itk => itk.Id == f.Id) ?? false)).ToList();
        await _logger.TraceAsync($"Removing {imagesToRemove.Count} images.");
        foreach (var image in imagesToRemove)
        {
            user.RemoveImage(image.Id);
        }

        foreach (var image in command.ImagesToKeep ?? [])
        {
            user.ChangeImageOrder(image.Id, image.Order);
        }

        var queryingUser = await _userRepo.GetByIdAsync(actorId, cancellationToken);
        var readDto = await _userDtoMapper.ToDtoAsync(user, queryingUser, cancellationToken);
        return new(true, readDto);
    }
}