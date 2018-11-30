using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database.Sqlite;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Util;
using MTS.Activity;
using MTS.Entity;
using MTS.Utils;
using Uri = Android.Net.Uri;

namespace MTS.Adapters
{
    class AlarmAdapter : BaseAdapter<AlarmItem>
    {
        private Android.App.Activity _context;
        private List<AlarmItem> _items;
        private TextView _textClock, _textView, _textRingtone;
        private ImageButton _imageButton, _btnSave;
        private SwitchCompat _switchCompat;
        private TextInputEditText _inputEditText;
        private SqLiteDBUtil _sqLiteDbUtil;
        private CheckBox[] _daysCheckboxes;
        private ArrayList _alarmDays;

        public override int Count => Items.Count;
        public Android.App.Activity Context { get => _context; set => _context = value; }
        public List<AlarmItem> Items { get => _items; set => _items = value; }

        public AlarmAdapter(Android.App.Activity context)
        {
            this.Context = context;

            _sqLiteDbUtil = new SqLiteDBUtil(this.Context);
        }

        public AlarmAdapter(Android.App.Activity context, List<AlarmItem> items)
        {
            Context = context;
            Items = items;

            _sqLiteDbUtil = new SqLiteDBUtil(this.Context);
            _alarmDays = new ArrayList();
        }

        public override AlarmItem this[int position] => Items[position];

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = Items[position];

            var view = convertView ?? Context.LayoutInflater.Inflate(Resource.Layout.item_list_alarm, null);

            _textClock = view.FindViewById<TextView>(Resource.Id.text_time_alarm);
            _textClock.Text = item.Time.ToString("HH:mm");
            _textClock.Tag = item.Id;

            _inputEditText = view.FindViewById<TextInputEditText>(Resource.Id.text_input_name_alarm);
            _inputEditText.Text = item.NameAlarm;
            _inputEditText.Tag = item.Id;
            _inputEditText.AfterTextChanged += _inputEditText_AfterTextChanged;

            _switchCompat = view.FindViewById<SwitchCompat>(Resource.Id.toggle_btn_alarm);
            _switchCompat.Checked = item.Checked;
            _switchCompat.Tag = item.Id;
            _switchCompat.CheckedChange += _switchCompat_CheckedChange;

            _textView = view.FindViewById<TextView>(Resource.Id.text_day_alarm);
            _textView.Text = "";

            _imageButton = view.FindViewById<ImageButton>(Resource.Id.btn_delete_alarm);
            _imageButton.Tag = item.Id;
            _imageButton.Click += _imageButton_Click;

            _btnSave = view.FindViewById<ImageButton>(Resource.Id.btn_save_alarm);
            _btnSave.Tag = item.Id;
            _btnSave.Click += _btnSave_Click;

            _textRingtone = view.FindViewById<TextView>(Resource.Id.text_choice_ringtone);
            _textRingtone.Tag = item.Id;
            _textRingtone.Click += _textRingtone_Click;
            if (item.RingtoneUri !=null)
            {
                _textRingtone.Text = null;

                var uriData = item.RingtoneUri.Split(' ');
                for (int i = 1; i < uriData.Length; i++)
                {
                    _textRingtone.Text += uriData[i] + " ";
                }
            }

            _daysCheckboxes = new CheckBox[7];
            _daysCheckboxes[0] = view.FindViewById<CheckBox>(Resource.Id.checkBox_mon);
            _daysCheckboxes[1] = view.FindViewById<CheckBox>(Resource.Id.checkBox_tue);
            _daysCheckboxes[2] = view.FindViewById<CheckBox>(Resource.Id.checkBox_wed);
            _daysCheckboxes[3] = view.FindViewById<CheckBox>(Resource.Id.checkBox_thu);
            _daysCheckboxes[4] = view.FindViewById<CheckBox>(Resource.Id.checkBox_fri);
            _daysCheckboxes[5] = view.FindViewById<CheckBox>(Resource.Id.checkBox_sat);
            _daysCheckboxes[6] = view.FindViewById<CheckBox>(Resource.Id.checkBox_sun);

            for (int i = 0; i < _daysCheckboxes.Length; i++)
            {
                _daysCheckboxes[i].CheckedChange += OnCheckedChange;
                _daysCheckboxes[i].Tag = (i + 1);
            }

            if (item.DaysAlarm != null)
            {
                var days = item.DaysAlarm.Split(' ');
                for (int i = 0; i < days.Length - 1; i++)
                {
                    var checkBoxIndex = Convert.ToInt32(days[i]) - 2;

                    _daysCheckboxes[checkBoxIndex].Checked = true;
                }
            }

