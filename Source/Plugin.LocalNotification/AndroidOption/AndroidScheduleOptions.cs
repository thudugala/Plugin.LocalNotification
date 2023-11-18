namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// NotificationRequestSchedule for Android
    /// </summary>
    public class AndroidScheduleOptions
    {
        /// <summary>
        /// Default is RtcWakeup
        /// </summary>
        public AndroidAlarmType AlarmType { get; set; } = AndroidAlarmType.RtcWakeup;

        /// <summary>
        /// In Android, do not Schedule or show notification if NotifyTime is earlier than DateTime.Now and this time delay.
        /// Default is 1 min
        /// </summary>
        public TimeSpan AllowedDelay { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// internal use, only for Android
        /// </summary>
        /// <returns></returns>
        internal DateTime? GetNextNotifyTimeForRepeat(DateTime? notifyTime, NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
        {
            // NotifyTime does not change for Repeat request
            if (notifyTime is null)
            {
                return null;
            }

            var repeatInterval = GetNotifyRepeatInterval(repeatType, notifyRepeatInterval);
            if (repeatInterval is null)
            {
                return null;
            }

            var newNotifyTime = notifyTime.Value.Add(repeatInterval.Value);
            var nowTime = DateTime.Now.AddSeconds(10);
            while (newNotifyTime <= nowTime)
            {
                newNotifyTime = newNotifyTime.Add(repeatInterval.Value);
            }
            return newNotifyTime;
        }

        /// <summary>
        /// internal use, only for Android
        /// </summary>
        /// <returns></returns>
        internal TimeSpan? GetNotifyRepeatInterval(NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
        {
            TimeSpan? repeatInterval = null;
            switch (repeatType)
            {
                case NotificationRepeat.Daily:
                    // To be consistent with iOS, Schedule notification next day same time.
                    repeatInterval = TimeSpan.FromDays(1);
                    break;

                case NotificationRepeat.Weekly:
                    // To be consistent with iOS, Schedule notification next week same day same time.
                    repeatInterval = TimeSpan.FromDays(7);
                    break;

                case NotificationRepeat.TimeInterval:
                    if (notifyRepeatInterval.HasValue)
                    {
                        repeatInterval = notifyRepeatInterval.Value;
                    }
                    break;

                case NotificationRepeat.No:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return repeatInterval;
        }

        /// <summary>
        /// To be consistent with iOS, Do not show notification if NotifyTime is earlier than (DateTime.Now - AllowedDelay)
        /// </summary>
        /// <returns></returns>
        internal bool IsValidNotifyTime(DateTime timeNow, DateTime? notifyTime)
        {
            var (startTime, _) = GetNotifyTimeRange(timeNow);

            return startTime <= notifyTime;
        }

        internal bool IsValidShowLaterTime(DateTime timeNow, DateTime? notifyTime)
        {
            var (_, endTime) = GetNotifyTimeRange(timeNow);

            return notifyTime > endTime;
        }

        internal bool IsValidShowNowTime(DateTime timeNow, DateTime? notifyTime)
        {
            var (startTime, endTime) = GetNotifyTimeRange(timeNow);

            return startTime <= notifyTime && notifyTime <= endTime;
        }

        private (DateTime StartTime, DateTime EndTime) GetNotifyTimeRange(DateTime timeNow)
        {
            var startTime = timeNow.Subtract(AllowedDelay);
            var endTime = timeNow.AddMinutes(1);

            return (startTime, endTime);
        }
    }
}