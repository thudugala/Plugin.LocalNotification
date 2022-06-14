using Plugin.LocalNotification.AndroidOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationRequestGeofence
    {
        /// <summary>
        /// 
        /// </summary>
        public GeofenceNotifyOn NotifyOn { get; set; } = GeofenceNotifyOn.OnEntry;

        /// <summary>
        /// 
        /// </summary>
        public AndroidGeofenceOptions Android { get; set; } = new();

        /// <summary>
        /// The center of the geofence
        /// </summary>
        public Position Center { get; set; }

        /// <summary>
        /// The radius of the region.
        /// Default 5m
        /// </summary>
        public double RadiusInMeters { get; set; } = 5;

        /// <summary>
        /// 
        /// </summary>
        public bool IsGeofence => Center != null;

        /// <summary>
        /// 
        /// </summary>
        public class Position
        {
            /// <summary>
            /// 
            /// </summary>
            public double Latitude { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double Longitude { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum GeofenceNotifyOn
        {
            /// <summary>
            /// 
            /// </summary>
            OnEntry,
            /// <summary>
            /// 
            /// </summary>
            OnExit
        }
    }
}
