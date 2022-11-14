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
            switch (type)
            {
                case iOSAuthorizationOptions.None:
                    return UNAuthorizationOptions.None;

                case iOSAuthorizationOptions.Badge:
                    return UNAuthorizationOptions.Badge;

                case iOSAuthorizationOptions.Sound:
                    return UNAuthorizationOptions.Sound;

                case iOSAuthorizationOptions.Alert:
                    return UNAuthorizationOptions.Alert;

                case iOSAuthorizationOptions.CarPlay:
                    return UNAuthorizationOptions.CarPlay;
            }

#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0) == false)
            {
                return UNAuthorizationOptions.None;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(12))
            {
                return UNAuthorizationOptions.None;
            }
#endif
            switch (type)
            {
                case iOSAuthorizationOptions.CriticalAlert:
                    return UNAuthorizationOptions.CriticalAlert;

                case iOSAuthorizationOptions.ProvidesAppNotificationSettings:
                    return UNAuthorizationOptions.ProvidesAppNotificationSettings;

                case iOSAuthorizationOptions.Provisional:
                    return UNAuthorizationOptions.Provisional;
            }

#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0) == false)
            {
                return UNAuthorizationOptions.None;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(13))
            {
                return UNAuthorizationOptions.None;
            }
#endif
            switch (type)
            {
                case iOSAuthorizationOptions.Announcement:
                    return UNAuthorizationOptions.Announcement;
            }

#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0) == false)
            {
                return UNAuthorizationOptions.None;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(15))
            {
                return UNAuthorizationOptions.None;
            }
#endif
            switch (type)
            {
                case iOSAuthorizationOptions.TimeSensitive:
                    return UNAuthorizationOptions.TimeSensitive;

                default:
                    return UNAuthorizationOptions.None;
            }
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

