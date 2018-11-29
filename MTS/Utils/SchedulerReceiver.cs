using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MTS.Entity;
using TimeZone = System.TimeZone;

namespace MTS.Utils
{
    [BroadcastReceiver]
    //[BroadcastReceiver(Enabled = true, Exported = false)]
    //[IntentFilter(new[] { "MTS.MTS" })]
    public class SchedulerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "MTS.MTS");

            //Acquire the lock
            wl.Acquire();
            var notify = new NotifyAlarmBuilder(context);
            notify.CreateNotificationChannel();
            notify.ContentText = intent.GetStringExtra("SchedulerDescription");
            notify.ContentTitle = intent.GetStringExtra("SchedulerTitle");
            notify.ContentRingtonePath = intent.GetStringExtra("RingtoneUri");
            notify.Show();

            //Release the lock
            wl.Release();
        }

        public void SetOnetimeTimer(Context context, long cal, SchedulerItem schedulerItem)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent intent = new Intent(context, typeof(SchedulerReceiver));
            intent.PutExtra("SchedulerDescription", schedulerItem.SchedulerDescription);
            intent.PutExtra("SchedulerTitle", schedulerItem.SchedulerTitle);
            if (schedulerItem.RingtoneUri != null)
            {
                intent.PutExtra("RingtoneUri", schedulerItem.RingtoneUri.Split(' ').ToArray()[0]);
            }

            PendingIntent pi = PendingIntent.GetBroadcast(context, schedulerItem.Id, intent, PendingIntentFlags.CancelCurrent);
            
            am.Set(AlarmType.RtcWakeup, cal, pi);
        }
    }
}