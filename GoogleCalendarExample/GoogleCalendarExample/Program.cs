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
            set_app_config("<id>", "<secret>", "<key>");
            Console.WriteLine("Authorization");
            Auth();
            Console.WriteLine("Calendar List");
            var result = service.CalendarList.List().Fetch();
            foreach (Google.Apis.Calendar.v3.Data.CalendarListEntry item in result.Items)
	        {
                Console.WriteLine(item.Id + " >>> " + item.Summary + " >>> " + item.Description);
	        }
            Console.WriteLine("Get Google Calendar event");
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("<calendar id>").Items)
            {
                Console.WriteLine(item.Id + " >>> " + item.Summary + " >>> " + item.Description + " >>> " + item.Start.DateTime);
            }
            Console.WriteLine("Get future events");
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("<calendar id>").Items)
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
            foreach (Google.Apis.Calendar.v3.Data.Event item in GetEvents("<calendar id>").Items)
            {
                Events res = GetInstanceEvent("<calendar id>",item.Id);
                try
                {
                    if (res.Items.Count>1)
                    {
                        Console.WriteLine(item.Summary + " >>> " + item.Description+" >>>"+res.Items.Count+res.Items[res.Items.Count-1].End.DateTime);
                    }
                }catch(Exception){}
            }
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
            var result = service.Events.List(calendarid).Fetch();
            return result;
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
    }
}
