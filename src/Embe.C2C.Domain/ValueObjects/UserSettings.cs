namespace Embe.C2C.Domain.ValueObjects;

public record UserSettings
(
    bool EmailNotifications,
    bool DeviceNotifications,
    bool NotifyOnLike,
    bool NotifyOnMatch,
    bool NotifyOnMessage
);