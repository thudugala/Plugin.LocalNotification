namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents Android-specific notification permission options, such as requesting permission to schedule exact alarms.
/// </summary>
public class AndroidNotificationPermission
{
    /// <summary>
    /// Gets or sets a value indicating whether to request permission to schedule exact alarms. Default is <c>false</c>.
    /// </summary>
    public bool RequestPermissionToScheduleExactAlarm { get; set; } = false;
}
