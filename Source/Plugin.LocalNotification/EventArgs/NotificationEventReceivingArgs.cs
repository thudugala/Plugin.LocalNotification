namespace Plugin.LocalNotification.EventArgs;

/// <summary>
/// Provides data for the notification event receiving event, including whether the notification was handled and should not popup.
/// </summary>
public class NotificationEventReceivingArgs : NotificationEventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the notification was handled and should not popup. Default is <c>false</c>.
    /// </summary>
    public bool Handled { get; set; } = false;
}