namespace Plugin.LocalNotification.Core.Models.AndroidOption;

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

    /// <summary>
    /// Optional text-input fields attached to this action, allowing the user to type a reply
    /// directly from the notification shade without opening the app.
    /// Each element adds one <c>RemoteInput</c> to the native action.
    /// </summary>
    public IList<AndroidActionInput> Inputs { get; set; } = [];
}