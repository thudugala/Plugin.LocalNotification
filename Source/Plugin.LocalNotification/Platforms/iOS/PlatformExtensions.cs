using Plugin.LocalNotification.iOSOption;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms
{
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
            if (!OperatingSystem.IsIOSVersionAtLeast(15))
            {
                return default;
            }

            switch (priority)
            {
                case iOSPriority.Passive:
                    return UNNotificationInterruptionLevel.Passive2;

                case iOSPriority.Active:
                    return UNNotificationInterruptionLevel.Active2;

                case iOSPriority.TimeSensitive:
                    return UNNotificationInterruptionLevel.TimeSensitive2;

                case iOSPriority.Critical:
                    return UNNotificationInterruptionLevel.Critical2;

                default:
                    return UNNotificationInterruptionLevel.Active2;
            }
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
        public static UNNotificationActionOptions ToNative(this iOSActionType type)
        {
            return type switch
            {
                iOSActionType.Foreground => UNNotificationActionOptions.Foreground,
                iOSActionType.Destructive => UNNotificationActionOptions.Destructive,
                iOSActionType.AuthenticationRequired => UNNotificationActionOptions.AuthenticationRequired,
                _ => UNNotificationActionOptions.None,
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToNative(this NotificationCategoryType type)
        {
            return type.ToString();
        }
    }
}