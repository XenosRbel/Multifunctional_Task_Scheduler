﻿using System;
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

            _adapter = new SchedulerAdapter(this.Activity);

            _sqliteDbUtil = new SqLiteDBUtil(this.Activity);

            LoadData();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_scheduler, container, false);

            _listView = _view.FindViewById<Android.Support.V7.Widget.ListViewCompat>(Resource.Id.list_view_scheduler);

            FloatingActionButton fab = _view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_scheduler);
            fab.Click += Fab_Click;

            return _view;
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            new FragmentUtil(this.Activity, this.Activity.SupportFragmentManager)
                .CreateLoadView(Resource.Id.fragment_main_container, new SchedulerNewItemFragment());
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