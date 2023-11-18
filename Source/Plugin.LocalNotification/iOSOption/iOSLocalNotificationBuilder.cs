#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption
{
    /// <inheritdoc/>
    public class iOSLocalNotificationBuilder : IiOSLocalNotificationBuilder
    {
#if IOS
        /// <summary>
        ///
        /// </summary>
        internal UserNotificationCenterDelegate? CustomUserNotificationCenterDelegate { get; private set; }
#endif

#if IOS
        /// <inheritdoc/>
        public IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate)
        {
            CustomUserNotificationCenterDelegate = customUserNotificationCenterDelegate;
            return this;
        }
#endif
    }
}