using System;

namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    ///
    /// </summary>
    [Flags]
    public enum iOSAuthorizationOptions : ulong
    {
        /// <summary>
        ///
        /// </summary>
        None = 0,

        /// <summary>
        ///
        /// </summary>
        Badge = 1,

        /// <summary>
        ///
        /// </summary>
        Sound = 2,

        /// <summary>
        ///
        /// </summary>
        Alert = 4,

        /// <summary>
        ///
        /// </summary>
        CarPlay = 8,

        /// <summary>
        ///
        /// </summary>
        CriticalAlert = 16,

        /// <summary>
        ///
        /// </summary>
        ProvidesAppNotificationSettings = 32,

        /// <summary>
        ///
        /// </summary>
        Provisional = 64,

        /// <summary>
        ///
        /// </summary>
        Announcement = 128,

        /// <summary>
        ///
        /// </summary>
        TimeSensitive = 256
    }
}