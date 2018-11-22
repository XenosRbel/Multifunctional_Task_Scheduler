using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MTS.Entity;

namespace MTS.Adapters
{
    class AlarmAdapter : BaseAdapter<AlarmItem>
    {
        private Android.App.Activity _context;
        private List<AlarmItem> _items;
        private TextView _textClock, _textView;
        private ImageButton _imageButton;
        private SwitchCompat _switchCompat;
        private SQLiteDatabase _database;

        public override int Count => Items.Count;
        public Android.App.Activity Context { get => _context; set => _context = value; }
        public List<AlarmItem> Items { get => _items; set => _items = value; }

        public AlarmAdapter(Android.App.Activity context)
        {
            this.Context = context;
        }

        public AlarmAdapter(Android.App.Activity context, List<AlarmItem> items)
        {
            Context = context;
            Items = items;
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

            _textClock = view.FindViewById<TextView>(Resource.Id.text_time_alarm);
            _textClock.Text = item.Time.ToString("HH:mm");

            _switchCompat = view.FindViewById<SwitchCompat>(Resource.Id.toggle_btn_alarm);
            _switchCompat.Checked = item.Checked;
            _switchCompat.Tag = item.Id;
            _switchCompat.CheckedChange += _switchCompat_CheckedChange;

            _textView = view.FindViewById<TextView>(Resource.Id.text_day_alarm);
            _textView.Text = Convert.ToString(item.Time.DayOfWeek);

            _imageButton = view.FindViewById<ImageButton>(Resource.Id.btn_delete_alarm);
            _imageButton.Tag = item.Id;
            _imageButton.Click += _imageButton_Click;

            return view;
        }

        private void _imageButton_Click(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;

            var path = Android.OS.Environment.ExternalStorageDirectory.Path +
                       $"/Android/data/{_context.Resources.GetString(Resource.String.app_name)}.{_context.Resources.GetString(Resource.String.app_name)}/files/";
            var filePath = Path.Combine(path, "MTS.db");

            _database = SQLiteDatabase.OpenOrCreateDatabase(filePath, null);
            _database.ExecSQL("CREATE TABLE IF NOT EXISTS Alarms (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, alarmTime TEXT, alarmStatus INTEGER);");

            _database.Delete("Alarms", $"id = {button.Tag.ToString()}", null);

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Id.ToString() != button.Tag.ToString()) continue;
                Items.RemoveAt(i);

                this.NotifyDataSetChanged();
            }
        }

        private void _switchCompat_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var switchCompat = (SwitchCompat) sender;

            var path = Android.OS.Environment.ExternalStorageDirectory.Path +
                       $"/Android/data/{_context.Resources.GetString(Resource.String.app_name)}.{_context.Resources.GetString(Resource.String.app_name)}/files/";
            var filePath = Path.Combine(path, "MTS.db");

            _database = SQLiteDatabase.OpenOrCreateDatabase(filePath, null);
            _database.ExecSQL("CREATE TABLE IF NOT EXISTS Alarms (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, alarmTime TEXT, alarmStatus INTEGER);");

            ContentValues values = new ContentValues();
            values.Put("alarmStatus", Convert.ToInt32(switchCompat.Checked));

            _database.Update("Alarms", values, $"id = {switchCompat.Tag.ToString()}", null);

            foreach (var item in Items)
            {
                if (item.Id.ToString() != switchCompat.Tag.ToString()) continue;
                item.Checked = switchCompat.Checked;

                this.NotifyDataSetChanged();
            }
        }
    }

    class AlarmAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}

