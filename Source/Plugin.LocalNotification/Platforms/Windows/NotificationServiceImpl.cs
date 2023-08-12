using Microsoft.Toolkit.Uwp.Notifications;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Plugin.LocalNotification.Platforms
{
    internal class NotificationServiceImpl : INotificationService
    {
        private readonly IList<NotificationCategory> _categoryList = new List<NotificationCategory>();

        public Func<NotificationRequest, Task<NotificationEventReceivingArgs>> NotificationReceiving { get; set; }

        public event NotificationActionTappedEventHandler NotificationActionTapped;
        public event NotificationReceivedEventHandler NotificationReceived;
        public event NotificationDisabledEventHandler NotificationsDisabled;

        public Task<bool> AreNotificationsEnabled() => throw new NotImplementedException();

        public bool Cancel(params int[] notificationIdList) => throw new NotImplementedException();

        public bool CancelAll() => throw new NotImplementedException();

        public bool Clear(params int[] notificationIdList) => throw new NotImplementedException();

        public bool ClearAll() => throw new NotImplementedException();

        public Task<IList<NotificationRequest>> GetDeliveredNotificationList() => throw new NotImplementedException();

        public Task<IList<NotificationRequest>> GetPendingNotificationList() => throw new NotImplementedException();

        public void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            NotificationActionTapped?.Invoke(e);
        }

        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        public void OnNotificationsDisabled()
        {
            NotificationsDisabled?.Invoke();
        }

        public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
        {
            if (categoryList is null || categoryList.Any() == false)
            {
                return;
            }

            foreach (var category in categoryList.Where(category =>
                         category.CategoryType != NotificationCategoryType.None))
            {
                _categoryList.Add(category);
            }
        }

        public Task<bool> RequestNotificationPermission(NotificationPermission permission = null)
        {
            return LocalNotificationCenter.RequestNotificationPermissionAsync(permission);
        }

        public Task<bool> Show(NotificationRequest request)
        {
            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);

            var builder = new ToastContentBuilder()
                .AddArgument(LocalNotificationCenter.ReturnRequestActionId, NotificationActionEventArgs.TapActionId)
                .AddArgument(LocalNotificationCenter.ReturnRequest, serializedRequest)
                .AddHeader(nameof(request.NotificationId), request.Group, string.Empty)
                .AddText(request.Title)
                .AddText(request.Subtitle)
                .AddText(request.Description);

            if (request.Schedule != null && request.Schedule.NotifyTime != null)
            {
                builder.Schedule(new DateTimeOffset(request.Schedule.NotifyTime.Value));
            };

            if (_categoryList.Any())
            {
                var categoryByType = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
                if (categoryByType != null)
                {
                    foreach (var notificationAction in categoryByType.ActionList)
                    {
                        builder.AddButton(new ToastButton()
                            .SetContent(notificationAction.Title)
                            .AddArgument(LocalNotificationCenter.ReturnRequestActionId, notificationAction.ActionId)                         
                            .AddArgument(LocalNotificationCenter.ReturnRequest, serializedRequest));                            
                    }
                }
            }

            builder.Show(toast =>
            {
                toast.Activated += (sender, args) =>
                {
                    var toastArgs = args as ToastActivatedEventArgs;
                    LocalNotificationCenter.NotifyNotificationTapped(toastArgs.Arguments);
                };
                toast.Dismissed += (sender, args) =>
                {
                    var arguments = $"{LocalNotificationCenter.ReturnRequestActionId}={NotificationActionEventArgs.DismissedActionId};";
                    arguments += $"{LocalNotificationCenter.ReturnRequest}={serializedRequest}";
                   
                    LocalNotificationCenter.NotifyNotificationTapped(arguments);
                };
            });

            return Task.FromResult(true);
        }
    }
}
