using System;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{

    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    public class NotificationCategory
    {
        public NotificationCategory(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        public NotificationAction[] NotificationActions { get; set; }
    }
}
