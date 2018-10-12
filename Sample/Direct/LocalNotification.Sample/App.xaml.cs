using System.Collections.Generic;
using Plugin.LocalNotification;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LocalNotification.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Local Notification tap event listener
            MessagingCenter.Instance.Subscribe<LocalNotificationTappedEvent>(this,
                typeof(LocalNotificationTappedEvent).FullName, LoadPageFromNotification);

            GoToMainPage();
        }

        public new static App Current => Application.Current as App;

        public void GoToMainPage()
        {
            MainPage = new MainPage();
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
            // Handle when your app starts
        }

        private void LoadPageFromNotification(LocalNotificationTappedEvent e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var list = ObjectSerializer<List<string>>.DeserializeObject(e.Data);
            if (list.Count != 2)
            {
                return;
            }
            if (list[0] != typeof(NotificationPage).FullName)
            {
                return;
            }
            var tapCount = list[1];

            MainPage = new NotificationPage(int.Parse(tapCount));
        }
    }
}