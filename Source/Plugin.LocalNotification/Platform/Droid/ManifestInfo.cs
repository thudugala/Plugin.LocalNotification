//Permissions for android

using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
[assembly: UsesPermission(Manifest.Permission.ScheduleExactAlarm)]
[assembly: LinkerSafe]