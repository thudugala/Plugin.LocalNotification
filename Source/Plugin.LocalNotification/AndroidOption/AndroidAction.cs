namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents an action that can be performed from an Android local notification, such as tapping a button.
/// </summary>
public class AndroidAction
{
    /// <summary>
    /// Gets or sets the icon to display for the action.
    /// </summary>
    public AndroidIcon IconName { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the app should launch when the action is tapped. Default is <c>false</c>.
    /// </summary>
    public bool LaunchAppWhenTapped { get; set; } = false;

    /// <summary>
    /// Gets or sets the pending intent flags for the action. Default is <see cref="AndroidPendingIntentFlags.CancelCurrent"/>.
    /// </summary>
    public AndroidPendingIntentFlags PendingIntentFlags { get; set; } = AndroidPendingIntentFlags.CancelCurrent;
}