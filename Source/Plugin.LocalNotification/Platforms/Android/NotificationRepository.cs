using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.LocalNotification.Platforms
{
    /// <summary>
    ///
    /// </summary>
    internal class NotificationRepository
    {
        private static readonly Lazy<NotificationRepository> MySingleton =
            new(() => new NotificationRepository(),
                System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static readonly object Locker = new();

        /// <summary>
        ///
        /// </summary>
        internal static NotificationRepository Current => MySingleton.Value;

        /// <summary>
        ///
        /// </summary>
        private const string PendingListKey = "PendingList";

        /// <summary>
        ///
        /// </summary>
        private const string DeliveredListKey = "DeliveredList";

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static ISharedPreferences GetSharedPreferences()
        {
            const string sharedName = "plugin.LocalNotification." + nameof(NotificationRepository);
            return Application.Context.GetSharedPreferences(sharedName, FileCreationMode.Private);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationIdList"></param>
        internal void RemoveByPendingIdList(params int[] notificationIdList)
        {
            var itemList = GetPendingList();
            itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
            SetPendingList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal void AddPendingRequest(NotificationRequest request)
        {
            var itemList = GetPendingList();
            itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
            itemList.RemoveAll(r =>
                r.Schedule.NotifyTime.HasValue &&
                r.Schedule.Android.IsValidNotifyTime(DateTime.Now, r.Schedule.NotifyTime) == false);
            itemList.Add(request);
            SetPendingList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal void AddDeliveredRequest(NotificationRequest request)
        {
            var itemList = GetDeliveredList();
            itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
            itemList.Add(request);
            SetDeliveredList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        internal void RemoveDeliveredList()
        {
            SetDeliveredList(null);
        }

        /// <summary>
        ///
        /// </summary>
        internal void RemovePendingList()
        {
            SetPendingList(null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationIdList"></param>
        internal void RemoveByDeliveredIdList(params int[] notificationIdList)
        {
            var itemList = GetDeliveredList();
            itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
            SetDeliveredList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal List<NotificationRequest> GetPendingList()
        {
            var itemList = GetList(PendingListKey);
            return itemList;
        }

        private static void SetPendingList(List<NotificationRequest> list)
        {
            SetList(PendingListKey, list);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal List<NotificationRequest> GetDeliveredList()
        {
            var itemList = GetList(DeliveredListKey);
            return itemList;
        }

        private static void SetDeliveredList(List<NotificationRequest> list)
        {
            SetList(DeliveredListKey, list);
        }

        private static List<NotificationRequest> GetList(string key)
        {
            lock (Locker)
            {
                using var sharedPreferences = GetSharedPreferences();
                var jsonText = sharedPreferences.GetString(key, string.Empty);
                return string.IsNullOrWhiteSpace(jsonText)
                    ? new List<NotificationRequest>()
                    : LocalNotificationCenter.GetRequestList(jsonText);
            }
        }

        private static void SetList(string key, List<NotificationRequest> list)
        {
            lock (Locker)
            {
                using var sharedPreferences = GetSharedPreferences();
                using var editor = sharedPreferences?.Edit();
                var jsonText = string.Empty;
                if (list != null && list.Any())
                {
                    jsonText = LocalNotificationCenter.GetRequestListSerialize(list);
                }
                editor?.PutString(key, jsonText);
                editor?.Apply();
            }
        }
    }
}