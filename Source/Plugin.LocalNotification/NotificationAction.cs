using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationAction
    {
        /// <summary>
        ///
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Action<int, string> Handler { get; set; }

        /// <summary>
        ///
        /// </summary>
        public iOSActionType iOSAction { get; set; } = iOSActionType.None;
    }
}