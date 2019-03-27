using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public static partial class NotificationCenter
    {
        private static INotificationService _current;

        /// <summary>
        ///
        /// </summary>
        public static INotificationService Current
        {
            get =>
                _current ?? throw new ArgumentException(
                    "[Plugin.LocalNotification] No platform plugin found.  Did you install the nuget package in your app project as well?");
            set => _current = value;
        }
    }
}