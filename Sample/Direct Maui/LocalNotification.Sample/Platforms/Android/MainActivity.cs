using Android.App;
using Android.Content.PM;
using Android.OS;

namespace LocalNotification.Sample;

[Activity(Theme = "@style/Maui.SplashTheme",
    MainLauncher = true, 
    ConfigurationChanges = ConfigChanges.ScreenSize | 
                            ConfigChanges.Orientation | 
                            ConfigChanges.UiMode | 
                            ConfigChanges.ScreenLayout | 
                            ConfigChanges.SmallestScreenSize | 
                            ConfigChanges.Density,
    ShowForAllUsers = true,
    ShowWhenLocked = true,
    TurnScreenOn = true)]
public class MainActivity : MauiAppCompatActivity
{
    
}
