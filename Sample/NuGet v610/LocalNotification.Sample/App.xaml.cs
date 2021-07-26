﻿using System.Collections.Generic;
using Plugin.LocalNotification;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;

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
            if (string.IsNullOrWhiteSpace(e.Request.ReturningData))
            {
                return;
            }

            var list = ObjectSerializer.DeserializeObject<List<string>>(e.Request.ReturningData);
            if (list.Count != 2)
            {
                return;
            }
            if (list[0] != typeof(NotificationPage).FullName)
            {
                return;
            }
            var tapCount = list[1];

            ((NavigationPage)MainPage).Navigation.PushAsync(new NotificationPage(int.Parse(tapCount)));
        }
    }
}