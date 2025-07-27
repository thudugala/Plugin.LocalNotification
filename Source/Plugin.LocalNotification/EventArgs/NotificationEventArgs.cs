namespace Plugin.LocalNotification.EventArgs;

/// <summary>
/// Represents the method that will handle the event when a notification is received.
/// On iOS, this event is fired only when the app is in the foreground.
/// </summary>
/// <param name="e">The event arguments containing details about the received notification.</param>
public delegate void NotificationReceivedEventHandler(NotificationEventArgs e);

/// <summary>
/// Represents the method that will handle the event when notifications are disabled.
/// </summary>
public delegate void NotificationDisabledEventHandler();

/// <summary>
/// Provides data for notification events, including the notification request details.
/// </summary>
public class NotificationEventArgs : System.EventArgs
{
    /// <summary>
    /// Gets or sets the notification request associated with the event.
    /// </summary>
    public NotificationRequest Request { get; set; } = new ();
}