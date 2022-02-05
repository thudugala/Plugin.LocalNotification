using System;

namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidScheduleOptionsBuilder
    {
        private readonly AndroidScheduleOptions _options;

        /// <summary>
        ///
        /// </summary>
        public AndroidScheduleOptionsBuilder()
        {
            _options = new AndroidScheduleOptions();
        }

        /// <summary>
        /// Builds the request to <see cref="AndroidOptions"/>
        /// </summary>
        /// <returns></returns>
        public AndroidScheduleOptions Build() => _options;

        /// <summary>
        ///
        /// </summary>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        public AndroidScheduleOptionsBuilder WithAlarmType(AndroidAlarmType alarmType)
        {
            _options.AlarmType = alarmType;
            return this;
        }

        /// <summary>
        /// In Android, do not Schedule or show notification if NotifyTime is earlier than (DateTime.Now - this time delay).
        /// Default is 1 min
        /// </summary>
        public AndroidScheduleOptionsBuilder WithAllowedDelay(TimeSpan allowedDelay)
        {
            _options.AllowedDelay = allowedDelay;
            return this;
        }
    }
}