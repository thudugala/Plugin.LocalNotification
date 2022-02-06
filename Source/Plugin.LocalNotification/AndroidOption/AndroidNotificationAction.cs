namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidNotificationAction
    {
        /// <summary>
        ///
        /// </summary>
        public AndroidIcon IconName { get; set; } = new AndroidIcon();

        /// <summary>
        ///
        /// </summary>
        public AndroidPendingIntentFlags PendingIntentFlags { get; set; } = AndroidPendingIntentFlags.CancelCurrent;
    }
}