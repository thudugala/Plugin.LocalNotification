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

            var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                _tapCount.ToString()
            };

            notification.ReturningData = DataSerializer<List<string>>.SerializeReturningData(list);

            notificationService.Show(notification);
        }
    }
}
