using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Services;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateHandler : CommandHandler<UpdateCommand, Result<ReadDto<UserDto, UserPermission>?>>
{
    private readonly ISearchProfileRepository _searchProfileRepository;
    private readonly IUserRepository _userRepo;
    private readonly IAuthenticatedUserService _user;
    private readonly UserAuthorizationService _authorizationPolicy;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly SearchProfileService _searchProfileService;

    public UpdateHandler
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
        SearchProfileService searchProfileService
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _user = user;
        _authorizationPolicy = authorizationPolicy;
        _userDtoMapper = userDtoMapper;
        _searchProfileService = searchProfileService;
        _userRepo = userRepo;
        _searchProfileRepository = searchProfileRepository;
    }

    protected override async Task<CommandResult<Result<ReadDto<UserDto, UserPermission>?>>> HandleAsync
    (
        ISparseRepository context,
        UpdateCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Update))
        {
            return new CommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.Forbidden, "User is not authorized to update this profile."));
        }

        var actorId = _user.UserId ?? throw new InvalidOperationException("User is not authenticated.");

        try
        {
            var alias = Alias.Create(command.Alias);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var location = command.Location != null ? new Location(command.Location.Latitude, command.Location.Longitude) : null;
            var bio = string.IsNullOrWhiteSpace(command.Bio) ? null : command.Bio;

            var user = await _userRepo.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                return new CommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.NotFound, "User not found."));
            }

            var isClearingLocation = command.Location == null && user.Location != null;

            user.UpdateAlias(actorId, alias);
            user.UpdateBirthDate(actorId, birthDate);
            user.UpdateGender(actorId, gender);
            user.UpdateLocation(actorId, location);
            user.UpdateBio(actorId, bio);

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

            var imagesToRemove = user.Images.Where(f => !command.ImagesToKeep?.Any(itk => itk.Id == f.Id) ?? true).ToList();
            foreach (var image in imagesToRemove)
            {
                user.RemoveImage(actorId, image.Id);
            }

            foreach (var image in command.ImagesToKeep ?? [])
            {
                user.ChangeImageOrder(actorId, image.Id, image.Order);
            }

            var queryingUser = await _userRepo.GetByIdAsync(actorId, cancellationToken);
            var enrichedUser = user.Enrich(queryingUser);
            var dto = await _userDtoMapper.ToDtoAsync(enrichedUser, variant, cancellationToken);
            var readDto = new ReadDto<UserDto, UserPermission>(dto!, permissions);

            return new CommandResult<Result<ReadDto<UserDto, UserPermission>?>>(true, Result<ReadDto<UserDto, UserPermission>?>.Success(readDto));
        }
        catch (DomainException)
        {
            return new CommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.DomainError, "Invalid input data."));
        }

    }
}