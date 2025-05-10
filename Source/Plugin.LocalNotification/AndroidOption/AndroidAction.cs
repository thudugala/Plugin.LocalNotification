namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
///
/// </summary>
public class AndroidAction
{
    /// <summary>
    ///
    /// </summary>
    public AndroidIcon IconName { get; set; } = new();

    /// <summary>
    /// Default is false
    /// </summary>
    public bool LaunchAppWhenTapped { get; set; } = false;

    /// <summary>
    ///
    /// </summary>
    public AndroidPendingIntentFlags PendingIntentFlags { get; set; } = AndroidPendingIntentFlags.CancelCurrent;
}