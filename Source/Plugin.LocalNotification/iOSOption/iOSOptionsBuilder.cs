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

        /// <summary>
        /// The Priority determines the degree of Priority associated with the notification.
        /// Default is active
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public iOSOptionsBuilder WithPriority(iOSPriority priority)
        {
            _options.Priority = priority;
            return this;
        }

        /// <summary>
        /// The system uses the relevanceScore, a value between 0 and 1, to sort the notifications from your app. The highest score gets featured in the notification summary.
        /// </summary>
        /// <param name="relevanceScore"></param>
        /// <returns></returns>
        public iOSOptionsBuilder WithRelevanceScore(double relevanceScore)
        {
            _options.RelevanceScore = relevanceScore;
            return this;
        }

        /// <summary>
        /// The string the notification adds to the category’s summary format string.
        /// </summary>
        /// <param name="summaryArgument"></param>
        /// <returns></returns>
        public iOSOptionsBuilder WithSummaryArgument(string summaryArgument)
        {
            _options.SummaryArgument = summaryArgument;
            return this;
        }

        /// <summary>
        /// The number of items the notification adds to the category’s summary format string.
        /// </summary>
        /// <param name="summaryArgumentCount"></param>
        /// <returns></returns>
        public iOSOptionsBuilder WithSummaryArgumentCount(int summaryArgumentCount)
        {
            _options.SummaryArgumentCount = summaryArgumentCount;
            return this;
        }
    }
}