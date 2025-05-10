namespace Plugin.LocalNotification;

/// <summary>
///
/// </summary>
public enum NotificationCategoryType
{
    /// <summary>
    ///
    /// </summary>
    None,

    /// <summary>
    /// Ongoing information about device or contextual status
    /// </summary>
    Status,

    /// <summary>
    /// Alarm or timer
    /// </summary>
    Alarm,

    /// <summary>
    /// User-scheduled reminder
    /// </summary>
    Reminder,

    /// <summary>
    /// Calendar event
    /// </summary>
    Event,

    /// <summary>
    /// Error in background operation or authentication status
    /// </summary>
    Error,

    /// <summary>
    /// Progress of a long-running background operation
    /// </summary>
    Progress,

    /// <summary>
    /// Promotion or advertisement
    /// </summary>
    Promo,

    /// <summary>
    /// A specific, timely recommendation for a single thing. For example, a news app might want to recommend a news story it believes the user will want to read next
    /// </summary>
    Recommendation,

    /// <summary>
    /// Indication of running background service
    /// </summary>
    Service,
}