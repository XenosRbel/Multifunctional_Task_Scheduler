using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MTS.Entity
{
    public class SchedulerItem
    {
        public SchedulerItem()
        {
            
        }

        public int Id { set; get; }
        public string SchedulerTitle { set; get; }
        public string SchedulerDescription { set; get; }
        public DateTime Time { set; get; }
        public string RingtoneUri { set; get; }
        
    }
}