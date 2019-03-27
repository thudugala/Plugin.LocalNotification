using System.Threading.Tasks;
using Android.App;
using Android.App.Job;

namespace Plugin.LocalNotification.Platform.Droid
{
    [Service(Name = "plugin.LocalNotification.ScheduledNotificationJobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    internal class ScheduledNotificationJobService : JobService
    {
        public override bool OnStartJob(JobParameters jobParams)
        {
            if (jobParams.Extras.ContainsKey(NotificationCenter.ExtraReturnNotification) == false)
            {
                return false;
            }
            
            Task.Run(() =>
            {
                var serializedNotification = jobParams.Extras.GetString(NotificationCenter.ExtraReturnNotification);
                var notification = ObjectSerializer<NotificationRequest>.DeserializeObject(serializedNotification);
                
                NotificationCenter.Current.Show(notification);
                JobFinished(jobParams, false);
            });
            
            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            // Called by Android when it has to terminate a running service.
            return false; // Don't reschedule the job.
        }
    }
}