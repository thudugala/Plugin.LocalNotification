using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Return Notification Android Options Key.
        /// </summary>
        public static string ReturnRequestAndroid => "Plugin.LocalNotification.RETURN_REQUEST_ANDROID";

        /// <summary>
        /// Return Notification iOS Options Key.
        /// </summary>
        public static string ReturnRequestIos => "Plugin.LocalNotification.RETURN_REQUEST_IOS";

        /// <summary>
        ///  Return Notification Schedule Options Key.
        /// </summary>
        public static string ReturnRequestSchedule => "Plugin.LocalNotification.RETURN_REQUEST_SCHEDULE";

        internal static NotificationRequest GetRequest(string serializedRequest, string serializedRequestSchedule, string serializedRequestAndroid, string serializedRequestIos)
        {
            System.Diagnostics.Debug.WriteLine($"GetNotification");
            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request Schedule [{serializedRequestSchedule}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request Android [{serializedRequestAndroid}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request iOS [{serializedRequestIos}]");

            if (string.IsNullOrWhiteSpace(serializedRequest))
            {
                return null;
            }

            var request = ObjectSerializer.DeserializeObject<NotificationRequest>(serializedRequest);
            if (string.IsNullOrWhiteSpace(serializedRequestSchedule) == false)
            {
                var requestSchedule =
                    ObjectSerializer.DeserializeObject<NotificationRequestSchedule>(serializedRequestSchedule);
                if (requestSchedule != null)
                {
                    request.Schedule = requestSchedule;
                }
            }
            if (string.IsNullOrWhiteSpace(serializedRequestAndroid) == false)
            {
                var requestAndroid =
                    ObjectSerializer.DeserializeObject<AndroidOptions>(serializedRequestAndroid);
                if (requestAndroid != null)
                {
                    request.Android = requestAndroid;
                }
            }
            if (string.IsNullOrWhiteSpace(serializedRequestIos) == false)
            {
                var requestIos =
                    ObjectSerializer.DeserializeObject<iOSOptions>(serializedRequestIos);
                if (requestIos != null)
                {
                    request.iOS = requestIos;
                }
            }

            return request;
        }

        internal static Dictionary<string, string> GetRequestSerialize(NotificationRequest request)
        {
            var dictionary = new Dictionary<string, string>();

            var serializedRequest = ObjectSerializer.SerializeObject(request);

            // Why serialized options separately ?
            // System.Xml.Serialization.XmlSerializer Deserialize and Serialize methods ignore
            // object property "Android" when linking option set to "SDK Assemblies Only"
            var serializedRequestSchedule = ObjectSerializer.SerializeObject(request.Schedule);
            var serializedRequestAndroid = ObjectSerializer.SerializeObject(request.Android);
            var serializedRequestIos = ObjectSerializer.SerializeObject(request.Android);

            System.Diagnostics.Debug.WriteLine($"GetNotificationSerialize");
            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request Schedule [{serializedRequestSchedule}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request Android [{serializedRequestAndroid}]");
            System.Diagnostics.Debug.WriteLine($"Serialized Request iOS [{serializedRequestIos}]");

            dictionary.Add(ReturnRequest, serializedRequest);
            dictionary.Add(ReturnRequestSchedule, serializedRequestSchedule);
            dictionary.Add(ReturnRequestAndroid, serializedRequestAndroid);
            dictionary.Add(serializedRequestIos, serializedRequestIos);

            return dictionary;
        }
    }
}