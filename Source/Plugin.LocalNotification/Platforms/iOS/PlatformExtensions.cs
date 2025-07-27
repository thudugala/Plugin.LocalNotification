using Plugin.LocalNotification.iOSOption;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// Provides extension methods for converting Plugin.LocalNotification iOS option types to native iOS types.
/// </summary>
public static class PlatformExtensions
{
    /// <summary>
    /// Converts a <see cref="iOSPriority"/> value to its native <see cref="UNNotificationInterruptionLevel"/> equivalent.
    /// </summary>
    /// <param name="priority">The priority value to convert.</param>
    /// <returns>The corresponding <see cref="UNNotificationInterruptionLevel"/> value.</returns>
    public static UNNotificationInterruptionLevel ToNative(this iOSPriority priority)
    {
        return !OperatingSystem.IsIOSVersionAtLeast(15)
            ? default
            : priority switch
        {
            iOSPriority.Passive => UNNotificationInterruptionLevel.Passive2,
            iOSPriority.Active => UNNotificationInterruptionLevel.Active2,
            iOSPriority.TimeSensitive => UNNotificationInterruptionLevel.TimeSensitive2,
            iOSPriority.Critical => UNNotificationInterruptionLevel.Critical2,
            _ => UNNotificationInterruptionLevel.Active2,
        };
    }

    /// <summary>
    /// Converts a <see cref="iOSAuthorizationOptions"/> value to its native <see cref="UNAuthorizationOptions"/> equivalent.
    /// </summary>
    /// <param name="type">The authorization options value to convert.</param>
    /// <returns>The corresponding <see cref="UNAuthorizationOptions"/> value.</returns>
    public static UNAuthorizationOptions ToNative(this iOSAuthorizationOptions type)
    {
        var nativeEnum = (UNAuthorizationOptions)type;
        return nativeEnum;
    }

    /// <summary>
    /// Converts a <see cref="iOSActionType"/> value to its native <see cref="UNNotificationActionOptions"/> equivalent.
    /// </summary>
    /// <param name="type">The action type value to convert.</param>
    /// <returns>The corresponding <see cref="UNNotificationActionOptions"/> value.</returns>
    public static UNNotificationActionOptions ToNative(this iOSActionType type) => type switch
    {
        iOSActionType.Foreground => UNNotificationActionOptions.Foreground,
        iOSActionType.Destructive => UNNotificationActionOptions.Destructive,
        iOSActionType.AuthenticationRequired => UNNotificationActionOptions.AuthenticationRequired,
        _ => UNNotificationActionOptions.None,
    };

    /// <summary>
    /// Converts a <see cref="NotificationCategoryType"/> value to its native string representation.
    /// </summary>
    /// <param name="type">The category type value to convert.</param>
    /// <returns>The string representation of the category type.</returns>
    public static string ToNative(this NotificationCategoryType type) => type.ToString();
}