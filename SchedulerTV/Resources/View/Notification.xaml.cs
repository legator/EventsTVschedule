using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchedulerTV.Resources.View
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public event EventHandler Closed;

        public Notification()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            Etitle.Text = App.ViewModel.SelectedEventTV.Name;
            Edescription.Text = App.ViewModel.SelectedEventTV.Discription;
            Estart_time.Text = App.ViewModel.SelectedEventTV.Start;

            string duration = App.ViewModel.SelectedEventTV.End;
            DateTime value = new DateTime(Convert.ToInt32(App.ViewModel.SelectedEventTV.Dateint.Split('.')[2]), Convert.ToInt32(App.ViewModel.SelectedEventTV.Dateint.Split('.')[1]), Convert.ToInt32(App.ViewModel.SelectedEventTV.Dateint.Split('.')[0]));
            int hour = Convert.ToInt32(duration) / 60;
            int minutes = Convert.ToInt32(duration) - hour * 60;
            string[] start = App.ViewModel.SelectedEventTV.Start.Split(':');
            minutes = minutes + Convert.ToInt32(start[1]);
            if (minutes>=60)
            {
                hour = hour + 1;
                minutes = minutes - 60;
            }
            hour = hour + Convert.ToInt32(start[0]);
            if (hour>=24)
            {
                hour = hour - 24;
                value = value.AddDays(1);
            }
            Eend_time.Text = hour+":"+minutes;
            Eend_date.Text = value.Day + "." + value.Month + "." + value.Year;

            Estart_date.Text = App.ViewModel.SelectedEventTV.Dateint;
            Ewhere.Text = App.ViewModel.SelectedEventTV.ChannelName;
        }

        void Close()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        private void SaveEvent(object sender, RoutedEventArgs e)
        {            
            var start = new EventDateTime();
            
            start.DateTime = App.ViewModel.DateTimeConvert(Estart_date.Text,Estart_time.Text);
            start.TimeZone = "+0400";

            var end = new EventDateTime();
            end.DateTime = App.ViewModel.DateTimeConvert(Eend_date.Text, Eend_time.Text);
            end.TimeZone = "+0400";

            var ev = new Event();

            ev.End = end;
            ev.Kind = "calendar#event";
            ev.Start = start;
            ev.Summary = Etitle.Text;
            ev.Description = Edescription.Text;
            ev.Id = null;
            ev.ICalUID = null;
            ev.Location = Ewhere.Text;

            var eventReminder = new EventReminder();
            switch (Enotifytype.Text)
            {
                case "sms": eventReminder.Method = "sms"; break;
                case "email": eventReminder.Method = "email"; break;
            }
            switch (Enotifytime.Text)
            {
                case "minutes": eventReminder.Minutes = Convert.ToInt32(Enotifyvalue.Text); break;
                case "hours": eventReminder.Minutes = (Convert.ToInt32(Enotifyvalue.Text)*60); break;
                case "days": eventReminder.Minutes = (Convert.ToInt32(Enotifyvalue.Text) * 60 * 24); break;
            }
            ev.Reminders = new Event.RemindersData();
            ev.Reminders.UseDefault = false;
            ev.Reminders.Overrides = new List<EventReminder>();
            ev.Reminders.Overrides.Add(eventReminder);

            ev.OriginalStartTime = start;

            App.ViewModel.AddEvent(ev);
            Close();
        }

        private void Back2List(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
