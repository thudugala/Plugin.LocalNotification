namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Represents geofence options for iOS notifications, including whether the notification should repeat.
/// </summary>
public class iOSGeofenceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the geofence notification should repeat. If true, you must explicitly remove the notification request to stop delivery. Default is <c>false</c>.
    /// </summary>
    public bool Repeats { get; set; } = false;
}