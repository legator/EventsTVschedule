using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Samples.Helper;
using Google.Apis.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GoogleCalendarExample
{
    class Program
    {
        #region init var
        private static CalendarService service;
        private static string ClientID { get; set; }
        private static string ClientSecret { get; set; }
        private static string Key { get; set; }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Configuration for app");
            set_app_config("35229438944.apps.googleusercontent.com","8ohxvvPfOTAAAZtaIhUUH2pF","<key>");
            Console.WriteLine("Authorization");
            Auth();
            Load();
            //
            /*Console.WriteLine("Calendar list")
            foreach (CalendarListEntry item in CalendarList)
            {
                Console.WriteLine(item.Id);
            }*/
            Console.WriteLine("calendar event");
            GetCalendarEvent("faitas.oleg@gmail.com");
            Console.WriteLine(CalendarEvent.Count.ToString());
            /*
            Events r = service.Events.List("faitas.oleg@gmail.com").Fetch();
            var t = service.Events.List("faitas.oleg@gmail.com");
            int icount = r.Items.Count;
            while (true)
	        {
	            string page = r.NextPageToken;
                if (!String.IsNullOrEmpty(r.NextPageToken))
                {
                    t.PageToken = page;
                    r = t.Fetch();
                    foreach (Event item in r.Items)
                    {
                        Console.WriteLine(item.Summary);
                    }
                    Console.ReadLine();
                }
                else break;
	        }
            */
            /*
            Console.WriteLine("future event");
            GetFutureCalendarEvent("faitas.oleg@gmail.com");
            foreach (Event item in FutureCalendarEvent)
            {
                Console.WriteLine(item.Summary);
            }
            */
            Console.WriteLine("day event");
            GetDayCalendarEvent("faitas.oleg@gmail.com",DateTime.Now);
            foreach (Event item in CalendarDayEvent)
            {
                Console.WriteLine(item.Summary);
            }
            /*Console.WriteLine("All day event");
            GetAllDayCalendarEvent("faitas.oleg@gmail.com");
            foreach (Event item in CalendarAllDayEvent)
            {
                EventDateTime evd = item.Start;
                if (evd == null)
                {
                    Console.WriteLine("null");
                }
                else
                    Console.WriteLine(evd.Date);
            }*/
            /*Console.WriteLine("Get instance event");
            GetInstanceCalendarEvent("faitas.oleg@gmail.com");
            string s = "";
            foreach (Event item in InstanceCalendarEvent)
            {
                if (s != item.Summary)
                {
                    s = item.Summary;
                    Console.WriteLine(item.Summary+"-"+item.Start.Date);
                }
                
            }*/
            //

            /*Console.WriteLine("Calendar List");
            var result = service.CalendarList.List().Fetch();
            foreach (Google.Apis.Calendar.v3.Data.CalendarListEntry item in result.Items)
	        {
                Console.WriteLine(item.Id + " >>> " + item.Summary + " >>> " + item.Description);
	        }
            Console.WriteLine("Get Google Calendar event");
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("faitas.oleg@gmail.com").Items)
            {
                Console.WriteLine(item.Id + " >>> " + item.Summary + " >>> " + item.Description + " >>> " + item.Start.DateTime);
            }
            Console.WriteLine("Get future events");
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("faitas.oleg@gmail.com").Items)
            {
                if (item.Start.DateTime != null)
                {
                    if (ConvertDateTime(item.Start).CompareTo(DateTime.Now) > 0 && ConvertDateTime(item.End).CompareTo(DateTime.Now) < 0)
                    {
                        Console.WriteLine(item.Id + " >>> " + item.Summary + " >>> " + item.Description + " >>> " + item.Start.DateTime);
                    }
                }
            }
            Console.WriteLine("Get instance events");
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("faitas.oleg@gmail.com").Items)
            {
                Events res = GetInstanceEvent("faitas.oleg@gmail.com", item.Id);
                try
                {
                    if (res.Items.Count>1)
                    {
                        Console.WriteLine(item.Summary + " >>> " + item.Description+" >>>"+res.Items.Count+res.Items[res.Items.Count-1].End.DateTime);
                    }
                }catch(Exception){}
            }*/
            Console.WriteLine("\nFinish!!!");
            Console.ReadLine();
        }

        private static void set_app_config(string cid, string csecret, string key)
        {
            ClientID = cid;
            ClientSecret = csecret;
            Key = key;
        }

        private static void Auth()
        {
            service = new CalendarService(CreateAuthenticator());
        }

        private static IAuthenticator CreateAuthenticator()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientID;
            provider.ClientSecret = ClientSecret;
            return new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication);
        }

        private static IAuthorizationState GetAuthentication(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "google.legAToR.GcalendarExample.dotnet.calendar";
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

        private static Google.Apis.Calendar.v3.Data.Events GetEvents(string calendarid)
        {
            /*
            var result = service.Events.List(calendarid).Fetch();
            return result;
            */
            Events es = new Events();
            var result = service.Events.List(calendarid).Fetch();
            var listrequest = service.Events.List(calendarid);
            es = result;

            while (true)
            {
                string page = result.NextPageToken;
                if (!String.IsNullOrEmpty(result.NextPageToken))
                {
                    listrequest.PageToken = page;
                    result = listrequest.Fetch();
                    foreach (Event item in result.Items)
                    {
                        es.Items.Add(item);
                    }
                }
                else break;
            }
            return es;
        }

        private static DateTime ConvertDateTime(EventDateTime evtime)
        {
            string d0 = "";
            for (int i = 0; i < 10; i++)
            {
                d0 = d0 + evtime.DateTime[i];
            }
            string d1 = "";
            for (int i = 11; i < evtime.DateTime.Length; i++)
            {
                d1 = d1 + evtime.DateTime[i];
            }
            DateTime ed = new DateTime(Convert.ToInt32(d0.Split('-')[0]), Convert.ToInt32(d0.Split('-')[1]), Convert.ToInt32(d0.Split('-')[2]),Convert.ToInt32(d1.Split('+')[0].Split(':')[0]),Convert.ToInt32(d1.Split('+')[0].Split(':')[1]),Convert.ToInt32(d1.Split('+')[0].Split(':')[2]));
            return ed;
        }

        private static void DeleteEvent(string calendarid, string eventid)
        {
            string result = service.Events.Delete(calendarid, eventid).Fetch();
        }

        public static void CreateEvent(string calendarId, string descr,string sum, string loc, EventDateTime end,EventDateTime start, Google.Apis.Calendar.v3.Data.Event.RemindersData rem )
        {
            Event ev = new Event();
            ev.Description = descr;
            ev.End = end;
            ev.Start = start;
            ev.Location = loc;
            ev.Summary = sum;
            ev.Reminders = rem;
            Event createdEvent = service.Events.Insert(ev, calendarId).Fetch();
        }

        private static void UpdateEvent(string calendarid, string eventid)
        {
            Event ev = service.Events.Get(calendarid, eventid).Fetch();
            Event updatedEvent = service.Events.Update(ev, calendarid, eventid).Fetch();
        }

        private static Events GetInstanceEvent(string calendarid,string eventid)
        {
            var result = service.Events.Instances(calendarid, eventid).Fetch();
            return result;
        }


        public static IList<CalendarListEntry> CalendarList { get; set; }
        public static IList<Event> CalendarEvent { get; set; }
        public static IList<Event> CalendarAllDayEvent { get; set; }
        public static List<Event> FutureCalendarEvent { get; set; }
        public static List<Event> CalendarDayEvent { get; set; }
        public static List<Event> InstanceCalendarEvent { get; set; }
        public static List<Event> FutureInstanceCalendarEvent { get; set; }

        public static void Load()
        {
            CalendarList = service.CalendarList.List().Fetch().Items;
        }

        public static void GetCalendarEvent(string calendarid)
        {
            CalendarEvent = GetEvents(calendarid).Items;
        }

        public static void GetAllDayCalendarEvent(string calendarid)
        {
            List<Event> evt = new List<Event>();
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents(calendarid).Items)
            {
                if (item.Start.DateTime == null)
                {
                    evt.Add(item);
                }
            }
            CalendarAllDayEvent = evt;
        }

        public static void GetFutureCalendarEvent(string calendarid)
        {
            List<Event> fevent = new List<Event>();
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents(calendarid).Items)
            {
                if (item.Start.DateTime != null)
                {
                    //TimeSpan diff = date2.Subtract(date1);
                    if (ConvertDateTime(item.Start).CompareTo(DateTime.Now) > 0 && ConvertDateTime(item.End).CompareTo(DateTime.Now) < 0)
                    {
                        fevent.Add(item);
                    }
                }
            }
            GetFutureInstanceCalendarEvent(calendarid);
            foreach (Event item in FutureInstanceCalendarEvent)
            {
                fevent.Add(item);
            }
            FutureCalendarEvent = fevent;
        }

        public static void GetDayCalendarEvent(string calendarid, DateTime dt)
        {
            List<Event> evt = new List<Event>();
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents(calendarid).Items)
            {
                if (item.Start.DateTime != null)
                {
                    if (ConvertDateTime(item.Start).ToShortDateString() == dt.ToShortDateString())
                    {
                        evt.Add(item);
                    }
                }
            }
            CalendarDayEvent = evt;
        }

        public static void GetInstanceCalendarEvent(string calendarid)
        {
            List<Event> evt = new List<Event>();
            foreach (Event item in GetEvents(calendarid).Items)
            {
                Events res = GetInstanceEvent(calendarid, item.Id);
                try
                {
                    foreach (Event it in res.Items)
                    {
                        evt.Add(it);
                    }
                }
                catch (Exception) { }
            }
            InstanceCalendarEvent = evt;
        }

        public static void GetFutureInstanceCalendarEvent(string calendarid)
        {
            List<Event> evt = new List<Event>();
            GetInstanceCalendarEvent(calendarid);
            foreach (Event item in InstanceCalendarEvent)
            {
                if (item.Start.DateTime != null)
                {
                    if (ConvertDateTime(item.Start).CompareTo(DateTime.Now) > 0 && ConvertDateTime(item.End).CompareTo(DateTime.Now) < 0)
                    {
                        evt.Add(item);
                    }
                }
            }
            FutureInstanceCalendarEvent = evt;
        }
    }
}
