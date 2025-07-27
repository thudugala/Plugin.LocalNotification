namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Launch the App instead of posting the notification to the status bar.
/// Only for use with extremely high-priority notifications demanding the user's immediate attention,
/// such as an incoming phone call or alarm clock that the user has explicitly set to a particular time.
/// If this facility is used for something else, please give the user an option to turn it off and use a normal notification, as this can be extremely disruptive.
/// The system UI may choose to display a heads-up notification, instead of launching this app, while the user is using the device.
/// Apps targeting Android Q 10 (29) and above will have to request a permission (Manifest.permission.USE_FULL_SCREEN_INTENT) in order to use full screen intents.
/// To be launched, the notification must also be posted to a channel with importance level set to IMPORTANCE_HIGH or higher.
/// </summary>
public class AndroidLaunch
{
    /// <summary>
    /// Gets or sets a value indicating whether the notification should be sent even if other notifications are suppressed. Default is <c>true</c>.
    /// </summary>
    public bool InHighPriority { get; set; } = true;
}
