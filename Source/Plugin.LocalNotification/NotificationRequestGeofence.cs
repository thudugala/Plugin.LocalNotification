using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

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
        /// Android specific properties.
        /// </summary>
        public AndroidGeofenceOptions Android { get; set; } = new();

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public iOSGeofenceOptions iOS { get; set; } = new();

        /// <summary>
        /// The center of the geofence
        /// </summary>
        public Position Center { get; set; } = new();

        /// <summary>
        /// The radius of the region.
        /// Default 5m
        /// </summary>
        public double RadiusInMeters { get; set; } = 5;

        /// <summary>
        ///
        /// </summary>
        public bool IsGeofence => Center != null && Center.IsPositionSet;

        /// <summary>
        ///
        /// </summary>
        public class Position
        {
            /// <summary>
            /// Latitude in degrees, between -90 and +90 inclusive
            /// </summary>
            public double Latitude { get; set; } = double.NaN;

            /// <summary>
            /// Longitude in degrees, between -180 and +180 inclusive.
            /// </summary>
            public double Longitude { get; set; } = double.NaN;

            /// <summary>
            /// 
            /// </summary>
            public bool IsPositionSet => !double.IsNaN(Latitude) && !double.IsNaN(Longitude);
        }

        /// <summary>
        ///
        /// </summary>
        [Flags]
        public enum GeofenceNotifyOn
        {
            /// <summary>
            /// User enters the geofence
            /// </summary>
            OnEntry = 1,

            /// <summary>
            /// user exits the geofence
            /// </summary>
            OnExit = 2
        }
    }
}