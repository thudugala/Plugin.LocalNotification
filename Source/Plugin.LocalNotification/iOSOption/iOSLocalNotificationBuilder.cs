#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption
{
    /// <inheritdoc/>
    public class iOSLocalNotificationBuilder : IiOSLocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        public bool UseCustomDelegate { get; set; }

#if IOS
        /// <summary>
        /// 
        /// </summary>
        public UserNotificationCenterDelegate CustomUserNotificationCenterDelegate { get; private set; }
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
