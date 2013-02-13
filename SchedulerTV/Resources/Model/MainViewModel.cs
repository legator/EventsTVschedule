using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Samples.Helper;
using Google.Apis.Util;
using NLog;
using SchedulerTV.Resources.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.ComponentModel;
using System.Windows.Data;
using Google.Apis.Calendar.v3.Data;
using ItemClass;
using ntvplus;

namespace SchedulerTV.Resources.Model
{
    public class MainViewModel:ViewModelBase
    {
        private readonly static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public KeyWords KeyList { get; set; }
        public string Root { get; set; }
        private parsetv ntv { get; set; }
        private List<DateItem> date_data { get; set; }
        private List<ChannelItem> channel_data { get; set; }
        public CalendarService service { get; set; }
        public ProgramItem SelectedEventTV { get; set; }

        private bool isupdate;
        public bool IsUpdate
        {
            get
            {
                bool flag = this.isupdate;
                return flag;
            }
            set
            {
                bool flag = this.isupdate != value;
                if (flag)
                {
                    this.isupdate = value;
                    base.RaisePropertyChanged("IsUpdate");
                }
            }
        }

        private bool Islogged;
        public bool IsLogged
        {
            get
            {
                bool flag = this.Islogged;
                return flag;
            }
            set
            {
                bool flag = this.Islogged != value;
                if (flag)
                {
                    this.Islogged = value;
                    base.RaisePropertyChanged("IsLogged");
                }
            }
        }

        private bool Issearch;
        public bool IsSearch
        {
            get
            {
                bool flag = this.Issearch;
                return flag;
            }
            set
            {
                bool flag = this.Issearch != value;
                if (flag)
                {
                    this.Issearch = value;
                    base.RaisePropertyChanged("IsSearch");
                }
            }
        }

        private bool Istopmost = true;
        public bool IsTopMost
        {
            get
            {
                bool flag = this.Istopmost;
                return flag;
            }
            set
            {
                bool flag = this.Istopmost != value;
                if (flag)
                {
                    this.Istopmost = value;
                    base.RaisePropertyChanged("IsTopMost");
                }
            }
        }

        private ObservableCollection<Key> searchkey;
        public ObservableCollection<Key> SearchKey
        {
            get { return searchkey; }
            set
            {
                if (searchkey == value)
                    return;
                searchkey = value;
                keyselectedindex = -1;
                RaisePropertyChanged("SearchKey");
            }
        }

        private ObservableCollection<string> chlist;
        public ObservableCollection<string> ChList
        {
            get { return chlist; }
            set
            {
                if (chlist == value)
                    return;
                chlist = value;
                RaisePropertyChanged("ChList");
            }
        }

        private ObservableCollection<string> itemlist;
        public ObservableCollection<string> ItemList
        {
            get { return itemlist; }
            set
            {
                if (itemlist == value)
                    return;
                itemlist = value;
                RaisePropertyChanged("ItemList");
            }
        }

        private ObservableCollection<ProgramItem> actionlist;
        public ObservableCollection<ProgramItem> ActionList
        {
            get { return actionlist; }
            set
            {
                if (actionlist == value)
                    return;
                actionlist = value;
                RaisePropertyChanged("ActionList");
            }
        }

        private ObservableCollection<ProgramItem> searchactionlist;
        public ObservableCollection<ProgramItem> SearchActionList
        {
            get { return searchactionlist; }
            set
            {
                if (searchactionlist == value)
                    return;
                searchactionlist = value;
                RaisePropertyChanged("SearchActionList");
            }
        }

        public Raketa RaketaEPG { get; set; }

        private int keyselectedindex = -1;
        public int KeySelectedIndex
        {
            get { return keyselectedindex; }
            set
            {
                if (keyselectedindex == value)
                    return;
                keyselectedindex = value;
                RaisePropertyChanged("KeySelectedIndex");
            }
        }

