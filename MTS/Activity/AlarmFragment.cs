using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            _alarmItems = new List<AlarmItem>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_alarm, container, false);

            _listView = _view.FindViewById<Android.Support.V7.Widget.ListViewCompat>(Resource.Id.list_view_alarm);

            SetDataToAdapter();

            return _view;
        }

        private async void SetDataToAdapter()
        {
            await Task.Run((() =>
            {
                this.Activity.RunOnUiThread(() =>
                {
                    _alarmItems.Add(new AlarmItem()
                    {
                        Checked = true,
                        Time = DateTime.Now,
                        Id = 1
                    });
                    _alarmItems.Add(new AlarmItem()
                    {
                        Checked = false,
                        Time = DateTime.Now,
                        Id = 2
                    });
                    _listView.Adapter = new AlarmAdapter(this.Activity, _alarmItems);
                });
            }));
        }
    }
}