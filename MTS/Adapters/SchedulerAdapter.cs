using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Util;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Java.Util;
using MTS.Entity;
using MTS.Utils;

namespace MTS.Adapters
{
    class SchedulerAdapter : BaseAdapter<SchedulerItem>
    {
        private Android.App.Activity _context;
        private TextView _dateCreateTextView, 
            _timeCreateTextView,
            _ringtoneTextView;

        private ImageButton _deleteButton,
            _saveButton;

        private EditText _descriptionEditText;
        private TextInputEditText _titleInputEditText;
        private List<SchedulerItem> _schedulerItems;
        private SqLiteDBUtil _sqLiteDbUtil;

        public SchedulerAdapter(Android.App.Activity context)
        {
            this._context = context;
            _sqLiteDbUtil = new SqLiteDBUtil(this._context);
        }

        public SchedulerAdapter(Android.App.Activity context, List<SchedulerItem> schedulerItems) : this(context)
        {
            _schedulerItems = schedulerItems;
            this._context = context;
        }

        public override SchedulerItem this[int position] => _schedulerItems[position];

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
            var item = _schedulerItems[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.item_list_scheduler, null);

            _titleInputEditText = view.FindViewById<TextInputEditText>(Resource.Id.text_scheduler_title);
            _titleInputEditText.Tag = item.Id;
            _titleInputEditText.Text = item.SchedulerTitle;
            _titleInputEditText.AfterTextChanged += _titleInputEditText_AfterTextChanged;

            _dateCreateTextView = view.FindViewById<TextView>(Resource.Id.text_scheduler_date_create);
            _dateCreateTextView.Tag = item.Id;
            _dateCreateTextView.Text = item.Time.ToString("yyyy MMMM dd");

            _timeCreateTextView = view.FindViewById<TextView>(Resource.Id.text_scheduler_time_create);
            _timeCreateTextView.Tag = item.Id;
            _timeCreateTextView.Text = item.Time.ToString("HH:mm");

            _ringtoneTextView = view.FindViewById<TextView>(Resource.Id.text_choice_ringtone_scheduler);
            _ringtoneTextView.Tag = item.Id;
            if (item.RingtoneUri != null)
            {
                _ringtoneTextView.Text = null;
                var uriData = item.RingtoneUri.Split(' ');
                for (int i = 1; i < uriData.Length; i++)
                {
                    _ringtoneTextView.Text += uriData[i] + " ";
                }
            }
            _ringtoneTextView.Click += _ringtoneTextView_Click;

            _descriptionEditText = view.FindViewById<EditText>(Resource.Id.edit_text_scheduler);
            _descriptionEditText.Tag = item.Id;
            _descriptionEditText.Text = item.SchedulerDescription;
            _descriptionEditText.AfterTextChanged += _descriptionEditText_AfterTextChanged;

            _deleteButton = view.FindViewById<ImageButton>(Resource.Id.btn_delete_scheduler);
            _deleteButton.Tag = item.Id;
            _deleteButton.Click += _deleteButton_Click;

            _saveButton = view.FindViewById<ImageButton>(Resource.Id.btn_save_scheduler);
            _saveButton.Tag = item.Id;
            _saveButton.Click += _saveButton_Click;

            this.NotifyDataSetChanged();
            return view;
        }

        private void _titleInputEditText_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            var editText = (TextInputEditText) sender;
            var id = Convert.ToInt32(editText.Tag);

