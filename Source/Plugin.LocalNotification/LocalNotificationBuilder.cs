using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalNotificationBuilder : ILocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        public AndroidLocalNotificationBuilder AndroidBuilder { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public iOSLocalNotificationBuilder IOSBuilder { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public LocalNotificationBuilder()
        {
            AndroidBuilder = new AndroidLocalNotificationBuilder();
            
        }

        /// <inheritdoc/>
        public IAndroidLocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android)
        {
            android?.Invoke(AndroidBuilder);
            return AndroidBuilder;
        }

        /// <inheritdoc/>
        public IiOSLocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS)
        {
            iOS?.Invoke(IOSBuilder);
            return IOSBuilder;
        }
    }
}
