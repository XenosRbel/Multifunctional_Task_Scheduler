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
using Android.Media;
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

            var timePicker = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                selectedTime = time;

                var id = DateTime.Now.ToString().GetHashCode();
                _alarmItems.Add(new AlarmItem()
                {
                    Checked = false,
                    Time = selectedTime,
                    Id = id,
                    NameAlarm = "Без названия",
                    DaysAlarm = ""
                });

                ContentValues values = new ContentValues();
                values.Put("id", id.ToString());
                values.Put("alarmTime", selectedTime.ToString());
                values.Put("alarmStatus", Convert.ToInt32(false));
                values.Put("nameAlarm", "Без названия");
                values.Put("daysAlarm", "");

                _sqliteDbUtil.InsertRowAlarms(values);
                _adapter.NotifyDataSetChanged();
            });

            timePicker.Show(this.FragmentManager, TimePickerFragment.TAG);
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