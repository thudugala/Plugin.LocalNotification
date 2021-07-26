using System;
using Plugin.LocalNotification.iOSOption;

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
        ///
        /// </summary>
        public iOSActionType iOSAction { get; set; } = iOSActionType.None;

        /// <summary>
        ///
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NotificationAction other)
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