#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption;

/// <summary>
///
/// </summary>
public interface IiOSLocalNotificationBuilder
{
#if IOS
    /// <summary>
    /// This allow developer to change UNUserNotificationCenterDelegate,
    /// extend Plugin.LocalNotification.Platform.iOS.UserNotificationCenterDelegate
    /// Create custom IUNUserNotificationCenterDelegate
    /// and set it using this method
    /// </summary>
    /// <param name="customUserNotificationCenterDelegate"></param>
    /// <returns></returns>
    IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate);
#endif
}