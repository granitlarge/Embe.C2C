using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Services;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Verifications.Handlers;

public class SendVerificationEmailHandler
(
    IAuthService authService,
    IEmailService emailService,
    EmailComposerService emailComposerService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler
) : CommandHandler<SendVerificationEmailCommand, ErrorOr<Success>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthService _authService = authService;
    private readonly IEmailService _emailService = emailService;
    private readonly EmailComposerService _emailComposerService = emailComposerService;

    protected async override Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        SendVerificationEmailCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var verificationCode = await _authService.GenerateVerificationCodeAsync(command.Email, cancellationToken);
        var (subject, htmlContent, plainText) = await _emailComposerService.CreateVerificationEmailMessageAsync
        (
            verificationCode,
            cancellationToken
        );
        await _emailService.SendAsync(command.Email, subject, htmlContent, plainText, cancellationToken);
        return new(true, Result.Success);
    }
}