namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidAction
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