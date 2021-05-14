using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public static partial class NotificationCenter
    {
        private static INotificationService _current;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get =>
                _current ?? throw new InvalidOperationException(Properties.Resources.PluginNotFound);
            set => _current = value;
        }
    }
}