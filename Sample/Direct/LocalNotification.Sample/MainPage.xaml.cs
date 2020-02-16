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
            NotifyDatePicker.MinimumDate = DateTime.Today;
            NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

            //ScheduleNotification("first", 5);
            //ScheduleNotification("second", 10);
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
                NotifyTime = DateTime.Now.AddSeconds(seconds)
            };
            NotificationCenter.Current.Show(notification);
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
                Repeats = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No,
                Android =
                {
                    IconName = "icon1",
                    Color = 33468,
                    //AutoCancel = false,
                    //Ongoing = true
                },
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

            NotificationCenter.Current.Show(request);
        }
    }
}