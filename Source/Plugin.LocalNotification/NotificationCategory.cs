namespace Plugin.LocalNotification
{
    /// <summary>
    /// Categories serve as the container for actions
    /// </summary>
    /// <remarks>
    /// CategoryType is the unique identifier for the Category
    /// </remarks>
    /// <param name="categoryType">A unique identifier for the Category</param>
    public class NotificationCategory(NotificationCategoryType categoryType) : IEquatable<NotificationCategory>
    {
        /// <summary>
        ///
        /// </summary>
        public HashSet<NotificationAction> ActionList { get; set; } = [];

        /// <summary>
        /// A unique identifier for the Category
        /// </summary>
        public NotificationCategoryType CategoryType { get; } = categoryType;

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NotificationCategory? other)
        {
            return other != null &&
                   CategoryType == other.CategoryType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as NotificationCategory);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return CategoryType.GetHashCode();
        }
    }
}