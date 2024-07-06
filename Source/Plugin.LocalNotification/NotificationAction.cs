using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.WindowsOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// ActionId is the unique identifier for the Category
    /// </remarks>
    /// <param name="actionId">A unique identifier for the Action</param>
    public class NotificationAction(int actionId) : IEquatable<NotificationAction>
    {

        /// <summary>
        /// A unique identifier for the Action
        /// </summary>
        public int ActionId { get; } = actionId;

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public iOSAction IOS { get; set; } = new();

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
        public bool Equals(NotificationAction? other) => other != null &&
                   ActionId == other.ActionId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => Equals(obj as NotificationAction);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => ActionId.GetHashCode();
    }
}