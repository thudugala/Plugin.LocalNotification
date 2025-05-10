namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Set the progress this notification represents. The platform template will represent this using a ProgressBar.
/// </summary>
public class AndroidProgressBar
{
    /// <summary>
    /// Set whether this progress bar is in indeterminate mode
    /// </summary>
    public bool IsIndeterminate { get; set; }

    /// <summary>
    /// Set Upper limit of this progress bar's range
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Set progress bar's current level of progress
    /// </summary>
    public int Progress { get; set; }
}