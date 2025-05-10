namespace Plugin.LocalNotification.EventArgs;

/// <summary>
///
/// </summary>
public class NotificationEventReceivingArgs : NotificationEventArgs
{
    /// <summary>
    /// If set to true, Notification will not popup
    /// </summary>
    public bool Handled { get; set; } = false;
}