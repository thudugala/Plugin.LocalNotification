namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    /// NotificationRequest for iOS
    /// </summary>
    public class iOSOptionsBuilder
    {
        private readonly iOSOptions _options;

        /// <summary>
        ///
        /// </summary>
        internal iOSOptionsBuilder()
        {
            _options = new iOSOptions();
        }

        /// <summary>
        /// Builds the request to <see cref="iOSOptions"/>
        /// </summary>
        /// <returns></returns>
        public iOSOptions Build() => _options;

        /// <summary>
        /// Setting this flag will prevent iOS from displaying the default banner when a Notification is received in foreground
        /// Default is false
        /// </summary>
        public iOSOptionsBuilder ShouldHideForegroundAlert(bool hideForegroundAlert)
        {
            _options.HideForegroundAlert = hideForegroundAlert;
            return this;
        }

        /// <summary>
        /// Setting this flag will enable iOS to play the default notification sound even if the app is in foreground
        /// Default is false
        /// </summary>
        public iOSOptionsBuilder ShouldPlayForegroundSound(bool playForegroundSound)
        {
            _options.PlayForegroundSound = playForegroundSound;
            return this;
        }
    }
}