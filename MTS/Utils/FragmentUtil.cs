using System;
using System.Collections.Generic;
using System.Text;

namespace MTS.Utils
{
    class FragmentUtil
    {
        public FragmentUtil(Android.App.Activity activity, Android.Support.V4.App.FragmentManager fragmentManager)
        {
            Activity = activity;
            FragmentManager = fragmentManager;
        }

        public Android.App.Activity Activity { set; get; }
        public Android.Support.V4.App.FragmentManager FragmentManager { set; get; }

        public void CreateLoadView(int containerViewId, Android.Support.V4.App.Fragment fragmentView)
        {
            var transaction = this.FragmentManager.BeginTransaction();

            transaction.Replace(containerViewId, fragmentView)
                .AddToBackStack($"{fragmentView.GetType().Name}")
                .Commit();
        }
    }
}
