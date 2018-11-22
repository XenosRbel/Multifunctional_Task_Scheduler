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
using Android.Support.V7.Widget;
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
        private SQLiteDatabase _database;
        private AlarmAdapter _adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            _alarmItems = new List<AlarmItem>();

            _adapter = new AlarmAdapter(this.Activity, _alarmItems);

            var path1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //"/storage/emulated/0Android/dataMTS"
            var path = Android.OS.Environment.ExternalStorageDirectory.Path +
                       $"/Android/data/{Resources.GetString(Resource.String.app_name)}.{Resources.GetString(Resource.String.app_name)}/files/";
            var filePath = Path.Combine(path, "MTS.db");

            _database = SQLiteDatabase.OpenOrCreateDatabase(filePath, null);//getBaseContext().openOrCreateDatabase("app.db", MODE_PRIVATE, null);
            _database.ExecSQL("CREATE TABLE IF NOT EXISTS Alarms (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, alarmTime TEXT, alarmStatus INTEGER);");
            LoadData();
            //_database.Close();
            //db.ExecSQL("INSERT INTO users VALUES ('Tom Smith', 23);");
            ////db.ExecSQL("INSERT INTO users VALUES ('John Dow', 31);");
            //var query = db.RawQuery("SELECT * FROM users;", null);
            //if (query.MoveToFirst())
            //{

            //    String name = query.GetString(0);
            //    int age = query.GetInt(1);
            //}
        }

        private void LoadData()
        {
            var query = _database.RawQuery("SELECT * FROM Alarms;", null);
            if (query.MoveToFirst())
            {
                do
                {
                    _alarmItems.Add(new AlarmItem()
                    {
                        Checked = Convert.ToBoolean(query.GetInt(2)),
                        Time = Convert.ToDateTime(query.GetString(1)),
                        Id = Convert.ToInt32(query.GetInt(0))
                    });
                }
                while (query.MoveToNext());
            }
            query.Close();

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
            View view = (View)sender;

            ContentValues values = new ContentValues();
            values.Put("alarmTime", DateTime.Now.ToString());
            values.Put("alarmStatus", Convert.ToInt32(true));
            _database.Insert("Alarms", null, values);

            _alarmItems.Add(new AlarmItem()
            {
                Checked = true,
                Time = DateTime.Now,
                Id = _alarmItems.Count
            });

            _adapter.NotifyDataSetChanged();
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