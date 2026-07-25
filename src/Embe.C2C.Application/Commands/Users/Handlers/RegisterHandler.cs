using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : CommandHandler<RegisterCommand, ErrorOr<Credentials>>
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

    protected override async Task<CommandResult<ErrorOr<Credentials>>> InternalHandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var registerUserResult = await _authService.RegisterUserAsync(command.Email, command.Password, cancellationToken);
        if (!registerUserResult.IsSuccess)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(registerUserResult.Errors)
            );
        }

        var errors = new List<Error>();
        var email = Email.Create(command.Email).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Email))));
        var alias = Alias.Create(command.Alias).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Alias))));
        var birthDate = BirthDate.Create(command.BirthDate).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.BirthDate))));

        if (errors.Count != 0)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(errors)
            );
        }

        var files = new HashSet<ImageDetails>();
        var identityUserId = registerUserResult.Value!.Id;
        var user = User
            .Register(email.Value, alias.Value, birthDate.Value, gender: null, location: null, images: null, bio: null, identityUserId)
            .ElseDo(e => errors.AddRange(e));

        if (errors.Count != 0)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(errors)
            );
        }

        _userRepo.Set.Add(user.Value);

        await _userRepo.SaveChangesAsync(cancellationToken);

        var signInResult = await _authService.SignInAsync(email.Value.Value, command.Password, cancellationToken);

        if (!signInResult.IsSuccess)
        {
            throw new NotImplementedException();
        }

        return new
        (
            true,
            ErrorOrFactory.From(signInResult.Value!)
        );
    }
}