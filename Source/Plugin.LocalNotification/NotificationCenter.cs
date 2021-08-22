using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public static partial class NotificationCenter
    {
        private static INotificationService _current;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get =>
                _current ?? throw new InvalidOperationException(Properties.Resources.PluginNotFound);
            set => _current = value;
        }

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ReturnRequest => "Plugin.LocalNotification.RETURN_REQUEST";

        internal static NotificationRequest GetRequest(string serializedRequest)
        {
            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");
            if (string.IsNullOrWhiteSpace(serializedRequest))
            {
                return null;
            }

            var request = JsonSerializer.Deserialize<NotificationRequest>(serializedRequest);
            return request;
        }

        internal static Dictionary<string, string> GetRequestSerialize(NotificationRequest request)
        {
            var dictionary = new Dictionary<string, string>();

            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = null;
            }
            var serializedRequest = JsonSerializer.Serialize(request);

            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");

            dictionary.Add(ReturnRequest, serializedRequest);

            return dictionary;
        }
    }
}