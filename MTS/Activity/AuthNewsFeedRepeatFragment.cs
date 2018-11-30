using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MTS.Utils;
using VKontakte;

namespace MTS.Activity
{
    public class AuthNewsFeedRepeatFragment : Android.Support.V4.App.Fragment
    {
        private View _view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_newsfeed_dismiss, container, false);
            var textView = _view.FindViewById<TextView>(Resource.Id.text_repeat_auth);
            textView.Click += TextView_Click;

            return _view;
        }

        private void TextView_Click(object sender, EventArgs e)
        {
            new FragmentUtil(this.Activity, this.Activity.SupportFragmentManager)
                .CreateLoadView(Resource.Id.fragment_main_container, new NewsFeedFragment());
        }
    }
}