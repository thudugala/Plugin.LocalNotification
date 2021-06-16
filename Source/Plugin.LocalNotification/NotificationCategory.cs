using System;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    public class NotificationCategory
    {
        public NotificationCategory(NotificationCategoryType type)
        {
            CategoryType = type;
        }

        public NotificationCategoryType CategoryType { get; }

        public NotificationAction[] NotificationActions { get; set; }
    }
}