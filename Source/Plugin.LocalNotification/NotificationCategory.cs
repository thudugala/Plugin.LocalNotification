using System;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    public class NotificationCategory : IEquatable<NotificationCategory>
    {
        /// <summary>
        /// CategoryType is the unique identifier for the Category
        /// </summary>
        /// <param name="categoryType">A unique identifier for the Category</param>
        public NotificationCategory(NotificationCategoryType categoryType)
        {
            CategoryType = categoryType;
        }

        /// <summary>
        ///
        /// </summary>
        public HashSet<NotificationAction> ActionList { get; set; }

        /// <summary>
        /// A unique identifier for the Category
        /// </summary>
        public NotificationCategoryType CategoryType { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NotificationCategory other)
        {
            return other != null &&
                   CategoryType == other.CategoryType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(CategoryType);
        }
    }
}