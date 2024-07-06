using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.Json;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class LocalNotificationBuilder : ILocalNotificationBuilder
    {
        /// <summary>
        /// Register notification categories and their corresponding actions
        /// </summary>
        internal HashSet<NotificationCategory> CategorySet { get; } = [];

        /// <summary>
        ///
        /// </summary>
        internal INotificationSerializer Serializer { get; private set; } = new NotificationSerializer();

        /// <summary>
        /// Android specific Builder.
        /// </summary>
        internal AndroidLocalNotificationBuilder AndroidBuilder { get; } = new();

        /// <summary>
        /// Android specific Builder.
        /// </summary>
        internal iOSLocalNotificationBuilder IOSBuilder { get; } = new();
                
        /// <inheritdoc/>
        public ILocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android)
        {
            android?.Invoke(AndroidBuilder);
            return this;
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS)
        {
            iOS?.Invoke(IOSBuilder);
            return this;
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder AddCategory(NotificationCategory category)
        {
            CategorySet.Add(category);
            return this;
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder SetSerializer(INotificationSerializer serializer)
        {
            Serializer = serializer ?? new NotificationSerializer();
            return this;
        }
    }
}