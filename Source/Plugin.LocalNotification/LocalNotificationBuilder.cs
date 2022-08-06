using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.Json;
using System;
using System.Collections.Generic;

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
        public HashSet<NotificationCategory> CategorySet { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public INotificationSerializer Serializer { get; private set; }

        /// <summary>
        /// Android specific Builder.
        /// </summary>
        public AndroidLocalNotificationBuilder AndroidBuilder { get; private set; }

        /// <summary>
        /// Android specific Builder.
        /// </summary>
        public iOSLocalNotificationBuilder IOSBuilder { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public LocalNotificationBuilder()
        {
            AndroidBuilder = new AndroidLocalNotificationBuilder();
            IOSBuilder = new iOSLocalNotificationBuilder();
            CategorySet = new HashSet<NotificationCategory>();
            Serializer = new NotificationSerializer();
        }

        /// <inheritdoc/>
        public IAndroidLocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android)
        {
            android?.Invoke(AndroidBuilder);
            return AndroidBuilder;
        }

        /// <inheritdoc/>
        public IiOSLocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS)
        {
            iOS?.Invoke(IOSBuilder);
            return IOSBuilder;
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