            for (int i = 0; i < _schedulerItems.Count; i++)
            {
                if (_schedulerItems[i].Id == id)
                {
                    _schedulerItems[i].SchedulerTitle = editText.Text;
                }
            }
        }

        private void _descriptionEditText_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            var editText = (EditText) sender;
            var id = Convert.ToInt32(editText.Tag);

            for (int i = 0; i < _schedulerItems.Count; i++)
            {
                if (_schedulerItems[i].Id == id)
                {
                    _schedulerItems[i].SchedulerDescription = editText.Text;
                }
            }
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;
            var id = Convert.ToInt32(button.Tag);

            _sqLiteDbUtil.DeleteRowScheduler(id.ToString());

            var itemIndex = _schedulerItems.FindIndex(x => x.Id == id);

            if (itemIndex != -1)
            {
                _schedulerItems.RemoveAt(itemIndex);
            }

            this.NotifyDataSetChanged();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            var button = (ImageButton) sender;

            var id = Convert.ToInt32(button.Tag);
            var selectedItem = _schedulerItems.Where(x => x.Id == id).ToArray().First();

            var values = new ContentValues();
            values.Put("schedulerTitle", selectedItem.SchedulerTitle);
            values.Put("SchedulerDescription", selectedItem.SchedulerDescription);
            values.Put("Time", selectedItem.Time.ToString());
            values.Put("RingtoneUri", selectedItem.RingtoneUri);

            _sqLiteDbUtil.UpdateRowScheduler(values,id.ToString());

            this.NotifyDataSetChanged();

            var shedulerTask = new SchedulerReceiver();
            TelephonyManager manager = (TelephonyManager)this._context.GetSystemService(Context.TelephonyService);

            var tz = Java.Util.TimeZone.GetTimeZone("GMT+03:00");
            var cal = Calendar.GetInstance(tz);
            cal.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            cal.Set(selectedItem.Time.Year, selectedItem.Time.Month, selectedItem.Time.Day, selectedItem.Time.Hour, selectedItem.Time.Minute);
            

            Calendar calendar = Calendar.GetInstance(tz);
           // calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            calendar.Add(CalendarField.Month, Math.Abs(selectedItem.Time.Month - DateTime.Today.Month));
            calendar.Add(CalendarField.Hour, Math.Abs(selectedItem.Time.Hour - DateTime.Today.Hour));
            calendar.Add(CalendarField.Minute, Math.Abs(selectedItem.Time.Minute - DateTime.Today.Minute));
            calendar.Add(CalendarField.Second, 30);
            calendar.Set(CalendarField.Millisecond, 0);

            Intent intent = new Intent(this._context, typeof(SchedulerReceiver));
            intent.PutExtra("SchedulerDescription", selectedItem.SchedulerDescription);
            intent.PutExtra("SchedulerTitle", selectedItem.SchedulerTitle);
            if (selectedItem.RingtoneUri != null)
            {
                intent.PutExtra("RingtoneUri", selectedItem.RingtoneUri.Split(' ').ToArray()[0]);
            }

            
            PendingIntent pi = PendingIntent.GetBroadcast(this._context, selectedItem.Id, intent, PendingIntentFlags.UpdateCurrent);

            SimpleDateFormat dateFormat = new SimpleDateFormat("EE mm dd HH:mm:ss 'GMT'Z yyyy");
            string a = dateFormat.Format(cal.Time);

            AlarmManager am = (AlarmManager)_context.GetSystemService(Context.AlarmService);
            am.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + calendar.TimeInMillis, pi);
            //shedulerTask.SetOnetimeTimer(this._context, selectedItem.Time.Ticks, selectedItem);
        }

        private void _ringtoneTextView_Click(object sender, EventArgs e)
        {
            var text = (TextView) sender;
            var id = Convert.ToInt32(text.Tag);

            Intent intent = new Intent(RingtoneManager.ActionRingtonePicker);
            intent.PutExtra(RingtoneManager.ExtraRingtoneTitle, "Выберите рингтон:");
            intent.PutExtra(RingtoneManager.ExtraRingtoneShowSilent, false);
            intent.PutExtra(RingtoneManager.ExtraRingtoneShowDefault, true);
            intent.PutExtra(RingtoneManager.ExtraRingtoneType, (int)RingtoneType.Notification);

            var mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(this._context);
            var mPrefsEditor = mSharedPrefs.Edit();
            mPrefsEditor.PutInt("ITEM_ID", id);
            mPrefsEditor.Commit();

            _context.StartActivityForResult(intent, 1111);
        }

        public override int Count => _schedulerItems.Count;
    }
}