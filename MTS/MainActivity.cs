using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using MTS.Activity;
using MTS.Utils;
using VKontakte;
using VKontakte.API;
using VKontakte.API.Methods;
using VKontakte.Utils;
using Calendar = Android.Icu.Util.Calendar;
using TimeZone = Android.Icu.Util.TimeZone;

namespace MTS
{
    [Register("MTS.MTS.MainActivity")]
    [IntentFilter(new[] { Intent.ActionView }, DataScheme = "6762064", Categories = new[] {
        Intent.CategoryBrowsable,
        Intent.CategoryDefault
    })]
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

            //Intent i = new Intent(AlarmClock.ActionSetAlarm);
            //i.PutExtra(AlarmClock.ExtraSkipUi, true);
            //i.PutExtra(AlarmClock.ExtraHour, 21);
            //i.PutExtra(AlarmClock.ExtraMinutes, 20);
            ////i.PutExtra(AlarmClock.ExtraDays, );
            //i.PutExtra(AlarmClock.ExtraMessage, "Good Morning");
            //StartActivity(i);    
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

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 111 && resultCode == Result.Ok)
            {
                var uri = (Android.Net.Uri)data.GetParcelableExtra(RingtoneManager.ExtraRingtonePickedUri);
                if (uri != null)
                {
                    var ringTonePath = uri.Path;
                    var ringtone = RingtoneManager.GetRingtone(this, uri);
                    var title = ringtone.GetTitle(this);
                    
                    var values = new ContentValues();
                    values.Put("ringtoneUri", ringTonePath + " " + title);
                    var sqLiteDbUtil = new SqLiteDBUtil(this);
                    var mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    var idRow = mSharedPref.GetInt("ITEM_ID", 0);

                    sqLiteDbUtil.UpdateRowAlarms(values, idRow.ToString());
                }
            }

            if (requestCode == 1111 && resultCode == Result.Ok)
            {
                var uri = (Android.Net.Uri)data.GetParcelableExtra(RingtoneManager.ExtraRingtonePickedUri);
                if (uri != null)
                {
                    var ringTonePath = uri.Path;
                    var ringtone = RingtoneManager.GetRingtone(this, uri);
                    var title = ringtone.GetTitle(this);

                    var values = new ContentValues();
                    values.Put("RingtoneUri", ringTonePath + " " + title);
                    var sqLiteDbUtil = new SqLiteDBUtil(this);
                    var mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                    var idRow = mSharedPref.GetInt("ITEM_ID", 0);

                    sqLiteDbUtil.UpdateRowScheduler(values, idRow.ToString());
                }
            }

            if (requestCode == 10485)
            {
                var task = VKSdk.OnActivityResultAsync(requestCode, (Result)resultCode, data, out var vkResult);

                if (!vkResult)
                {
                    base.OnActivityResult(requestCode, resultCode, data);
                }

                try
                {
                    var token = await task;

                    Toast toast = Toast.MakeText(this,
                        $"{Resources.GetString(Resource.String.vk_auth_success)}", ToastLength.Long);
                    toast.Show();

                    Console.WriteLine("User passed Authorization: " + token.AccessToken);
                }
                catch (VKException ex)
                {
                    Toast toast = Toast.MakeText(this,
                        $"{Resources.GetString(Resource.String.vk_auth_dismiss)}", ToastLength.Long);
                    toast.Show();

                    Console.WriteLine("User didn't pass Authorization: " + ex);

                    new FragmentUtil(this, this.SupportFragmentManager)
                        .CreateLoadView(Resource.Id.fragment_main_container, new AuthNewsFeedRepeatFragment());
                }
            }
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
                    new FragmentUtil(this, this.SupportFragmentManager)
                        .CreateLoadView(Resource.Id.fragment_main_container, new NewsFeedFragment());
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                VKSdk.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

