namespace Plugin.LocalNotification.EventArgs;

/// <summary>
/// Represents the method that will handle the event when a notification action is tapped.
/// </summary>
/// <param name="e">The event arguments containing details about the tapped notification action.</param>
public delegate void NotificationActionTappedEventHandler(NotificationActionEventArgs e);

/// <summary>
/// Provides data for the notification action tapped event, including action identifiers and tap/dismiss status.
/// </summary>
public class NotificationActionEventArgs : NotificationEventArgs
{
    /// <summary>
    /// The action to execute when the notification is explicitly dismissed by the user, either with the "Clear All" button or by swiping it away individually.
    /// </summary>
    public const int DismissedActionId = 1000000;

    /// <summary>
    /// The action to execute when the notification is tapped by the user.
    /// </summary>
    public const int TapActionId = 2000000;

    /// <summary>
    /// Gets or sets the tapped action identifier.
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the notification was explicitly dismissed by the user.
    /// </summary>
    public bool IsDismissed => ActionId == DismissedActionId;

    /// <summary>
    /// Gets a value indicating whether the notification was tapped by the user.
    /// </summary>
    public bool IsTapped => ActionId == TapActionId;
}