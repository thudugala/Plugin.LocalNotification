using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification
{
    public class LocalNotificationTappedEvent
    {
        /// <summary>
        /// Returning data when click on notification.
        /// </summary>
        public IList<string> Data { get; set; }
    }
}
