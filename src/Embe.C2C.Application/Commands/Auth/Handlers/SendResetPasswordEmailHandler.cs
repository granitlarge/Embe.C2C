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
        var htmlContent = ResetPasswordEmail.HtmlContent(link);
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

    public static string HtmlContent(string link) =>

    $$"""
        <!DOCTYPE html>
        <html>

            <head>
                <link rel="preconnect" href="https://fonts.googleapis.com">
                <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
                <link
                    href="https://fonts.googleapis.com/css2?family=Cal+Sans&family=Lato:ital,wght@0,100;0,300;0,400;0,700;0,900;1,100;1,300;1,400;1,700;1,900&family=Roboto:ital,wght@0,100..900;1,100..900&display=swap"
                    rel="stylesheet">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
            </head>
            <style>
                * {
                    box-sizing: border-box;
                    margin: 0;
                    padding: 0;
                }

                .body {
                    background-color: rgb(143, 151, 240);
                    font-family: 'Roboto', sans-serif;
                    display: flex;
                    flex-direction: column;
                    justify-items: center;
                    align-items: center;
                    justify-content: center;
                    width: 100dvw;
                    height: 100dvh;
                }

                .surface {
                    background-color: white;
                    border-radius: 10px;
                    padding: 5px;
                    display: flex;
                    flex-direction: column;
                    width: 90%;
                    height: 90%;
                    gap: 5px;
                    align-items: center;
                }

                h1 {
                    margin-inline: auto;
                    color: black;
                    padding: 10px;
                }

                p {
                    color: black;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    vertical-align: middle;
                    text-align: center;
                }
            </style>

            <body class="body">
                <div class="surface">
                    <h1>reset your password</h1>
                    <p>
                        Someone has requested to reset the password on your account.
                        If this wasn't you, you can ignore this e-mail.
                        Otherwise, click the link below to reset your password.
                        The link is valid for 20 minutes.
                    </p>
                    <a href="{{link}}">{{link}}</a>
                </div>
            </body>
        </html>
    """;
}