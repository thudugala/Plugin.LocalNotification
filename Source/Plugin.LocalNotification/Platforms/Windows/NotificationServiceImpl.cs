using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Plugin.LocalNotification.Platforms
{
    internal class NotificationServiceImpl : INotificationService
    {
        private readonly IList<NotificationCategory> _categoryList = new List<NotificationCategory>();
        private readonly ToastNotifierCompat _notifier;

        public Func<NotificationRequest, Task<NotificationEventReceivingArgs>> NotificationReceiving { get; set; }

        public event NotificationActionTappedEventHandler NotificationActionTapped;
        public event NotificationReceivedEventHandler NotificationReceived;
        public event NotificationDisabledEventHandler NotificationsDisabled;

        public NotificationServiceImpl()
        {
            _notifier = ToastNotificationManagerCompat.CreateToastNotifier();
        }

        public Task<bool> AreNotificationsEnabled()
        {
            return _notifier.Setting == NotificationSetting.Enabled ? Task.FromResult(true) : Task.FromResult(false);
        }

        public bool Cancel(params int[] notificationIdList)
        {
            var scheduledToasts = _notifier.GetScheduledToastNotifications();

            var notificationIdSet = new HashSet<string>(Array.ConvertAll(notificationIdList, x => x.ToString()));
            var toRemove = scheduledToasts.FirstOrDefault(i => notificationIdSet.Contains(i.Tag));
            if (toRemove != null)
            {
                // And remove it from the schedule
                _notifier.RemoveFromSchedule(toRemove);
                return true;
            }
            return false;
        }

        public bool CancelAll()
        {
            var scheduledToasts = _notifier.GetScheduledToastNotifications();
            foreach (var notification in scheduledToasts)
            {
                _notifier.RemoveFromSchedule(notification);
            }
            return true;
        }

        public bool Clear(params int[] notificationIdList)
        {
            foreach (var notificationId in notificationIdList)
            {
                ToastNotificationManager.History.Remove(notificationId.ToString());
            }
            return true;
        }

        public bool ClearAll()
        {
            ToastNotificationManager.History.Clear();
            return true;
        }

        public Task<IList<NotificationRequest>> GetDeliveredNotificationList()
        {
            var deliveredNotifications = new List<NotificationRequest>();

            var toastNotifications = ToastNotificationManager.History.GetHistory();

            if (toastNotifications is null || !toastNotifications.Any())
            {
                return Task.FromResult<IList<NotificationRequest>>(deliveredNotifications);
            }
                       
            foreach (var toastNotification in toastNotifications)
            {
                var element = toastNotification.Content.ChildNodes.FirstOrDefault(e => e.NodeName == "toast");
                var attribute = element.Attributes.FirstOrDefault(a => a.NodeName == "launch");

                var (_, request) = LocalNotificationCenter.GetRequestFromArguments(attribute.InnerText);
                deliveredNotifications.Add(request);
            }
            return Task.FromResult<IList<NotificationRequest>>(deliveredNotifications);
        }

        public Task<IList<NotificationRequest>> GetPendingNotificationList()
        {
            var pendingNotifications = new List<NotificationRequest>();

            var scheduledToasts = _notifier.GetScheduledToastNotifications();

            if (scheduledToasts is null || !scheduledToasts.Any())
            {
                return Task.FromResult<IList<NotificationRequest>>(pendingNotifications);
            }
                        
            foreach (var scheduledToast in scheduledToasts)
            {
                var element = scheduledToast.Content.ChildNodes.FirstOrDefault(e => e.NodeName == "toast");
                var attribute = element.Attributes.FirstOrDefault(a => a.NodeName == "launch");

                var (_, request) = LocalNotificationCenter.GetRequestFromArguments(attribute.InnerText);
                                   
                pendingNotifications.Add(request);
            }
            return Task.FromResult<IList<NotificationRequest>>(pendingNotifications);
        }
                
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

        public async Task<bool> Show(NotificationRequest request)
        {
            var ff = new AppNotificationBuilder();
            var an = ff.BuildNotification();
            
            AppNotificationManager.Default.NotificationInvoked += (sender, args) =>
            {
               
            };

            AppNotificationManager.Default.Show(an);

            var builder = new ToastContentBuilder()
                .AddArgument(LocalNotificationCenter.ReturnRequest, request.NotificationId)
                .AddArgument(LocalNotificationCenter.ReturnRequestActionId, NotificationActionEventArgs.TapActionId)
                .AddText(request.Title)
                .AddText(request.Subtitle)
                .AddText(request.Description);

            if (!request.Windows.LaunchAppWhenTapped)
            {
                builder.SetBackgroundActivation();
            }

            if (request.Silent)
            {
                builder.AddAudio(new ToastAudio
                {
                    Silent = true,
                });
            }
            else if(!string.IsNullOrWhiteSpace(request.Sound))
            {
                builder.AddAudio(new ToastAudio
                {
                    Src = new Uri(request.Sound),
                    Silent = false,
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Group))
            {
                builder.AddHeader(nameof(request.NotificationId), request.Group, string.Empty);
            }

            if (_categoryList.Any())
            {
                var categoryByType = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
                if (categoryByType != null)
                {
                    foreach (var notificationAction in categoryByType.ActionList)
                    {
                        var button = new ToastButton()
                            .SetContent(notificationAction.Title)
                            .AddArgument(LocalNotificationCenter.ReturnRequest, request.NotificationId)
                            .AddArgument(LocalNotificationCenter.ReturnRequestActionId, notificationAction.ActionId);

                        if (!notificationAction.Windows.LaunchAppWhenTapped)
                        {
                            button.SetBackgroundActivation();
                        }
                        if (notificationAction.Windows.DismissWhenTapped)
                        {
                            button.SetDismissActivation();
                        }

                        builder.AddButton(button);
                    }
                }
            }

            if (request.Schedule != null && request.Schedule.NotifyTime != null)
            {
                builder.Schedule(new DateTimeOffset(request.Schedule.NotifyTime.Value), toast =>
                {
                    toast.Tag = request.NotificationId.ToString();
                    if (!string.IsNullOrEmpty(request.Group))
                    {
                        toast.Group = request.Group;
                    }
                });
            }
            else
            {
                builder.Show(toast =>
                {                    
                    toast.Tag = request.NotificationId.ToString();
                    if (!string.IsNullOrEmpty(request.Group))
                    {
                        toast.Group = request.Group;
                    }

                    toast.Activated += (sender, args) =>
                    {
                        var toastArgs = args as ToastActivatedEventArgs;
                        LocalNotificationCenter.NotifyNotificationTapped(toastArgs.Arguments);
                    };

                    toast.Dismissed += (sender, args) =>
                    {
                        var arguments = $"{LocalNotificationCenter.ReturnRequest}={sender.Tag};";
                        arguments += $"{LocalNotificationCenter.ReturnRequestActionId}={NotificationActionEventArgs.DismissedActionId}";

                        LocalNotificationCenter.NotifyNotificationTapped(arguments);
                    };
                });
            }

            return await Task.FromResult(true);
        }

    }
}