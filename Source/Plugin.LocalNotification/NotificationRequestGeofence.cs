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
        public bool Repeat { get; set; }

        /// <summary>
        /// The center of the geofence
        /// </summary>
        public Position Center { get; set; }

        /// <summary>
        /// The radius of the region.
        /// </summary>
        public Distance Radius { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsGeofence => Center != null && Radius != null;

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
        public class Distance
        {
            /// <summary>
            /// 
            /// </summary>
            public double TotalMeters { get; set; }
        }
    }
}
