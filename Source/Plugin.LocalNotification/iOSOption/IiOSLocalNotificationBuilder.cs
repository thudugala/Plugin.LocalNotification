#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Defines methods for building iOS-specific local notification options, including custom notification center delegate configuration.
/// </summary>
public interface IiOSLocalNotificationBuilder
{
#if IOS
    /// <summary>
    /// Sets a custom <see cref="UserNotificationCenterDelegate"/> for handling iOS notification events.
    /// Allows developers to extend or replace the default notification center delegate.
    /// </summary>
    /// <param name="customUserNotificationCenterDelegate">The custom notification center delegate to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate);
#endif
}