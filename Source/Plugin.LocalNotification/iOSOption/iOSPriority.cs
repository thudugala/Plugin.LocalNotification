namespace Plugin.LocalNotification.iOSOption;

/// <summary>
///
/// </summary>
public enum iOSPriority
{
    /// <summary>
    /// The system adds the notification to the notification list without lighting up the screen or playing a sound.
    /// </summary>
    Passive = 0,

    /// <summary>
    /// The system presents the notification immediately, lights up the screen, and can play a sound. (default)
    /// </summary>
    Active = 1,

    /// <summary>
    /// The system presents the notification immediately, lights up the screen, and can play a sound, but won’t break through system notification controls.
    /// </summary>
    TimeSensitive = 2,

    /// <summary>
    /// The system presents the notification immediately, lights up the screen, and bypasses the mute switch to play a sound.
    /// </summary>
    Critical = 3
}