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
using MTS.Entity;

namespace MTS.Adapters
{
    class AlarmAdapter : BaseAdapter<AlarmItem>
    {
        private Android.App.Activity _context;
        private List<AlarmItem> _items;

        public AlarmAdapter(Android.App.Activity context)
        {
            this.Context = context;
        }

        public AlarmAdapter(Android.App.Activity context, List<AlarmItem> items)
        {
            _context = context;
            _items = items;
        }

        public override AlarmItem this[int position] => Items[position];

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = Items[position];

            var view = convertView ?? Context.LayoutInflater.Inflate(Resource.Layout.item_list_alarm, null);

            view.FindViewById<TextClock>(Resource.Id.text_time_alarm).Text = Convert.ToString(item.Time.TimeOfDay);
            view.FindViewById<ToggleButton>(Resource.Id.toggle_btn_alarm).Checked = item.Checked;
            view.FindViewById<TextView>(Resource.Id.text_day_alarm).Text = Convert.ToString(item.Time.DayOfWeek);

            return view;
        }

        public override int Count => Items.Count;

        public Android.App.Activity Context { get => _context; set => _context = value; }
        public List<AlarmItem> Items { get => _items; set => _items = value; }
    }

    class AlarmAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}

