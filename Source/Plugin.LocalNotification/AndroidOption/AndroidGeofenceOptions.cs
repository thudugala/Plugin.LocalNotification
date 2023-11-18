namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidGeofenceOptions
    {
        /// <summary>
        /// Sets the expiration duration of geofence.
        /// This geofence will be removed automatically after this period of time.
        /// Time for this proximity alert, in milliseconds, or -1 to indicate no expiration.
        /// When positive, this geofence will be removed automatically after this amount of time.
        /// </summary>
        public long ExpirationDurationInMilliseconds { get; set; } = -1;

        /// <summary>
        /// If loitering delay is set the geofence service will send a alert roughly after user enters a geofence if the user stays inside the geofence during this period of time.
        /// If the user exits from the geofence in this amount of time, alert won't be sent.
        /// The delay for confirming dwelling, in milliseconds
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
}