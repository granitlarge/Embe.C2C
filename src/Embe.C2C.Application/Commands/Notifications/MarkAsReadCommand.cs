namespace Embe.C2C.Application.Commands.Notifications;

public record MarkAsReadCommand(Guid NotificationId, bool IsRead);
