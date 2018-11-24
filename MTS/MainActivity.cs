using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using MTS.Activity;
using MTS.Utils;
using Calendar = Android.Icu.Util.Calendar;
using TimeZone = Android.Icu.Util.TimeZone;

namespace MTS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation_menu);
            navigation.SetOnNavigationItemSelectedListener(this);

            new FragmentUtil(this, this.SupportFragmentManager)
                .CreateLoadView(Resource.Id.fragment_main_container, new SchedulerFragment());

            //var notify = new NotifyAlarmBuilder(this);
            //notify.CreateNotificationChannel();
            //notify.ContentInfo = "Info";
            //notify.ContentText = "Text";
            //notify.ContentTitle = "Title";
            //notify.Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_scheduler:
                {
                    new FragmentUtil(this, this.SupportFragmentManager)
                        .CreateLoadView(Resource.Id.fragment_main_container, new SchedulerFragment());
                        return true;
                }
                case Resource.Id.navigation_alarm:
                {
                    new FragmentUtil(this, this.SupportFragmentManager)
                        .CreateLoadView(Resource.Id.fragment_main_container, new AlarmFragment());
                    return true;
                }
                case Resource.Id.navigation_newsFeed:
                {
                    return true;
                }
                case Resource.Id.navigation_exit:
                {
                    this.FinishAffinity();
                    return true;
                }
            }
            return false;
        }
    }
}

