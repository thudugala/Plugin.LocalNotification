namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents geofence options for Android local notifications, allowing configuration of expiration, loitering delay, and responsiveness.
/// </summary>
public class AndroidGeofenceOptions
{
    /// <summary>
    /// Gets or sets the expiration duration of the geofence in milliseconds.
    /// The geofence will be removed automatically after this period of time.
    /// Use -1 to indicate no expiration.
    /// </summary>
    public long ExpirationDurationInMilliseconds { get; set; } = -1;

    /// <summary>
    /// Gets or sets the loitering delay in milliseconds.
    /// If set, an alert will be sent after the user enters a geofence and stays inside during this period.
    /// If the user exits before this time, no alert will be sent.
    /// </summary>
    public int LoiteringDelayMilliseconds { get; set; }

    /// <summary>
    /// Sets the best-effort notification responsiveness of the geofence.
    /// Defaults to 0.
    /// Setting a big responsiveness value, for example 5 minutes, can save power significantly.
    /// However, setting a very small responsiveness value, for example 5 seconds,
    /// doesn't necessarily mean you will get notified right after the user enters a geofence:
    /// internally, the geofence might adjust the responsiveness value to save power when needed.
    /// (milliseconds) defines the best-effort description of how soon should the callback be called when the transition associated with the Geofence is triggered.
    /// For instance, if set to 300000 milliseconds the callback will be called 5 minutes within entering or exiting the geofence.
    /// </summary>
    public int ResponsivenessMilliseconds { get; set; }
}