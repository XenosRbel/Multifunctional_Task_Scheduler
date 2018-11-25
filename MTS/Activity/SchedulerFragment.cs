using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using MTS.Adapters;
using MTS.Entity;
using MTS.Utils;
using VKontakte;

namespace MTS.Activity
{
    public class SchedulerFragment : Android.Support.V4.App.Fragment
    {
        private View _view;
        private ListViewCompat _listView;
        private SQLiteDatabase _database;
        private SchedulerAdapter _adapter;
        private SqLiteDBUtil _sqliteDbUtil;
        private List<SchedulerItem> _schedulerItems;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _schedulerItems = new List<SchedulerItem>();

            _adapter = new SchedulerAdapter(this.Activity, _schedulerItems);

            _sqliteDbUtil = new SqLiteDBUtil(this.Activity);

            LoadData();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_scheduler, container, false);

            _listView = _view.FindViewById<Android.Support.V7.Widget.ListViewCompat>(Resource.Id.list_view_scheduler);

            FloatingActionButton fab = _view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_scheduler);
            fab.Click += Fab_Click;

            SetDataToAdapter();
            return _view;
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            var selectedTime = new DateTime();

            var frag = DatePickerFragment.NewInstance(delegate (DateTime date)
            {
                var output = new DateTime(date.Year, date.Month, date.Day,
                    selectedTime.Hour, selectedTime.Minute, selectedTime.Second);

                var id = DateTime.Now.ToString().GetHashCode();
                _schedulerItems.Add(new SchedulerItem()
                    {
                        Id = id,
                        SchedulerTitle = "Без названия",
                        Time = output
                    }
                );

                var values = new ContentValues();
                values.Put("id", id.ToString());
                values.Put("schedulerTitle", "Без названия");
                values.Put("Time", output.ToString());

                _sqliteDbUtil.InsertRowScheduler(values);
                _adapter.NotifyDataSetChanged();
            });

            var timePicker = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                selectedTime = time;

                frag.Show(this.FragmentManager, DatePickerFragment.TAG);
            });

            timePicker.Show(this.FragmentManager, TimePickerFragment.TAG);

        }

        private void LoadData()
        {
            _sqliteDbUtil.GetSchedulers(ref _schedulerItems);
            _adapter.NotifyDataSetChanged();
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