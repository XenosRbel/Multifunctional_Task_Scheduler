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
using Android.Views;
using Android.Widget;
using MTS.Entity;

namespace MTS.Utils
{
    class SqLiteDBUtil
    {
        private SQLiteDatabase _database;
        private Android.App.Activity _activity;

        public SQLiteDatabase Database { get => _database; set => _database = value; }
        public Android.App.Activity Activity { get => _activity; set => _activity = value; }

        public SqLiteDBUtil(Android.App.Activity activity)
        {
            this.Activity = activity;
            Database = SQLiteDatabase.OpenOrCreateDatabase(GetPathDB(), null);

            this.InitAlarmTable()
                .InitSchedulerTable();
        }

        public string GetPathDB()
        {
            var path = Android.OS.Environment.ExternalStorageDirectory.Path +
                       $"/Android/data/{Activity.Resources.GetString(Resource.String.app_name)}.{Activity.Resources.GetString(Resource.String.app_name)}/files/";
            var filePath = Path.Combine(path, "MTS.db");
            return filePath;
        }

        public SqLiteDBUtil InitAlarmTable()
        {
            Database.ExecSQL("CREATE TABLE IF NOT EXISTS " +
                              "Alarms (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                              "alarmTime TEXT, " +
                              "alarmStatus INTEGER, " +
                              "nameAlarm TEXT);");

            return this;
        }

        public SqLiteDBUtil InitSchedulerTable()
        {
            Database.ExecSQL("CREATE TABLE IF NOT EXISTS " +
                              "Scheduler (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                              "schedulerTitle TEXT, " +
                              "schedulerText TEXT);");
            return this;
        }

        public void InsertRowAlarms(ContentValues values)
        {
            Database.Insert("Alarms", null, values);
        }

        public void UpdateRowAlarms(ContentValues values, string idRow)
        {
            Database.Update("Alarms", values, $"id = {idRow}", null);
        }

        public void DeleteRowAlarms(string idRow)
        {
            Database.Delete("Alarms", $"id = {idRow}", null);
        }

        public void InsertRowScheduler(ContentValues values)
        {
            Database.Insert("Scheduler", null, values);
        }

        public void UpdateRowScheduler(ContentValues values, string idRow)
        {
            Database.Update("Scheduler", values, $"id = {idRow}", null);
        }

        public void DeleteRowScheduler(string idRow)
        {
            Database.Delete("Scheduler", $"id = {idRow}", null);
        }

        public void GetAlarmItems(ref List<AlarmItem> data)
        {
            var query = Database.RawQuery("SELECT * FROM Alarms;", null);
            if (query.MoveToFirst())
            {
                do
                {
                    data.Add(new AlarmItem()
                    {
                        Checked = Convert.ToBoolean(query.GetInt(2)),
                        Time = Convert.ToDateTime(query.GetString(1)),
                        Id = Convert.ToInt32(query.GetInt(0)),
                        NameAlarm = Convert.ToString(query.GetString(3))
                    });
                }
                while (query.MoveToNext());
            }
            query.Close();
        }

        public void GetSchedulers(ref List<SchedulerItem> data)
        {
            var query = Database.RawQuery("SELECT * FROM Scheduler;", null);
            if (query.MoveToFirst())
            {
                do
                {
                    //data.Add(new SchedulerItem()
                    //{
                    //    Checked = Convert.ToBoolean(query.GetInt(2)),
                    //    Time = Convert.ToDateTime(query.GetString(1)),
                    //    Id = Convert.ToInt32(query.GetInt(0))
                    //});
                }
                while (query.MoveToNext());
            }
            query.Close();
        }
    }
}