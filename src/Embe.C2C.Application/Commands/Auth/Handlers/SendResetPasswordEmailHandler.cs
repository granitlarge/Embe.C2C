using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Abstractions.Settings;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Auth.Handlers;

public class SendResetPasswordEmailHandler
(
    ISettings settings,
    IAuthService authService,
    IUserRepository userRepository,
    IEmailService emailService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler
) : CommandHandler<SendResetPasswordEmailCommand, ErrorOr<Success>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly ISettings _settings = settings;
    private readonly IAuthService _authService = authService;
    private readonly IEmailService _emailService = emailService;
    private readonly IUserRepository _userRepository = userRepository;

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        SendResetPasswordEmailCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (user is null)
        {
            return new(false, ApplicationErrors.NoUserWithSuppliedEmail.ToValidationErrorOr());
        }

        var link = await _authService.GeneratePasswordResetLinkAsync(user.Email.Value, cancellationToken);
        var plainTextContent = ResetPasswordEmail.PlainTextContent(link);
        var htmlContent = Emails.Template(_settings.Application.Name, "reset your password", ResetPasswordEmail.HtmlBody(link));
        await _emailService.SendAsync(command.Email, $"{_settings.Application.Name} | Reset Your Password", htmlContent, plainTextContent, cancellationToken);
        return new(true, Result.Success);
    }
}

public static class ResetPasswordEmail
{
    public static string PlainTextContent(string link) =>
    $$"""
        Someone has requested to reset the password on your account.
        If this wasn't you, ignore this e-mail.
        Otherwise, click the link below to reset your password.
        The link is valid for 20 minutes.

        {{link}}
    """;

    public static string HtmlBody(string link) =>

    $$"""
        <p>
            Someone has requested to reset the password on your account.
            If this wasn't you, you can ignore this e-mail.
            Otherwise, click the link below to reset your password.
            The link is valid for 20 minutes.
        </p>
        <a href="{{link}}">{{link}}</a>
    """;
}