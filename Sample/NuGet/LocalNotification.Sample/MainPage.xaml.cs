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
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            _tapCount++;

            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                _tapCount.ToString()
            };

            var serializeReturningData = ObjectSerializer.SerializeObject(list);

            var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Test",
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount,
                ReturningData = serializeReturningData,
                Repeats = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No,
                Android =
                {
                    IconName = "my_icon",
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

        private void ShowCustomAlertFromNotification(NotificationReceivedEventArgs e)
        {
            if (e is null)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(e);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (CustomAlert.IsToggled)
                {
                    DisplayAlert(e.Title, e.Description, "OK");
                }
            });
        }
    }
}