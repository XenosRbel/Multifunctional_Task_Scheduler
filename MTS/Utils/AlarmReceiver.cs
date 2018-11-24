using System;
using System.Collections.Generic;
using System.Globalization;
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
using Calendar = Java.Util.Calendar;
using TimeZone = Java.Util.TimeZone;

namespace MTS.Utils
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static String ONE_TIME = "onetime";

        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "MTS.MTS");

            //Acquire the lock
            wl.Acquire();
     
            var notify = new NotifyAlarmBuilder(context);
            notify.CreateNotificationChannel();
            notify.ContentText = "Alarm Receiver";
            notify.ContentTitle = "Time is Over";
            notify.Show();

            //Release the lock
            wl.Release();
        }

        public void CancelAlarm(Context context, int requestCode)
        {
            Intent intent = new Intent(context, typeof(AlarmReceiver));
            PendingIntent sender = PendingIntent.GetBroadcast(context, requestCode, intent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }

        public void SetOnetimeTimer(Context context, DateTime date)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent intent = new Intent(context, typeof(AlarmReceiver));
            intent.PutExtra(ONE_TIME, true);

            PendingIntent pi = PendingIntent.GetBroadcast(context, 0, intent, 0);
            
            am.Set(AlarmType.RtcWakeup, date.Millisecond, pi);

            Toast.MakeText(context, date.ToString(CultureInfo.InvariantCulture), ToastLength.Long).Show();
        }
    }
}