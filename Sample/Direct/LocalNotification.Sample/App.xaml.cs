using Plugin.LocalNotification;
using System.Collections.Generic;
using System.Text.Json;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
        }

        private async void LoadPageFromNotification(NotificationTappedEventArgs e)
        {
            if (e.Request is null)
            {
                return;
            }

            var list = JsonSerializer.Deserialize<List<string>>(e.Request.ReturningData);
            if (list is null || list.Count != 4)
            {
                return;
            }

            if (list[0] != typeof(NotificationPage).FullName)
            {
                return;
            }

            var id = list[1];
            var message = list[2];
            var tapCount = list[3];

            await ((NavigationPage) MainPage).Navigation.PushModalAsync(new NotificationPage(int.Parse(id), message,
                int.Parse(tapCount)));
        }
    }
}