        public void LoadSearchKey()
        {
            KeyList = null;
            StreamReader sr = new StreamReader(Root + "/Resources/Data/Key.json");
            KeyList = JsonConvert.DeserializeObject<KeyWords>(sr.ReadToEnd());
            sr.Close();
            if (KeyList!=null)
            {
                SearchKey = new ObservableCollection<Key>(KeyList.Keys);
            }
            
        }

        public void SaveSearchKey(Key k)
        {
            if (KeyList == null)
            {
                KeyWords kw = new KeyWords();
                List<Key> kl = new List<Key>();
                kl.Add(k);
                kw.Keys = kl;
                KeyList = kw;
            }
            else
            {
                bool ispresent = false;
                foreach (Key item in KeyList.Keys)
                {
                    if (item.Category == k.Category && item.KeyName == k.KeyName)
                    {
                        ispresent = true;
                        break;
                    }
                }
                if (!ispresent)
                {
                    KeyList.Keys.Add(k);
                }
                else throw new System.InvalidOperationException("Double key");
            }
            SearchKey = new ObservableCollection<Key>(KeyList.Keys);
            SaveKey();
        }

        private void SaveKey()
        {
            StreamWriter sw = new StreamWriter(Root+"/Resources/Data/Key.json");
            string json = JsonConvert.SerializeObject(KeyList);
            sw.Write(json);
            sw.Close();
            LoadSearchKey();
        }

        public void DeleteSearchKey()
        {
            KeyList.Keys.Clear();
            foreach (Key item in searchkey)
            {
                KeyList.Keys.Add(item);
            }
            SaveKey();
        }

        public void LoadDataTV()
        {
            if (SchedulerTV.Properties.Settings.Default.TVSource == "ntvplus")
            {
                NTVplus();
            }
            else if (SchedulerTV.Properties.Settings.Default.TVSource == "teleguide")
            {
                TeleGuide();
            }
            else if (SchedulerTV.Properties.Settings.Default.TVSource == "raketa-tv")
            {
                RaketaTV();
            }
        }

        private void NTVplus()
        {
            ntv = new parsetv();
            channel_data = ntv.channel_load();
            date_data = ntv.date_load();
            List<string> ch = new List<string>();
            string cat_name = "";
            foreach (ChannelItem item in channel_data)
            {
                if (cat_name!=item.Category)
                {
                    ch.Add(item.Category);
                    cat_name = item.Category;
                }
                
            }
            ChList = null;
            ChList = new ObservableCollection<string>(ch);
        }

        public void LoadChannelByCategory(string name)
        {
            List<string> cl = new List<string>();
            foreach (ChannelItem item in channel_data)
            {
                if (item.Category==name)
                {
                    cl.Add(item.Name);
                }
            }
            ItemList = new ObservableCollection<string>(cl);
        }

        public void LoadActionTVByChannel(string name)
        {
            AsyncThread.InvokeAsync(() =>
            {
                string index = "", chname = "";
                foreach (ChannelItem item in channel_data)
                {
                    if (item.Name == name)
                    {
                        index = item.Id;
                        chname = item.Name;
                        break;
                    }
                }
                List<ProgramItem> plist = new List<ProgramItem>();
                for (int j = 0; j < date_data.Count; j++)
                {
                    List<ProgramItem> its = ntv.ActionByChDt(index, date_data[j].DateInt);
                    foreach (ProgramItem item in its)
                    {
                        item.ChannelName = chname;
                        plist.Add(item);
                    }
                }
                ActionList = new ObservableCollection<ProgramItem>(plist);
            });
        }

