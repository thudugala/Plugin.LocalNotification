namespace Plugin.LocalNotification.iOSOption;

/// <summary>
///
/// </summary>
public class iOSGeofenceOptions
{
    /// <summary>
    /// If you specify true for the repeats parameter,
    /// you must explicitly remove the notification request to stop the delivery of the associated notification
    /// Defualt value is false
    /// </summary>
    public bool Repeats { get; set; } = false;
}