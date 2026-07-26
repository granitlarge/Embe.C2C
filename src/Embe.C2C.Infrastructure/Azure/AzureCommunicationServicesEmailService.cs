using Azure.Communication.Email;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;
using static Azure.WaitUntil;

namespace Embe.C2C.Infrastructure.Azure;

public class NullEmailService : IEmailService
{
    public NullEmailService()
    {

    }

    public Task SendAsync(string email, string subject, string htmlContent, string plainText, CancellationToken cancellationToken)
    {
        Console.WriteLine
        (
            $$"""
                Sending e-mail with content '{{plainText}}' to {{email}}.
            """
        );
        return Task.CompletedTask;
    }
}

public class AzureCommunicationServicesEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly EmailSettings _emailSettings;

    public AzureCommunicationServicesEmailService(Settings settings)
    {
        _emailSettings = settings.Email;
        var configuration = settings.Configuration;
        var connectionStringKey = "AzureCommunicationServices";
        _emailClient = new EmailClient(configuration.GetConnectionString(connectionStringKey) ?? throw new MissingConfigurationKeyException($"ConnectionStrings:{connectionStringKey}"));
    }

    public async Task SendAsync(string email, string subject, string htmlContent, string plainText, CancellationToken cancellationToken)
    {
        var sender = _emailSettings.Sender;
        var recipients = new EmailRecipients([new EmailAddress(email)]);
        var emailContent = new EmailContent(subject)
        {
            Html = htmlContent,
            PlainText = plainText
        };
        var message = new EmailMessage(sender, recipients, emailContent);
        await _emailClient.SendAsync(Completed, message, cancellationToken);
    }
}