        public void SearchActionTVByKeyWords()
        {
            AsyncThread.InvokeAsync(()=>
            {
            List<ProgramItem> saction = new List<ProgramItem>();
            if (KeyList.Keys.Count != 0)
            {
                for (int i = 0; i < channel_data.Count; i++)
                {
                    for (int j = 0; j < date_data.Count; j++)
                    {
                        for (int k = 0; k < KeyList.Keys.Count; k++)
                        {
                            if (KeyList.Keys[k].Category=="Все" || KeyList.Keys[k].Category==channel_data[i].Category)
                            {
                                List<ProgramItem> its = ntv.ActionByChDt(channel_data[i].Id, date_data[j].DateInt);
                                foreach (ProgramItem item in its)
                                {
                                    Parsing parse = new Parsing();
                                    if (parse.Search(KeyList.Keys[k].KeyName, item.Name))
                                    {
                                        item.ChannelName = channel_data[i].Name;
                                        saction.Add(item);
                                        SearchActionList = new ObservableCollection<ProgramItem>(saction);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else IsUpdate = false;
            });
        }

        private void TeleGuide()
        {

        }

        private void RaketaTV()
        {
            LoadRaketaData();
        }

        private void LoadRaketaData()
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(@"http://raketa-tv.com/player/JSON/program_list.json");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            RaketaEPG = JsonConvert.DeserializeObject<Raketa>(result);
        }

        public void Login()
        {
            Auth();
        }

        void Auth()
        {
            AsyncThread.InvokeAsync(() =>
                                        {
                                            service = new CalendarService(CreateAuthenticator());
                                        });
            //service.Key = "AIzaSyDKCcmQSe_Dxd8iyZiPMbbs2syHwaIi00A";
            IsLogged = SchedulerTV.Properties.Settings.Default.Logged = true;
            SchedulerTV.Properties.Settings.Default.Save();
        }

        private static IAuthenticator CreateAuthenticator()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "35229438944.apps.googleusercontent.com";
            provider.ClientSecret = "8ohxvvPfOTAAAZtaIhUUH2pF";
            return new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication);
        }

        private static IAuthorizationState GetAuthentication(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "google.legAToR.DayScheduler.daycalendar";
            const string KEY = "y},drdzf11x9;87";
            string scope = CalendarService.Scopes.Calendar.GetStringValue();

            // Check if there is a cached refresh token available.
            IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
            if (state != null)
            {
                try
                {
                    client.RefreshToken(state);
                    return state; // Yes - we are done.
                }
                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
                {

                }
            }
            // Retrieve the authorization from the user.
            state = AuthorizationMgr.RequestNativeAuthorization(client, scope);
            AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
            return state;
        }

        public void DeleteEvent(string calendarid, string eventid)
        {
            string result = service.Events.Delete(calendarid, eventid).Fetch();
        }

        public void AddEvent(Event ev)
        {
            var request = service.Events.Insert(ev, "primary").Fetch();
        }

        public void AddEvent(ProgramItem item)
        {
            const string GoogleDateFormatString = "yyyy-MM-ddTHH:mm:00Z";
            //service.Key = Key;

            var list = service.Events.List("primary");
            var fetch = list.Fetch();

            var start = new EventDateTime();
            start.DateTime = "2013-03-05T10:30:00Z";
            start.TimeZone = "+0000";

            var end = new EventDateTime();
            end.DateTime = "2013-03-05T10:35:00Z";
            end.TimeZone = "+0000";

            var e = new Event();

            e.End = end;
            e.Kind = "calendar#event";
            e.Start = start;
            e.Summary = "Summary";
            e.Id = null;
            e.ICalUID = null;
            e.Location = "";

            var eventReminder = new EventReminder();
            eventReminder.Method = "sms";
            eventReminder.Minutes = 15;

            e.Reminders = new Event.RemindersData();
            e.Reminders.UseDefault = false;
            e.Reminders.Overrides = new List<EventReminder>();
            e.Reminders.Overrides.Add(eventReminder);

            // Recurrence:
            var recur = "RRULE:FREQ=DAILY;COUNT=20;INTERVAL=1;WKST=SU";

            e.Recurrence = new List<string>();
            e.Recurrence.Add(recur);
            e.RecurringEventId = "12345";
            e.OriginalStartTime = start;

            AddEvent(e);
        }

        public string DateTimeConvert(string d, string t)
        {
            //"yyyy-MM-ddTHH:mm:00Z"
            string dt = "";
            //3/31/2010
            string[] ds = d.Split('.');
            dt = ds[2] + "-" + ds[1] + "-" + ds[0] + "T" + t + ":00Z";
            return dt;
        }
    }
}
