using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System;

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
        /// 
        /// </summary>
        public iOSGeofenceOptions iOS { get; set; } = new();

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
            /// Latitude in degrees, between -90 and +90 inclusive
            /// </summary>
            public double Latitude { get; set; }

            /// <summary>
            /// Longitude in degrees, between -180 and +180 inclusive.
            /// </summary>
            public double Longitude { get; set; }
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
