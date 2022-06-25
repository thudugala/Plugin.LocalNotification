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
        public iOSNotificationPermission Permission { get; private set; }

#if IOS
        /// <summary>
        /// 
        /// </summary>
        public UserNotificationCenterDelegate CustomUserNotificationCenterDelegate { get; private set; }
#endif

        /// <summary>
        /// 
        /// </summary>
        public iOSLocalNotificationBuilder()
        {
            Permission = new iOSNotificationPermission();
        }

#if IOS
        /// <inheritdoc/>
        public IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate)
        {
            CustomUserNotificationCenterDelegate = customUserNotificationCenterDelegate;           
            return this;
        }
#endif

        /// <inheritdoc/>
        public IiOSLocalNotificationBuilder SetPermission(iOSNotificationPermission permission)
        {
            Permission = permission;
            return this;
        }
    }
}
