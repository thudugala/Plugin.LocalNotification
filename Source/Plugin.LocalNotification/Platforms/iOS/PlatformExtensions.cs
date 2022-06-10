using Plugin.LocalNotification.iOSOption;
using System;
using UIKit;
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
#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0) == false)
            {
                return default;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(15))
            {
                return default;
            }
#endif

            switch (priority)
            {
                case iOSPriority.Passive:
                    return UNNotificationInterruptionLevel.Passive;

                case iOSPriority.Active:
                    return UNNotificationInterruptionLevel.Active;

                case iOSPriority.TimeSensitive:
                    return UNNotificationInterruptionLevel.TimeSensitive;

                case iOSPriority.Critical:
                    return UNNotificationInterruptionLevel.Critical;

                default:
                    return UNNotificationInterruptionLevel.Active;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static UNNotificationActionOptions ToNative(this iOSActionType type)
        {
            switch (type)
            {
                case iOSActionType.Foreground:
                    return UNNotificationActionOptions.Foreground;

                case iOSActionType.Destructive:
                    return UNNotificationActionOptions.Destructive;

                case iOSActionType.AuthenticationRequired:
                    return UNNotificationActionOptions.AuthenticationRequired;

                default:
                    return UNNotificationActionOptions.None;
            }
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

