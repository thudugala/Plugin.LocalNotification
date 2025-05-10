using Plugin.LocalNotification.iOSOption;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
///
/// </summary>
public static class PlatformExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
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
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static UNAuthorizationOptions ToNative(this iOSAuthorizationOptions type)
    {
        var nativeEnum = (UNAuthorizationOptions)type;
        return nativeEnum;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static UNNotificationActionOptions ToNative(this iOSActionType type) => type switch
    {
        iOSActionType.Foreground => UNNotificationActionOptions.Foreground,
        iOSActionType.Destructive => UNNotificationActionOptions.Destructive,
        iOSActionType.AuthenticationRequired => UNNotificationActionOptions.AuthenticationRequired,
        _ => UNNotificationActionOptions.None,
    };

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToNative(this NotificationCategoryType type) => type.ToString();
}