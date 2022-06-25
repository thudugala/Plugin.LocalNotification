using Microsoft.Extensions.Logging.Debug;
using Plugin.LocalNotification;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            LocalNotificationCenter.Logger = new DebugLoggerProvider().CreateLogger("LocalNotification.Sample");
            LocalNotificationCenter.Current.RegisterCategoryList(new HashSet<NotificationCategory>(new List<NotificationCategory>()
            {
                new NotificationCategory(NotificationCategoryType.Status)
                {
                    ActionList = new HashSet<NotificationAction>( new List<NotificationAction>()
                    {
                        new NotificationAction(100)
                        {
                            Title = "Hello",
                            Android =
                            {
                                IconName =
                                {
                                    ResourceName = "i2"
                                }
                            }
                        },
                        new NotificationAction(101)
                        {
                            Title = "Close",
                            Android =
                            {
                                IconName =
                                {
                                    ResourceName = "i3"
                                }
                            }
                        }
                    })
                },
            }));
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