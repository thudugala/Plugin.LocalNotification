namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents a progress bar for Android notifications, allowing configuration of indeterminate mode, maximum value, and current progress.
/// </summary>
public class AndroidProgressBar
{
    /// <summary>
    /// Gets or sets a value indicating whether this progress bar is in indeterminate mode.
    /// </summary>
    public bool IsIndeterminate { get; set; }

    /// <summary>
    /// Gets or sets the upper limit of this progress bar's range.
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Gets or sets the current level of progress for this progress bar.
    /// </summary>
    public int Progress { get; set; }
}