#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    ///
    /// </summary>
    public interface IiOSLocalNotificationBuilder
    {
#if IOS
        /// <summary>
        ///
        /// </summary>
        /// <param name="customUserNotificationCenterDelegate"></param>
        /// <returns></returns>
        IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate);
#endif
    }
}