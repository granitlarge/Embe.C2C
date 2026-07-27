using Embe.C2C.Application.Events.Notifications;

namespace Embe.C2C.Application.Services;

public class EmailComposerService
{
    public EmailComposerService()
    {

    }

    public (string Subject, string HtmlContent, string PlainText) CreateMessage(NotificationCreatedIntegrationEvent created)
    {
        throw new NotImplementedException();
    }
}