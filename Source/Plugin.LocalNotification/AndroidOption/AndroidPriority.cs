namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Set the relative priority for this notification.
/// Priority is an indication of how much of the user's valuable attention should be consumed by this notification.
/// Low-priority notifications may be hidden from the user in certain situations, while the user might be interrupted for a higher-priority notification.
/// The system sets a notification's priority based on various factors including the setPriority value.
/// The effect may differ slightly on different platforms.
/// </summary>
public enum AndroidPriority
{
    /// <summary>
    /// Lowest notification priority; these items might not be shown to the user except under special circumstances, such as detailed notification logs.
    /// </summary>
    Min = -2,

    /// <summary>
    /// Lower notification priority for items that are less important. The UI may show these items smaller or at a different position in the list compared to Default items.
    /// </summary>
    Low = -1,

    /// <summary>
    /// Default notification priority. Use this value if your application does not prioritize its own notifications.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Higher notification priority for more important notifications or alerts. The UI may show these items larger or at a different position in notification lists compared to Default items.
    /// </summary>
    High = 1,

    /// <summary>
    /// Highest notification priority for your application's most important items that require the user's prompt attention or input.
    /// </summary>
    Max = 2,
}