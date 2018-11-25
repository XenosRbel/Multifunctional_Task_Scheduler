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
    class AlarmItem
    {
        public AlarmItem()
        {
        }

        public AlarmItem(DateTime time, bool @checked, int id)
        {
            Time = time;
            Checked = @checked;
            Id = id;
        }

        public DateTime Time { get; set; }
        public bool Checked { get; set; }
        public int Id { set; get; }
        public string NameAlarm { set; get; }
        public string DaysAlarm { set; get; }
        public string RingtoneUri { set; get;}
    }
}