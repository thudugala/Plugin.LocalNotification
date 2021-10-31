using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.IO;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Xamarin.Forms;
using Plugin.LocalNotification.Json;

namespace LocalNotification.Sample
{
    public partial class MainPage : ContentPage
    {
        private int _tapCount;
        private NotificationSerializer notificationSerializer;

        public MainPage()
        {
            InitializeComponent();

            notificationSerializer = new NotificationSerializer();

            NotificationCenter.Current.RegisterCategoryList(new HashSet<NotificationCategory>(new List<NotificationCategory>()
            {
                new NotificationCategory(NotificationCategoryType.Status)
                {
                    ActionList = new HashSet<NotificationAction>( new List<NotificationAction>()
                    {
                        new NotificationAction(100)
                        {
                            Title = "Hello",
                            AndroidIconName =
                            {
                                ResourceName = "i2",
                            },
                            iOSAction = iOSActionType.None
                        },
                        new NotificationAction(101)
                        {
                            Title = "Close",
                            AndroidIconName =
                            {
                                ResourceName = "i3",
                            },
                            iOSAction = iOSActionType.None
                        }
                    })
                },
            }));

            NotificationCenter.Current.NotificationReceived += ShowCustomAlertFromNotification;
            NotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;

            NotifyDatePicker.MinimumDate = DateTime.Today;
            NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

            //ScheduleNotificationGroup();
            //ScheduleNotification("first", 10);
            //ScheduleNotification("second", 20);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var imageStream = GetType().Assembly.GetManifestResourceStream("LocalNotification.Sample.icon.png");
            byte[] imageBytes = null;
            if (imageStream != null)
            {
                using (var ms = new MemoryStream())
                {
                    imageStream.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }
            }

            _tapCount++;
            var notificationId = 100;
            var title = "Test";
            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                _tapCount.ToString()
            };
            var serializeReturningData = notificationSerializer.Serialize(list);

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Subtitle = $"Tap Count: {_tapCount}",
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount,
                ReturningData = serializeReturningData,
                Image =
                {
                    //ResourceName = "icon",
                    Binary = imageBytes
                },
                CategoryType = NotificationCategoryType.Status,
                Android =
                {
                    IconSmallName =
                    {
                        ResourceName = "icon1",
                    },
                    Color =
                    {
                        ResourceName = "colorPrimary"
                    },
                    IsProgressBarIndeterminate = false,
                    ProgressBarMax = 20,
                    ProgressBarProgress = _tapCount
                    //AutoCancel = false,
                    //Ongoing = true
                },
                iOS =
                {
                    HideForegroundAlert = CustomAlert.IsToggled,
                    PlayForegroundSound = ForegroundSound.IsToggled
                }
            };

            // if not specified, default sound will be played.
            if (CustomSoundSwitch.IsToggled)
            {
                request.Sound = Device.RuntimePlatform == Device.Android
                    ? "good_things_happen"
                    : "good_things_happen.aiff";
            }

            // if not specified, notification will show immediately.
            if (UseNotifyTimeSwitch.IsToggled)
            {
                //var notifyDateTime = NotifyDatePicker.Date.Add(NotifyTimePicker.Time);
                //if (notifyDateTime <= DateTime.Now)
                //{
                //    notifyDateTime = DateTime.Now.AddSeconds(10);
                //}
                var notifyDateTime = DateTime.Now.AddSeconds(30);

                request.Schedule.NotifyAutoCancelTime = DateTime.Now.AddMinutes(5);
                request.Schedule.NotifyTime = notifyDateTime;
                //request.Schedule.RepeatType = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No;
                request.Schedule.RepeatType = NotificationRepeat.TimeInterval;
                request.Schedule.NotifyRepeatInterval = TimeSpan.FromMinutes(2);
            }

            NotificationCenter.Current.Show(request);
        }

        private void Current_NotificationActionTapped(NotificationActionEventArgs e)
        {
            switch (e.ActionId)
            {
                case 100:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(e.Request.Title, "Hello Button was Tapped", "OK");
                    });
                    break;

                case 101:
                    NotificationCenter.Current.Cancel(e.Request.NotificationId);
                    break;
            }
        }

        private void ScheduleNotification(string title, double seconds)
        {
            _tapCount++;
            var notificationId = (int)DateTime.Now.Ticks;
            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                _tapCount.ToString()
            };
            var serializeReturningData = notificationSerializer.Serialize(list);

            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = $"Tap Count: {_tapCount}",
                ReturningData = serializeReturningData,
                Schedule =
                {
                    NotifyTime = DateTime.Now.AddSeconds(seconds),
                },
                Android =
                {
                    Group = AndroidOptions.DefaultGroupId
                }
            };
            NotificationCenter.Current.Show(notification);
        }

        private void ScheduleNotificationGroup()
        {
            var notificationId = (int)DateTime.Now.Ticks;
            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = "Summary",
                Description = "Summary Desc",
                Android =
                {
                    Group = AndroidOptions.DefaultGroupId,
                    IsGroupSummary = true
                }
            };
            NotificationCenter.Current.Show(notification);
        }

        private void ShowCustomAlertFromNotification(NotificationEventArgs e)
        {
            if (e.Request is null)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(e);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (CustomAlert.IsToggled)
                {
                    var requestJson = notificationSerializer.Serialize(e.Request);

                    DisplayAlert(e.Request.Title, requestJson, "OK");
                }
            });
        }
    }
}