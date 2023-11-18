using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.WindowsOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationAction : IEquatable<NotificationAction>
    {
        /// <summary>
        /// ActionId is the unique identifier for the Category
        /// </summary>
        /// <param name="actionId">A unique identifier for the Action</param>
        public NotificationAction(int actionId)
        {
            ActionId = actionId;
        }

        /// <summary>
        /// A unique identifier for the Action
        /// </summary>
        public int ActionId { get; }

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public iOSAction iOS { get; set; } = new();

        /// <summary>
        /// Android specific properties.
        /// </summary>
        public AndroidAction Android { get; set; } = new();

        /// <summary>
        /// Windows specific properties.
        /// </summary>
        public WindowsAction Windows { get; set; } = new();

        /// <summary>
        ///
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NotificationAction? other)
        {
            return other != null &&
                   ActionId == other.ActionId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ActionId.GetHashCode();
        }
    }
}