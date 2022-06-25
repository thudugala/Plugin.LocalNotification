using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="android"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iOS"></param>
        /// <returns></returns>
        IiOSLocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS);
    }
}
