using Plugin.LocalNotification;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LocalNotification.Sample
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            NotificationCenter.NotificationTapped += LoadPageFromNotification;

            MainPage = new NavigationPage(new MainPage());
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

        private void LoadPageFromNotification(NotificationTappedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var list = ObjectSerializer.DeserializeObject<List<string>>(e.Data);
            if (list.Count != 4)
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

            ((NavigationPage)MainPage).Navigation.PushAsync(new NotificationPage(int.Parse(id), message, int.Parse(tapCount)));
        }
    }
}