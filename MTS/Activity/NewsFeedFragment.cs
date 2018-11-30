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
using VKontakte;
using WebView = Android.Webkit.WebView;

namespace MTS.Activity
{
    public class NewsFeedFragment : Android.Support.V4.App.Fragment
    {
        private static string[] MyScopes = {
            VKScope.Friends,
            VKScope.Wall,
            VKScope.Photos,
            VKScope.Nohttps,
            VKScope.Messages,
            VKScope.Docs
        };

        private View _view;
        private bool isResumed = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            VKSdk.CustomInitialize(this.Activity, this.Activity
                .Resources
                .GetInteger(Resource.Integer.com_vk_sdk_AppId), $"{this.Activity.Resources.GetString(Resource.String.vk_auth_verApi)}");

            VKSdk.WakeUpSession(this.Activity,
                response => {
                    if (isResumed)
                    {
                        if (response == VKSdk.LoginState.LoggedOut)
                        {
                            ShowLogin();
                        }
                    }
                },
                error => {
                    Console.WriteLine("WakeUpSession error: " + error);
                });
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_newsfeed, container, false);
            var webView = _view.FindViewById<WebView>(Resource.Id.webView_newsfeed);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.SaveFormData = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.BuiltInZoomControls = true;
            webView.LoadUrl("https://zen.yandex.ru/");

            return _view;
        }

        private void ShowLogin()
        {
            VKSdk.Login(this.Activity, MyScopes);
        }

        public override void OnResume()
        {
            base.OnResume();

            isResumed = true;

            if (!VKSdk.IsLoggedIn)
            {
                ShowLogin();
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            isResumed = false;
        }
    }
}