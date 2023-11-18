namespace Plugin.LocalNotification.EventArgs
{
    /// <summary>
    /// Returning event when tapped on notification action.
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationActionTappedEventHandler(NotificationActionEventArgs e);

    /// <summary>
    ///
    /// </summary>
    public class NotificationActionEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// The Action to execute when the notification is explicitly dismissed by the user,
        /// either with the "Clear All" button or by swiping it away individually.
        /// </summary>
        public const int DismissedActionId = 1000000;

        /// <summary>
        /// The Action to execute when the notification is tapped by the user.
        /// </summary>
        public const int TapActionId = 2000000;

        /// <summary>
        /// Tapped Action Id
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// True if the notification is explicitly dismissed by the user
        /// </summary>
        public bool IsDismissed => ActionId == DismissedActionId;

        /// <summary>
        /// True if the notification is explicitly dismissed by the user
        /// </summary>
        public bool IsTapped => ActionId == TapActionId;
    }
}