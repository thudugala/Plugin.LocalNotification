using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAndroidLocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannel(NotificationChannelRequest channelRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupChannelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannelGroup(NotificationChannelGroupRequest groupChannelRequest);
    }
}
