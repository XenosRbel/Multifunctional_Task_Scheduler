using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using MTS.Entity;

namespace MTS.Utils
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "MTS.MTS" })]
    class TimerExecuteActions : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "MTS.MTS");

            wl.Acquire();
            
            //Your code for Delete data from DB

            wl.Release();
        }

        public void CancelAlarm(Context context)
        {
            Intent intent = new Intent(context, typeof(SchedulerReceiver));
            PendingIntent sender = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.CancelCurrent);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }

        public void SetOnetimeTimer(Context context)
        {
            Intent intent = new Intent(context, typeof(TimerExecuteActions));
            

            PendingIntent pi = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var timeSec = 5;

            am.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + timeSec * 1000, pi);
        }
    }
}