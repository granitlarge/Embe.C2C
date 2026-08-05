namespace Embe.C2C.Application.Commands.Users;

public record UpdateSettingsCommand
(
    UserSettingsWriteDto Settings
);

public record UserSettingsWriteDto
(
    bool EmailNotifications,
    bool DeviceNotifications,
    bool NotifyOnLike,
    bool NotifyOnMatch,
    bool NotifyOnMessage
);