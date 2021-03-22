using Android.App;
using Android.Content;
using AndroidX.Work;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Categories = new[] { Intent.CategoryHome })]
    internal class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // HACK: Used to resume one-time workers after reboot.
            var _ = WorkManager.GetInstance(Application.Context);

            Android.Util.Log.Info(Application.Context.PackageName, $"{nameof(BootReceiver)}-{nameof(OnReceive)}");
        }
    }
}