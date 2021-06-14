using System;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{

    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    public class NotificationCategory
    {
        public NotificationCategory(NotificationCategoryTypes type)
        {
            Type = type;
        }

        public NotificationCategoryTypes Type { get; }

        public NotificationAction[] NotificationActions { get; set; }
    }
}
