﻿using CoreLocation;
using Foundation;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.iOSOption;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

/// <inheritdoc />
internal class NotificationServiceImpl : INotificationService
{
    /// <inheritdoc />
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    /// <inheritdoc />
    public bool IsSupported => true;

    /// <inheritdoc />
    public event NotificationReceivedEventHandler? NotificationReceived;

    /// <inheritdoc />
    public event NotificationActionTappedEventHandler? NotificationActionTapped;

    /// <inheritdoc />
    public event NotificationDisabledEventHandler? NotificationsDisabled;

    /// <inheritdoc />
    public void OnNotificationReceived(NotificationEventArgs e) => NotificationReceived?.Invoke(e);

    /// <inheritdoc />
    public void OnNotificationActionTapped(NotificationActionEventArgs e) => NotificationActionTapped?.Invoke(e);

    /// <inheritdoc />
    public void OnNotificationsDisabled() => NotificationsDisabled?.Invoke();

    /// <inheritdoc />
    public bool Cancel(params int[] notificationIdList)
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(11))
        {
            return false;
        }

        var itemList = notificationIdList.Select((item) => item.ToString()).ToArray();

        UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
        UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);

        return true;
    }

    /// <inheritdoc />
    public bool CancelAll()
    {
        UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        return true;
    }

    /// <inheritdoc />
    public bool Clear(params int[] notificationIdList)
    {
        var itemList = notificationIdList.Select((item) => item.ToString()).ToArray();

        UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
        return true;
    }

    /// <inheritdoc />
    public bool ClearAll()
    {
        UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> Show(NotificationRequest request)
    {
        UNNotificationTrigger? trigger = null;
        try
        {
            if (!OperatingSystem.IsOSPlatform("IOS"))
            {
                return false;
            }

            if (request is null)
            {
                return false;
            }

            var allowed = await AreNotificationsEnabled().ConfigureAwait(false);
            if (allowed == false)
            {
                LocalNotificationCenter.Log("User denied permission");
                OnNotificationsDisabled();
                return false;
            }

            using var content = await GetNotificationContent(request);

            var notificationId =
                request.NotificationId.ToString(CultureInfo.CurrentCulture);

            if (request.Geofence.IsGeofence)
            {
                var center = new CLLocationCoordinate2D(request.Geofence.Center.Latitude, request.Geofence.Center.Longitude);
                                    
                var regin = new CLCircularRegion(center,
                                request.Geofence.RadiusInMeters,
                                notificationId)
                {
                    NotifyOnEntry = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnEntry) == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry,
                    NotifyOnExit = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnExit) == NotificationRequestGeofence.GeofenceNotifyOn.OnExit,
                };

                trigger = UNLocationNotificationTrigger.CreateTrigger(regin, request.Geofence.IOS.Repeats);                    
            }
            else
            {
                var repeats = request.Schedule.RepeatType != NotificationRepeat.No;

                if (repeats && request.Schedule.RepeatType == NotificationRepeat.TimeInterval &&
                    request.Schedule.NotifyRepeatInterval.HasValue)
                {
                    var interval = request.Schedule.NotifyRepeatInterval.Value;

                    // Cannot delay and repeat in when TimeInterval
                    trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
                }
                else
                {
                    using var notifyTime = GetNsDateComponentsFromDateTime(request);
                    trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, repeats);
                }
            }

            var nativeRequest = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(nativeRequest)
                .ConfigureAwait(false);

            return true;
        }
        finally
        {
            trigger?.Dispose();
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<UNMutableNotificationContent> GetNotificationContent(NotificationRequest request)
    {
        if (!OperatingSystem.IsOSPlatform("IOS"))
        {
            return new UNMutableNotificationContent();
        }

        var userInfoDictionary = new NSMutableDictionary();
        var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
        userInfoDictionary.SetValueForKey(new NSString(serializedRequest), new NSString(LocalNotificationCenter.ReturnRequest));

        var content = new UNMutableNotificationContent
        {
            Title = request.Title,
            Subtitle = request.Subtitle,
            Body = request.Description,
            Badge = request.BadgeNumber,
            UserInfo = userInfoDictionary
        };

        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            content.InterruptionLevel = request.iOS.Priority.ToNative();
            content.RelevanceScore = request.iOS.RelevanceScore;
        }

        // Image Attachment
        if (request.Image != null)
        {
            var nativeImage = await GetNativeImage(request.Image);
            if (nativeImage != null)
            {
                content.Attachments = [nativeImage];
            }
        }

        if (request.CategoryType != NotificationCategoryType.None)
        {
            content.CategoryIdentifier = request.CategoryType.ToNative();
        }

        if (string.IsNullOrWhiteSpace(request.Group) == false)
        {
            content.ThreadIdentifier = request.Group;
        }

        if (string.IsNullOrWhiteSpace(request.iOS.SummaryArgument) == false)
        {                
            if (OperatingSystem.IsOSPlatform("IOS") &&
                OperatingSystem.IsIOSVersionAtLeast(12) &&
                !OperatingSystem.IsIOSVersionAtLeast(15))
            {
                content.SummaryArgument = request.iOS.SummaryArgument;
                content.SummaryArgumentCount = (nuint)request.iOS.SummaryArgumentCount;
            }                
        }

        content.Sound = request.Silent ?
            null :
            string.IsNullOrWhiteSpace(request.Sound) == false ?
                UNNotificationSound.GetSound(request.Sound) :
                UNNotificationSound.Default;

        return content;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationImage"></param>
    /// <returns></returns>
    protected virtual async Task<UNNotificationAttachment?> GetNativeImage(NotificationImage? notificationImage)
    {
        if (notificationImage is null || notificationImage.HasValue == false)
        {
            return null;
        }

        NSUrl? imageAttachment = null;
        if (string.IsNullOrWhiteSpace(notificationImage.ResourceName) == false)
        {
            imageAttachment = NSBundle.MainBundle.GetUrlForResource(
                Path.GetFileNameWithoutExtension(notificationImage.ResourceName),
                Path.GetExtension(notificationImage.ResourceName));
        }

        if (string.IsNullOrWhiteSpace(notificationImage.FilePath) == false)
        {
            if (File.Exists(notificationImage.FilePath))
            {
                imageAttachment = NSUrl.CreateFileUrl(notificationImage.FilePath, false, null);
            }
        }

        if (notificationImage.Binary is { Length: > 0 })
        {
            var cache = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,
                NSSearchPathDomain.User);
            var cachesFolder = cache[0];
            var cacheFile = $"{cachesFolder}{NSProcessInfo.ProcessInfo.GloballyUniqueString}";

            if (File.Exists(cacheFile))
            {
                File.Delete(cacheFile);
            }

            await File.WriteAllBytesAsync(cacheFile, notificationImage.Binary);

            imageAttachment = NSUrl.CreateFileUrl(cacheFile, false, null);
        }

        if (imageAttachment is null)
        {
            return null;
        }

        var options = new UNNotificationAttachmentOptions();

        return UNNotificationAttachment.FromIdentifier("image", imageAttachment, options, out _);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationRequest"></param>
    /// <returns></returns>
    protected static NSDateComponents GetNsDateComponentsFromDateTime(NotificationRequest notificationRequest)
    {
        var dateTime = notificationRequest.Schedule.NotifyTime ?? DateTime.Now.AddSeconds(1);

        return notificationRequest.Schedule.RepeatType switch
        {
            NotificationRepeat.Daily => new NSDateComponents
            {
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            },
            NotificationRepeat.Weekly => new NSDateComponents
            {
                // iOS: Weekday units are the numbers 1 through n, where n is the number of days in the week.
                // For example, in the Gregorian calendar, n is 7 and Sunday is represented by 1.
                // .Net: The returned value is an integer between 0 and 6,
                // where 0 indicates Sunday, 1 indicates Monday, 2 indicates Tuesday, 3 indicates Wednesday, 4 indicates Thursday, 5 indicates Friday, and 6 indicates Saturday.
                Weekday = (int)dateTime.DayOfWeek + 1,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            },
            NotificationRepeat.No => new NSDateComponents
            {
                Day = dateTime.Day,
                Month = dateTime.Month,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            },
            _ => new NSDateComponents
            {
                Day = dateTime.Day,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            }
        };
    }

    /// <inheritdoc />
    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
    {
        if (categoryList is null || categoryList.Count <= 0)
        {
            return;
        }

        var nativeCategoryList = new List<UNNotificationCategory>();
        foreach (var category in categoryList)
        {
            if (category.CategoryType == NotificationCategoryType.None)
            {
                continue;
            }

            var nativeCategory = RegisterActionList(category);
            if (nativeCategory != null)
            {
                nativeCategoryList.Add(nativeCategory);
            }
        }

        if (nativeCategoryList.Count <= 0)
        {
            return;
        }

        UNUserNotificationCenter.Current.SetNotificationCategories(
            new NSSet<UNNotificationCategory>(nativeCategoryList.ToArray()));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    protected static UNNotificationCategory? RegisterActionList(NotificationCategory? category)
    {
        if (category is null || category.CategoryType == NotificationCategoryType.None)
        {
            return null;
        }

        var nativeActionList = new List<UNNotificationAction>();
        foreach (var notificationAction in category.ActionList)
        {
            if (notificationAction.ActionId == -1000)
            {
                continue;
            }

            if (OperatingSystem.IsIOSVersionAtLeast(15))
            {
                var icon = notificationAction.IOS.Icon.Type switch
                {
                    iOSActionIconType.None => null,
                    iOSActionIconType.System => UNNotificationActionIcon.CreateFromSystem(notificationAction.IOS.Icon.Name),
                    iOSActionIconType.Template => UNNotificationActionIcon.CreateFromTemplate(notificationAction.IOS.Icon.Name),
                    _ => null,
                };

                var nativeAction = UNNotificationAction.FromIdentifier(
                notificationAction.ActionId.ToString(CultureInfo.InvariantCulture),
                notificationAction.Title,
                notificationAction.IOS.Action.ToNative(),
                icon);

                nativeActionList.Add(nativeAction);
            }
            else
            {
                var nativeAction = UNNotificationAction.FromIdentifier(
                    notificationAction.ActionId.ToString(CultureInfo.InvariantCulture),
                    notificationAction.Title,
                    notificationAction.IOS.Action.ToNative());

                nativeActionList.Add(nativeAction);
            }
        }

        if (nativeActionList.Count > 0)
        {
            return null;
        }

        var notificationCategory = UNNotificationCategory
            .FromIdentifier(category.CategoryType.ToNative(), [.. nativeActionList],
                [], UNNotificationCategoryOptions.CustomDismissAction);

        return notificationCategory;
    }

    /// <inheritdoc />
    public async Task<IList<NotificationRequest>> GetPendingNotificationList()
    {
        var pending = await UNUserNotificationCenter.Current.GetPendingNotificationRequestsAsync();

        return [.. pending.Select(r => LocalNotificationCenter.GetRequest(r.Content) ?? new NotificationRequest())];
    }

    /// <inheritdoc />
    public async Task<IList<NotificationRequest>> GetDeliveredNotificationList()
    {
        var delivered = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();

        return [.. delivered.Select(r => LocalNotificationCenter.GetRequest(r.Request.Content) ?? new NotificationRequest())];
    }

    /// <inheritdoc />
    public async Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null)
    {
        var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
        return settings.AlertSetting == UNNotificationSetting.Enabled;
    }

    /// <inheritdoc />
    public async Task<bool> RequestNotificationPermission(NotificationPermission? permission = null)
    {
        try
        {
            permission ??= new NotificationPermission();

            if (!permission.AskPermission)
            {
                return false;
            }

            var allowed = await AreNotificationsEnabled(permission);
            if (allowed)
            {
                return true;
            }

            // Ask the user for permission to show notifications on iOS 10.0+
            var authorizationOptions = permission.IOS.NotificationAuthorization.ToNative();
            var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(authorizationOptions).ConfigureAwait(false);

            if (error != null)
            {
                LocalNotificationCenter.Log(error.LocalizedDescription);
            }

            if (alertsAllowed)
            {
                if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.No)
                {
                    return false;
                }

                var locationManager = new CLLocationManager();

                if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.Always)
                {
                    locationManager.RequestAlwaysAuthorization();
                }
                else if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.WhenInUse)
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }

            return alertsAllowed;
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
            return false;
        }
    }
}