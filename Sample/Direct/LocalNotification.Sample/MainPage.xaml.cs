using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class MainPage
    {
        private int TapCount;

        public MainPage()
        {
            InitializeComponent();

            NotificationCenter.NotificationReceived += ShowCustomAlertFromNotification;

            NotifyDatePicker.MinimumDate = DateTime.Today;
            NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

            ScheduleNotificationGroup();
            ScheduleNotification("first", 5);
            ScheduleNotification("second", 10);
        }

        private void ShowCustomAlertFromNotification(NotificationReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (CustomAlert.IsToggled)
                    DisplayAlert(e.Title, e.Description, "OK");
            });
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
                    Group = "Plugin.LocalNotification.GROUP",
                    IsGroupSummary = true
                }
            };
            NotificationCenter.Show(notification);
        }

        private void ScheduleNotification(string title, double seconds)
        {
            TapCount++;
            var notificationId = (int)DateTime.Now.Ticks;
            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                TapCount.ToString()
            };
            var serializeReturningData = ObjectSerializer.SerializeObject(list);

            var notification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = $"Tap Count: {TapCount}",
                ReturningData = serializeReturningData,
                NotifyTime = DateTime.Now.AddSeconds(seconds),
                Android =
                {
                    Group = "Plugin.LocalNotification.GROUP"
                }
            };
            NotificationCenter.Show(notification);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            TapCount++;
            var notificationId = 100;
            var title = "Test";
            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                TapCount.ToString()
            };
            var serializeReturningData = ObjectSerializer.SerializeObject(list);

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = $"Tap Count: {TapCount}",
                BadgeNumber = TapCount,
                ReturningData = serializeReturningData,
                Repeats = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No,
                Android =
                {
                    IconName = "icon1",
                    Color = 33468,
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
                request.NotifyTime = notifyDateTime;
            }

            NotificationCenter.Show(request);
        }
    }
}