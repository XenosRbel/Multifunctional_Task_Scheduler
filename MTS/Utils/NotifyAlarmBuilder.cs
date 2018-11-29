using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Uri = Android.Net.Uri;

namespace MTS.Utils
{
    class NotifyAlarmBuilder
    {
        private Context _activity;
        public string ContentTitle { set; get; }
        public string ContentText { set; get; }
        public string ContentInfo { set; get; }
        public string ContentRingtonePath { set; get; }

        public static string ChannelId => channelId;
        public static string ChannelDescription => channelDescription;
        public static string ChannelName => channelName;

        private const string channelName = "Alarm";
        private const string channelDescription = "Alarm Notification";
        private const string channelId = "Alarm_Notify";

        public NotifyAlarmBuilder()
        {
            
        }

        public NotifyAlarmBuilder(Context activity)
        {
            _activity = activity;
        }

        public void CreateNotificationChannel()
        {
            //if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            //{
            //    return;
            //}

            //var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
            //{
            //    Description = ChannelDescription,
            //    LockscreenVisibility = NotificationVisibility.Public
            //};

            //var notificationManager = (NotificationManager)_activity.GetSystemService(Context.NotificationService);
            //notificationManager.CreateNotificationChannel(channel);
        }

        public void Show()
        {
            var builder = new NotificationCompat.Builder(this._activity);
            var manager = (NotificationManager)this._activity.GetSystemService(Context.NotificationService);
            builder.SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.baseline_exit_to_app_24px)
                .SetContentTitle(this.ContentTitle)
                .SetContentText(this.ContentText)
                .SetCategory(Notification.CategoryEvent)
                .SetDefaults(NotificationCompat.DefaultAll)
                .SetSound(Uri.Parse(ContentRingtonePath))
                .SetVibrate(new long[20]);

            manager.Notify(1, builder.Build());
        }
    }
}