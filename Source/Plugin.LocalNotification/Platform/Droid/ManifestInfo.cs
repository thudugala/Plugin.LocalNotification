//Permissions for android

using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
//[assembly: UsesPermission(Manifest.Permission.ScheduleExactAlarm)]
[assembly: UsesPermission("android.permission.SCHEDULE_EXACT_ALARM")]
[assembly: LinkerSafe]