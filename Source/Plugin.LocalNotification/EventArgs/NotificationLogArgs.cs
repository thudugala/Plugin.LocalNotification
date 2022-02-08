using System;

namespace Plugin.LocalNotification.EventArgs
{
    /// <summary>
    /// Returning error
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationLogHandler(NotificationLogArgs e);

    /// <summary>
    ///
    /// </summary>
    public class NotificationLogArgs : System.EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public Exception Error { get; internal set; }
    }
}