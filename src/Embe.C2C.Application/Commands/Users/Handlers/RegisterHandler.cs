using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : CommandHandler<RegisterCommand, TypedResult<RegisterUserFailureReason, Credentials>>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthService _authService;

    public RegisterHandler
    (
        IUserRepository userRepo,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
        _userRepo = userRepo;
    }

    protected override async Task<CommandResult<TypedResult<RegisterUserFailureReason, Credentials>>> InternalHandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {

        try
        {

            var registerUserResult = await _authService.RegisterUserAsync(command.Email, command.Password, cancellationToken);
            if (!registerUserResult.IsSuccess)
            {
                return new CommandResult<TypedResult<RegisterUserFailureReason, Credentials>>
                (
                    false,
                    TypedResult<RegisterUserFailureReason, Credentials>.Failure
                    (
                        registerUserResult.Reason,
                        registerUserResult.Message!
                    )
                );
            }

            var email = Email.Create(command.Email);
            var alias = Alias.Create(command.Alias);
            var birthDate = new BirthDate(command.BirthDate);

            var files = new HashSet<ImageDetails>();
            var identityUserId = registerUserResult.Value!.Id;
            var user = User.Register(email, alias, birthDate, gender: null, location: null, images: null, bio: null, identityUserId);

            _userRepo.Set.Add(user);

            await _userRepo.SaveChangesAsync(cancellationToken);

            var signInResult = await _authService.SignInAsync(email.Value, command.Password, cancellationToken);

            if (!signInResult.IsSuccess)
            {
                throw new NotImplementedException();
            }

            return new CommandResult<TypedResult<RegisterUserFailureReason, Credentials>>
            (
                true, 
                TypedResult<RegisterUserFailureReason, Credentials>.Success
                (
                    signInResult.Value!
                )
            );

        }
        catch (DomainException ex)
        {
            return new CommandResult<TypedResult<RegisterUserFailureReason, Credentials>>
            (
                false,
                TypedResult<RegisterUserFailureReason, Credentials>.Failure
                (
                    RegisterUserFailureReason.DomainError,
                    ex.Message
                )
            );
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