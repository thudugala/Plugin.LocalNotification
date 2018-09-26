using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class MainPage : ContentPage
    {
        private int _tapCount;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            _tapCount++;

            var notificationService = DependencyService.Get<ILocalNotificationService>();
            var notification = new Plugin.LocalNotification.LocalNotification
            {
                NotificationId = 100,
                Title = "Test",
                Description = $"Tap Count: {_tapCount}",
                BadgeNumber = _tapCount
            };
            notification.ReturningData.Add(typeof(NotificationPage).FullName);
            notification.ReturningData.Add(_tapCount.ToString());

            notificationService.Send(notification);
        }
    }
}
