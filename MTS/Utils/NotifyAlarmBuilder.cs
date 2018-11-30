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
        public string ContentRingtonePath { set; get; }

        public NotifyAlarmBuilder()
        {
            
        }

        public NotifyAlarmBuilder(Context activity)
        {
            _activity = activity;
        }

        public void Show()
        {
            var uri = Uri.Parse(ContentRingtonePath);
            
            var builder = new NotificationCompat.Builder(this._activity);
            var manager = (NotificationManager)this._activity.GetSystemService(Context.NotificationService);
            builder.SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.baseline_exit_to_app_24px)
                .SetContentTitle(this.ContentTitle)
                .SetContentText(this.ContentText)
                .SetCategory(Notification.CategoryEvent)
                .SetDefaults((int)NotificationDefaults.Vibrate)
                .SetSound(uri, (int)Stream.Notification);

            manager.Notify(1, builder.Build());
        }
    }
}