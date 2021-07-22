using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Plugin.LocalNotification.AndroidOption;
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _tapCount++;

            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                _tapCount.ToString()
            };

            var serializeReturningData = JsonSerializer.Serialize(list);

            var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Test",
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount,
                ReturningData = serializeReturningData,
                Android =
                {
                    IconSmallName = new AndroidIcon("my_icon"),
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
                request.Schedule.NotifyTime = notifyDateTime;
                request.Schedule.RepeatType = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No;
            }

            await NotificationCenter.Current.Show(request);
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
                    var requestJson = JsonSerializer.Serialize(e.Request);

                    DisplayAlert(e.Request.Title, requestJson, "OK");
                }
            });
        }
    }
}