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
using MTS.Entity;
using Java.Util;

namespace MTS.Utils
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "MTS.MTS" })]
    public class SchedulerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "MTS.MTS");
            
            wl.Acquire();
            var notify = new NotifyAlarmBuilder(context);
            notify.ContentText = intent.GetStringExtra("SchedulerDescription");
            notify.ContentTitle = intent.GetStringExtra("SchedulerTitle");
            notify.ContentRingtonePath = intent.GetStringExtra("RingtoneUri");
            notify.Show();
            
            wl.Release();
        }

        public void SetOnetimeTimer(Context context, SchedulerItem schedulerItem)
        {
            Calendar calendar = NewMethod(schedulerItem);

            Intent intent = new Intent(context, typeof(SchedulerReceiver));
            intent.PutExtra("SchedulerDescription", schedulerItem.SchedulerDescription);
            intent.PutExtra("SchedulerTitle", schedulerItem.SchedulerTitle);
            if (schedulerItem.RingtoneUri != null)
            {
                var ringrone = schedulerItem.RingtoneUri.Split(' ').ToArray()[0];
                intent.PutExtra("RingtoneUri",ringrone);
            }

            PendingIntent pi = PendingIntent.GetBroadcast(context, schedulerItem.Id, intent, PendingIntentFlags.UpdateCurrent);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pi);

            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy MMMM dd HH:mm:ss");
            string datFormat = dateFormat.Format(calendar.Time);

            Toast.MakeText(context, $"Задача создана на {datFormat}", ToastLength.Long).Show();
        }

        private static Calendar NewMethod(SchedulerItem schedulerItem)
        {
            var tz = Java.Util.TimeZone.GetTimeZone("GMT+03:00");
            var calendar = Java.Util.Calendar.GetInstance(tz);
            calendar.Set(Java.Util.CalendarField.Year, schedulerItem.Time.Year);
            calendar.Set(Java.Util.CalendarField.DayOfMonth, schedulerItem.Time.Day -1);
            calendar.Set(CalendarField.Month, schedulerItem.Time.Month - 1);
            calendar.Set(CalendarField.Hour, schedulerItem.Time.Hour);
            calendar.Set(CalendarField.Minute, schedulerItem.Time.Minute);
            calendar.Set(CalendarField.Second, 0);
            calendar.Set(CalendarField.Millisecond, 0);
            return calendar;
        }
    }
}