            this.NotifyDataSetChanged();
            return view;
        }

        private void OnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var day = (CheckBox) sender;

            _alarmDays.Add(Convert.ToInt32(day.Tag) + 1);
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;
            var id = Convert.ToInt32(button.Tag);

            var item = new AlarmItem();
            foreach (var t in Items)
            {
                if (t.Id == id)
                {
                    item = t;
                }
            }

            Intent intent = new Intent(AlarmClock.ActionSetAlarm);
            intent.PutExtra(AlarmClock.ExtraSkipUi, true);
            intent.PutExtra(AlarmClock.ExtraHour, item.Time.Hour);
            intent.PutExtra(AlarmClock.ExtraMinutes, item.Time.Minute);
            intent.PutExtra(AlarmClock.ExtraDays, _alarmDays);
            intent.PutExtra(AlarmClock.ExtraMessage, item.NameAlarm);

            var uri = Uri.Parse(item.RingtoneUri.Split(' ').ToArray()[0]);

            if (item.RingtoneUri != null)
            {
                intent.PutExtra(AlarmClock.ExtraRingtone, uri.EncodedSchemeSpecificPart);
            }
            Context.StartActivity(intent);

            
            var days = "";
            for (int i = 0; i < _alarmDays.Size(); i++)
            {
                days += _alarmDays.Get(i).ToString() + " ";
            }

            ContentValues values = new ContentValues();
            values.Put("daysAlarm", days);

            foreach (var itemL in Items)
            {
                if (itemL.Id.ToString() != id.ToString()) continue;
                itemL.DaysAlarm = days;
            }

            _sqLiteDbUtil.UpdateRowAlarms(values, id.ToString());

            values.Put("alarmStatus", Convert.ToInt32(true));

            _sqLiteDbUtil.UpdateRowAlarms(values, id.ToString());

            foreach (var itemSw in Items)
            {
                if (itemSw.Id.ToString() != id.ToString()) continue;
                itemSw.Checked = true;
            }

            this.NotifyDataSetChanged();
        }

        private void _textRingtone_Click(object sender, EventArgs e)
        {
            var text = (TextView) sender;

            Intent intent = new Intent(RingtoneManager.ActionRingtonePicker);
            intent.PutExtra(RingtoneManager.ExtraRingtoneTitle, $"{_context.Resources.GetString(Resource.String.choice_ringrone)}");
            intent.PutExtra(RingtoneManager.ExtraRingtoneShowSilent, false);
            intent.PutExtra(RingtoneManager.ExtraRingtoneShowDefault, true);
            intent.PutExtra(RingtoneManager.ExtraRingtoneType, (int)RingtoneType.Alarm);

            var mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(this._context);
            var mPrefsEditor = mSharedPrefs.Edit();
            mPrefsEditor.PutInt("ITEM_ID", Convert.ToInt32(text.Tag));
            mPrefsEditor.Commit();

            _context.StartActivityForResult(intent, 111);
        }

        private void _inputEditText_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            var inputText = (TextInputEditText) sender;
            
            ContentValues values = new ContentValues();
            values.Put("nameAlarm", inputText.Text);

            _sqLiteDbUtil.UpdateRowAlarms(values, inputText.Tag.ToString());

            foreach (var item in Items)
            {
                if (item.Id.ToString() != inputText.Tag.ToString()) continue;
                item.NameAlarm = inputText.Text;
            }

            this.NotifyDataSetChanged();
        }

        private void _imageButton_Click(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;
            
            _sqLiteDbUtil.DeleteRowAlarms(button.Tag.ToString());

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Id.ToString() != button.Tag.ToString()) continue;
                Items.RemoveAt(i);
            }

            this.NotifyDataSetChanged();

            Intent intent = new Intent(AlarmClock.ActionDismissAlarm);
            intent.PutExtra(AlarmClock.ExtraAlarmSearchMode, AlarmClock.AlarmSearchModeLabel);
            Context.StartActivity(intent);
        }

        private void _switchCompat_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var switchCompat = (SwitchCompat) sender;
            
            ContentValues values = new ContentValues();
            values.Put("alarmStatus", Convert.ToInt32(switchCompat.Checked));

            _sqLiteDbUtil.UpdateRowAlarms(values, switchCompat.Tag.ToString());

            foreach (var item in Items)
            {
                if (item.Id.ToString() != switchCompat.Tag.ToString()) continue;
                item.Checked = switchCompat.Checked;
            }

            this.NotifyDataSetChanged();
        }

        ~AlarmAdapter()
        {
            _sqLiteDbUtil.Database.Close();
        }
    }
}

