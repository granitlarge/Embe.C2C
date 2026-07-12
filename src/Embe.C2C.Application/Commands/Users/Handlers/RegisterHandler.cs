using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : TransactionalCommandHandler<RegisterCommand, TypedResult<RegisterUserFailureReason, User>>
{
    private readonly IAuthService _authService;

    public RegisterHandler
    (
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
    }

    protected override async Task<TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>> HandleAsync(ISparseRepository context, RegisterCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var registerUserResult = await _authService.RegisterUserAsync(command.Email, command.Password, cancellationToken);
            if (!registerUserResult.IsSuccess)
            {
                return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(false, TypedResult<RegisterUserFailureReason, User>.Failure(registerUserResult.Reason, registerUserResult.Message!));
            }

            var email = Email.Create(command.Email);
            var alias = Alias.Create(command.Alias);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var location = new Location(command.Location.Latitude, command.Location.Longitude);

            var files = new HashSet<ImageDetails>();
            var identityUserId = registerUserResult.Value!.Id;
            var user = User.Register(email, alias, birthDate, gender: gender, location: location, images: null, bio: null, identityUserId);

            context.DomainUsers.Add(user);

            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(true, TypedResult<RegisterUserFailureReason, User>.Success(user));
        }
        catch (DomainException ex)
        {
            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(false, TypedResult<RegisterUserFailureReason, User>.Failure(RegisterUserFailureReason.DomainError, ex.Message));
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