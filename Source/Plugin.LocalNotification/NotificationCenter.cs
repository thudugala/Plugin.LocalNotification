using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>
#pragma warning disable CA1724

    public static partial class NotificationCenter
#pragma warning restore CA1724
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