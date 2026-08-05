using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateSettingsHandler
(
    IUserRepository userRepository,
    IAuthenticatedUserService authenticatedUserService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    UserDtoMapper userDtoMapper
) : CommandHandler<UpdateSettingsCommand, ErrorOr<ReadDto<UserDto, UserPermission>>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{

    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

    protected override async Task<CommandResult<ErrorOr<ReadDto<UserDto, UserPermission>>>> InternalHandleAsync
    (
        UpdateSettingsCommand command,
        CancellationToken cancellationToken = default
    )
    {

        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("not authenticated");
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return new(false, ApplicationErrors.NotFound.ToNotFoundErrorOr());
        }

        var settings = new UserSettings
        (
            command.Settings.EmailNotifications,
            command.Settings.DeviceNotifications,
            command.Settings.NotifyOnLike,
            command.Settings.NotifyOnMatch,
            command.Settings.NotifyOnMessage
        );

        user.UpdateSettings(settings);

        var dto = await _userDtoMapper.ToDtoAsync(user, user, cancellationToken);

        if (dto is null)
            return new(false, ApplicationErrors.Forbidden.ToForbiddenErrorOr());
        
        return new(true, dto);

    }

}