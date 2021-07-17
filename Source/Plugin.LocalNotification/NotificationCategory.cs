using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    public class NotificationCategory
    {        
        /// <summary>
        ///
        /// </summary>
        public NotificationCategoryType CategoryType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IList<NotificationAction> ActionList { get; set; }
    }
}