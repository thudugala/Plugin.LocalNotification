using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace LocalNotification.Sample
{
    public class MainViewModel : ObservableObject
    {
        public bool NotificationHandled
        {
            get => App.NotificationHandled; set => App.NotificationHandled = value;
        }
    }
}
