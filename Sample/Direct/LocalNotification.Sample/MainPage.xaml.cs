using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class MainPage : ContentPage
    {
        private int _tapCount;

        public MainPage()
        {
            InitializeComponent();

            NotificationCenter.Current.NotificationReceived += ShowCustomAlertFromNotification;

            NotifyDatePicker.MinimumDate = DateTime.Today;
            NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

            ScheduleNotificationGroup();
            ScheduleNotification("first", 10);
            ScheduleNotification("second", 20);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
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
            var serializeReturningData = ObjectSerializer.SerializeObject(list);

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount,
                ReturningData = serializeReturningData,
                Android =
                {
                    IconSmallName = "icon1",
                    Color = 33468,
                    ProgressBarIndeterminate = false,
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
                var notifyDateTime = NotifyDatePicker.Date.Add(NotifyTimePicker.Time);
                if (notifyDateTime <= DateTime.Now)
                {
                    notifyDateTime = DateTime.Now.AddSeconds(10);
                }

                request.Schedule.NotifyTime = notifyDateTime;
                request.Schedule.Repeats = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No;
            }

            NotificationCenter.Current.Show(request);
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
            var serializeReturningData = ObjectSerializer.SerializeObject(list);

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

        private void ShowCustomAlertFromNotification(NotificationReceivedEventArgs e)
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
                    DisplayAlert(e.Request.Title, e.Request.Description, "OK");
                }
            });
        }
    }
}