using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : TransactionalCommandHandler<RegisterCommand, TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>
{
    private readonly IAuthService _authService;
    private readonly UserAuthorizationService _userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper;

    public RegisterHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService,
        DomainEventStore domainEventStore,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
        _userAuthorizationService = userAuthorizationService;
        _userDtoMapper = userDtoMapper;
    }

    protected override async Task<TransactionalCommandResult<TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>> HandleAsync(ISparseRepository context, RegisterCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var registerUserResult = await _authService.RegisterUserAsync(command.Email, command.Password, cancellationToken);
            if (!registerUserResult.IsSuccess)
            {
                return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>(false, TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>.Failure(registerUserResult.Reason, registerUserResult.Message!));
            }

            var email = Email.Create(command.Email);
            var alias = Alias.Create(command.Alias);
            var birthDate = new BirthDate(command.BirthDate);

            var files = new HashSet<ImageDetails>();
            var identityUserId = registerUserResult.Value!.Id;
            var user = User.Register(email, alias, birthDate, gender: null, location: null, images: null, bio: null, identityUserId);

            context.DomainUsers.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            var dto = await user.Enrich(null).ToDtoAsync(_userAuthorizationService, _userDtoMapper, cancellationToken);

            if (dto == null)
                return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>(false, TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>.Failure(RegisterUserFailureReason.UnknownError, "The user does not have access to their own data."));

            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>(true, TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>.Success(dto));
        }
        catch (DomainException ex)
        {
            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>>(false, TypedResult<RegisterUserFailureReason, ReadDto<UserDto, UserPermission>>.Failure(RegisterUserFailureReason.DomainError, ex.Message));
        }
    }
}

public enum RegisterUserFailureReason
{
    EmailAlreadyExists = 0,
    DomainError = 1,
    WeakPassword = 2,
    Unknown = 3,
    UnknownError = 4
}