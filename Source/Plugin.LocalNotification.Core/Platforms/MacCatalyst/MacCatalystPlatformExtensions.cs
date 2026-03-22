using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AppleOption;
using UserNotifications;

namespace Plugin.LocalNotification.Core.Platforms.MacCatalyst;

/// <summary>
/// Provides extension methods for converting Plugin.LocalNotification IOS option types to native IOS types.
/// </summary>
public static class MacCatalystPlatformExtensions
{
    /// <summary>
    /// Converts a <see cref="ApplePriority"/> value to its native <see cref="UNNotificationInterruptionLevel"/> equivalent.
    /// </summary>
    /// <param name="priority">The priority value to convert.</param>
    /// <returns>The corresponding <see cref="UNNotificationInterruptionLevel"/> value.</returns>
    public static UNNotificationInterruptionLevel ToNative(this ApplePriority priority)
    {
        return !OperatingSystem.IsIOSVersionAtLeast(15)
            ? default
            : priority switch
        {
            ApplePriority.Passive => UNNotificationInterruptionLevel.Passive2,
            ApplePriority.Active => UNNotificationInterruptionLevel.Active2,
            ApplePriority.TimeSensitive => UNNotificationInterruptionLevel.TimeSensitive2,
            ApplePriority.Critical => UNNotificationInterruptionLevel.Critical2,
            _ => UNNotificationInterruptionLevel.Active2,
        };
    }

    /// <summary>
    /// Converts a <see cref="AppleAuthorizationOptions"/> value to its native <see cref="UNAuthorizationOptions"/> equivalent.
    /// </summary>
    /// <param name="type">The authorization options value to convert.</param>
    /// <returns>The corresponding <see cref="UNAuthorizationOptions"/> value.</returns>
    public static UNAuthorizationOptions ToNative(this AppleAuthorizationOptions type)
    {
        var nativeEnum = (UNAuthorizationOptions)type;
        return nativeEnum;
    }

    /// <summary>
    /// Converts a <see cref="AppleActionType"/> value to its native <see cref="UNNotificationActionOptions"/> equivalent.
    /// </summary>
    /// <param name="type">The action type value to convert.</param>
    /// <returns>The corresponding <see cref="UNNotificationActionOptions"/> value.</returns>
    public static UNNotificationActionOptions ToNative(this AppleActionType type) => type switch
    {
        AppleActionType.Foreground => UNNotificationActionOptions.Foreground,
        AppleActionType.Destructive => UNNotificationActionOptions.Destructive,
        AppleActionType.AuthenticationRequired => UNNotificationActionOptions.AuthenticationRequired,
        _ => UNNotificationActionOptions.None,
    };

    /// <summary>
    /// Converts a <see cref="NotificationCategoryType"/> value to its native string representation.
    /// </summary>
    /// <param name="type">The category type value to convert.</param>
    /// <returns>The string representation of the category type.</returns>
    public static string ToNative(this NotificationCategoryType type) => type.ToString();
}