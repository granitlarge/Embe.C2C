using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record UserSettingsDto
(
    bool EmailNotifications,
    bool DeviceNotifications,
    bool NotifyOnLike,
    bool NotifyOnMatch,
    bool NotifyOnMessage
);

public static class UserSettingsDtoExtensions
{
    public static UserSettingsDto ToDo(this UserSettings settings)
    {
        return new UserSettingsDto
        (
            settings.EmailNotifications,
            settings.DeviceNotifications,
            settings.NotifyOnLike,
            settings.NotifyOnMatch,
            settings.NotifyOnMessage
        );
    }
}