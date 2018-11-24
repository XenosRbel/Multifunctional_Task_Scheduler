using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;
using MTS.Adapters;
using MTS.Entity;
using MTS.Utils;

namespace MTS.Activity
{
    public class AlarmFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        private ListViewCompat _listView;
        private List<AlarmItem> _alarmItems;
        private AlarmAdapter _adapter;
        private SqLiteDBUtil _sqliteDbUtil;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _alarmItems = new List<AlarmItem>();

            _adapter = new AlarmAdapter(this.Activity, _alarmItems);
            
            _sqliteDbUtil = new SqLiteDBUtil(this.Activity);

            LoadData();
        }

        private void LoadData()
        {
            _sqliteDbUtil.GetAlarmItems(ref _alarmItems);

            _adapter.NotifyDataSetChanged();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_alarm, container, false);

            _listView = _view.FindViewById<Android.Support.V7.Widget.ListViewCompat>(Resource.Id.list_view_alarm);

            FloatingActionButton fab = _view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            SetDataToAdapter();

            return _view;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            var selectedTime = new DateTime();

            var frag = DatePickerFragment.NewInstance(delegate (DateTime date)
            {
                var output = new DateTime(date.Year, date.Month, date.Day,
                    selectedTime.Hour, selectedTime.Minute, selectedTime.Second);

                ContentValues values = new ContentValues();
                values.Put("alarmTime", output.ToString());
                values.Put("alarmStatus", Convert.ToInt32(false));
                values.Put("nameAlarm", "Без названия");
                
                _sqliteDbUtil.InsertRowAlarms(values);

                _alarmItems.Add(new AlarmItem()
                {
                    Checked = false,
                    Time = output,
                    Id = _alarmItems.Count,
                    NameAlarm = "Без названия"
                });
                _adapter.NotifyDataSetChanged();

                var alarm = new AlarmReceiver();
                alarm.SetOnetimeTimer(this.Activity, output);
            });

            var timePicker = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                selectedTime = time;

                frag.Show(this.FragmentManager, DatePickerFragment.TAG);
            });

            timePicker.Show(this.FragmentManager, TimePickerFragment.TAG);
      
            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //.SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private async void SetDataToAdapter()
        {
            await Task.Run((() =>
            {
                this.Activity.RunOnUiThread(() =>
                {
                    _listView.Adapter = _adapter;
                });
            }));
        }
    }
}