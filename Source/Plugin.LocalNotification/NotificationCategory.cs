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
        /// <param name="type"></param>
        public NotificationCategory(NotificationCategoryType type)
        {
            CategoryType = type;
        }

        /// <summary>
        ///
        /// </summary>
        public NotificationCategoryType CategoryType { get; }

        /// <summary>
        ///
        /// </summary>
        public IList<NotificationAction> ActionList { get; set; }
    }
}