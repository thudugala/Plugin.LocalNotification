using Plugin.LocalNotification;
using Plugin.LocalNotification.Json;
using System;
using Plugin.LocalNotification.EventArgs;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            LocalNotificationCenter.NotificationLog += NotificationCenter_NotificationLog;
        }

        private void NotificationCenter_NotificationLog(NotificationLogArgs e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Error);
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
    }
}