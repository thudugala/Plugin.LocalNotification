using Plugin.LocalNotification.Core.Models;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// File-based repository for tracking pending and delivered notifications on Windows.
/// </summary>
internal static class NotificationRepository
{
    private static readonly object Locker = new();
    private static readonly string StorageDirectory;

    private const string PendingListFileName = "pending_notifications.json";
    private const string DeliveredListFileName = "delivered_notifications.json";

    static NotificationRepository()
    {
        StorageDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Plugin.LocalNotification");
        Directory.CreateDirectory(StorageDirectory);
    }

    internal static void RemoveByPendingIdList(params int[] notificationIdList)
    {
        var itemList = GetPendingList();
        _ = itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
        SetPendingList(itemList);
    }

    internal static void AddPendingRequest(NotificationRequest request)
    {
        var itemList = GetPendingList();
        _ = itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
        itemList.Add(request);
        SetPendingList(itemList);
    }

    internal static void AddDeliveredRequest(NotificationRequest request)
    {
        var itemList = GetDeliveredList();
        _ = itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
        itemList.Add(request);
        SetDeliveredList(itemList);
    }

    internal static void RemoveDeliveredList() => SetDeliveredList(null);

    internal static void RemovePendingList() => SetPendingList(null);

    internal static void RemoveByDeliveredIdList(params int[] notificationIdList)
    {
        var itemList = GetDeliveredList();
        _ = itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
        SetDeliveredList(itemList);
    }

    internal static List<NotificationRequest> GetPendingList() => GetList(PendingListFileName);

    internal static List<NotificationRequest> GetDeliveredList() => GetList(DeliveredListFileName);

    private static void SetPendingList(List<NotificationRequest>? list) => SetList(PendingListFileName, list);

    private static void SetDeliveredList(List<NotificationRequest>? list) => SetList(DeliveredListFileName, list);

    private static List<NotificationRequest> GetList(string fileName)
    {
        lock (Locker)
        {
            var filePath = Path.Combine(StorageDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return [];
            }

            var jsonText = File.ReadAllText(filePath);
            return string.IsNullOrWhiteSpace(jsonText)
                ? []
                : LocalNotificationCenter.GetRequestList(jsonText);
        }
    }

    private static void SetList(string fileName, List<NotificationRequest>? list)
    {
        lock (Locker)
        {
            var filePath = Path.Combine(StorageDirectory, fileName);
            if (list is null || list.Count == 0)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return;
            }

            var jsonText = LocalNotificationCenter.GetRequestListSerialize(list);
            File.WriteAllText(filePath, jsonText);
        }
    }
}
