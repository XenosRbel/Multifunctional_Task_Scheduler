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
    class SchedulerItem
    {
        public SchedulerItem()
        {
            
        }

        public SchedulerItem(int id, string schedulerTitle, string schedulerText)
        {
            Id = id;
            SchedulerTitle = schedulerTitle;
            SchedulerText = schedulerText;
        }

        public int Id { set; get; }
        public string SchedulerTitle { set; get; }
        public string SchedulerText { set; get; }
    }
}