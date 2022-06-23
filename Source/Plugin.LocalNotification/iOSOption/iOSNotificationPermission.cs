namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    /// 
    /// </summary>
    public class iOSNotificationPermission
    {
        /// <summary>
        /// 
        /// </summary>
        public bool SetUserNotificationCenterDelegate { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool AskPermission { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public iOSAuthorizationOptions NotificationAuthorization { get; set; } = iOSAuthorizationOptions.Alert |
            iOSAuthorizationOptions.Badge |
            iOSAuthorizationOptions.Sound;

        /// <summary>
        /// 
        /// </summary>
        public iOSLocationAuthorization LocationAuthorization { get; set; } = iOSLocationAuthorization.No;
    }
}
