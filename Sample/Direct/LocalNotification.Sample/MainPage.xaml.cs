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
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            _tapCount++;

            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                _tapCount.ToString()
            };

            var notifyDateTime = NotifyDatePicker.Date.Add(NotifyTimePicker.Time);
            if (notifyDateTime <= DateTime.Now)
            {
                notifyDateTime = DateTime.Now.AddSeconds(10);
            }

            var serializeReturningData = ObjectSerializer<List<string>>.SerializeObject(list);

            var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Test",
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount,
                ReturningData = serializeReturningData,
                NotifyTime = UseNotifyTimeSwitch.IsToggled ? notifyDateTime : (DateTime?)null // if not specified notification will show immediately.
            };

            NotificationCenter.Current.Show(request);
        }
    }
}