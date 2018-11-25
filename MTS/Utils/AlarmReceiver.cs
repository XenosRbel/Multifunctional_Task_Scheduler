using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using TimeZone = Java.Util.TimeZone;

namespace MTS.Utils
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "MTS.MTS");
            //Acquire the lock
            wl.Acquire();

            var notify = new NotifyAlarmBuilder(context);
            notify.CreateNotificationChannel();
            notify.ContentInfo = "Info";
            notify.ContentText = "Text";
            notify.ContentTitle = "Title";
            notify.Show();

            //Release the lock
            System.Diagnostics.Debug.Print("DDbug " + DateTime.Now);
            wl.Release();
        }

        public void CancelAlarm(Context context, int requestCode)
        {
            Intent intent = new Intent(context, typeof(AlarmReceiver));
            PendingIntent sender = PendingIntent.GetBroadcast(context, requestCode, intent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }
        public void setOnetimeTimer(Context context)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent intent = new Intent(context, typeof(AlarmReceiver));

            var date = DateTime.Now;
            date = date.AddMinutes(1);

            var cal = Calendar.GetInstance(TimeZone.Default);
            cal.Set(date.Year, date.Month, date.Day, date.Hour, date.Minute);

            PendingIntent pi = PendingIntent.GetBroadcast(context, 1, intent, PendingIntentFlags.CancelCurrent);
     
            long time = date.Minute * 600000;
            am.Set(AlarmType.RtcWakeup, cal.TimeInMillis, pi);
        }
        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}