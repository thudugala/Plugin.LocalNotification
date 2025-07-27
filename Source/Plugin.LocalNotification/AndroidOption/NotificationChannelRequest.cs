namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents a request to create or configure an Android notification channel, including importance, sound, vibration, and other options.
/// </summary>
public class NotificationChannelRequest
{
    private string id = AndroidOptions.DefaultChannelId;
    private string name = AndroidOptions.DefaultChannelName;

    /// <summary>
    /// Gets or sets the level of interruption (importance) for this notification channel.
    /// </summary>
    public AndroidImportance Importance { get; set; } = AndroidImportance.Default;

    /// <summary>
    /// Gets or sets the id of the channel. Must be unique per package. The value may be truncated if it is too long.
    /// Also, <c>NotificationRequest.Android.ChannelId</c> must be set to the same Id to target this channel.
    /// </summary>
    public string Id
    {
        get => string.IsNullOrWhiteSpace(id) ? AndroidOptions.DefaultChannelId : id;
        set => id = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultChannelId : value;
    }

    /// <summary>
    /// Gets or sets the user-visible name of this channel. Default is "General".
    /// </summary>
    public string Name
    {
        get => string.IsNullOrWhiteSpace(name) ? AndroidOptions.DefaultChannelName : name;
        set => name = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultChannelName : value;
    }

    /// <summary>
    /// Gets or sets the user-visible description of this channel.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group this channel belongs to.
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification light color for notifications posted to this channel, if the device supports that feature.
    /// </summary>
    public AndroidColor LightColor { get; set; } = new();

    /// <summary>
    /// Gets or sets the sound file name for the notification.
    /// </summary>
    public string Sound { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether notifications posted to this channel should play sound.
    /// </summary>
    public bool EnableSound { get; set; } = true;

    /// <summary>
    /// Gets or sets the vibration pattern for the channel. Only modifiable before the channel is submitted.
    /// </summary>
    public long[] VibrationPattern { get; set; } = [];

    /// <summary>
    /// Gets or sets whether notifications posted to this channel are shown on the lock screen in full or redacted form.
    /// </summary>
    public AndroidVisibilityType LockScreenVisibility { get; set; } = AndroidVisibilityType.Private;

    /// <summary>
    /// Gets or sets a value indicating whether notifications posted to this channel can appear as application icon badges in a Launcher.
    /// </summary>
    public bool ShowBadge { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether notifications posted to this channel should display notification lights, on devices that support that feature.
    /// </summary>
    public bool EnableLights { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether notifications posted to this channel should vibrate. The vibration pattern can be set with <c>VibrationPattern</c>.
    /// </summary>
    public bool EnableVibration { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether notifications posted to this channel can bypass DND (Do Not Disturb) mode.
    /// </summary>
    public bool CanBypassDnd { get; set; } = false;
}