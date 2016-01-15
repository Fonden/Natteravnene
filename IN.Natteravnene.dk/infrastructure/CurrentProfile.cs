/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Entity;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace NR.Infrastructure
{
    public static class CurrentProfile
    {
 
        //Enables roles Admin and Manageent to simulate memebership of an assosciation
        public static Guid AdminSimulationAssociation { get; set; }

        public static Guid PersonID
        {

            get
            {
                CheckData();
                return AsGuid("PersonID");
            }
        }

        public static string Username
        {

            get
            {
                CheckData();
                return AsString("Username");
            }
        }

        public static string Email
        {

            get
            {
                CheckData();
                return AsString("Email");
            }
        }

        public static string Mobile
        {

            get
            {
                CheckData();
                return AsString("Mobile");
            }
        }

        public static Guid AssociationID
        {

            get
            {
                CheckData();
                return AsGuid("AssociationID");
            }
        }

        public static string AssociationName
        {

            get
            {
                CheckData();
                return AsString("AssociationName");
            }
        }

        public static string TextServiceProvider
        {

            get
            {
                CheckData();
                return AsString("TextServiceProvider");
            }
        }

        public static string TextServiceProviderUserName
        {

            get
            {
                CheckData();
                return AsString("TextServiceProviderUserName");
            }
        }

        public static string TextServiceProviderPassword
        {

            get
            {
                CheckData();
                return AsString("TextServiceProviderPassword");
            }
        }

        public static Guid NetworkID
        {

            get
            {
                CheckData();
                return AsGuid("NetworkID");
            }
        }

        public static bool hasMembership
        {
            get
            {
                CheckData();
                return AsBool("hasMembership");
            }
        }

        #region Rights

        public static bool isTeamleader
        {
            get
            {
                CheckData();
                return AsBool("isTeamleader");
            }
        }

        public static bool isPlanner
        {
            get
            {
                CheckData();
                return AsBool("isPlanner");
            }
        }

        public static bool isSecretary
        {
            get
            {
                CheckData();
                return AsBool("isSecretary");
            }
        }

        public static bool isEditor
        {
            get
            {
                CheckData();
                return AsBool("isEditor");
            }
        }

        public static bool isSeniorInstructor
        {
            get
            {
                CheckData();
                return AsBool("isSeniorInstructor");
            }
        }
        #endregion

        #region Modules

        public static bool usePlanning
        {
            get
            {
                CheckData();
                return AsBool("usePlanning");
            }
        }

        public static bool useTakeTeamSpot
        {
            get
            {
                CheckData();
                return AsBool("useTakeTeamSpot");
            }
        }

        public static bool UseKeyBox
        {
            get
            {
                CheckData();
                return AsBool("UseKeyBox");
            }
        }

        #endregion

        #region Settings
        public static int ListLines
        {
            get
            {
                CheckData();
                return AsInt("ListLines") < 0 ? 10 : AsInt("ListLines");
            }
        }

        public static bool EmailNewsLetter
        {
            get
            {
                CheckData();
                return AsBool("EmailNewsLetter");
            }
        }

        public static bool PrintNewslettet
        {
            get
            {
                CheckData();
                return AsBool("PrintNewslettet");
            }
        }

        public static bool MailUndeliverable
        {
            get
            {
                CheckData();
                return AsBool("MailUndeliverable");
            }
        }

        #endregion

        public static Userbar userbar
        {
            get
            {
                CheckData();
                List<Notification> notifications;
                int UnreadNotifications;
                List<Message> messages;
                int UnreadMessages;
                using (var dbContext = new Repository())
                {
                    var NotQuery = dbContext.NotificationRecivers.Where(R => R.Reciver.PersonID == PersonID & !R.Read).Select(N => N.Notification).OrderByDescending(N => N.Created);
                    notifications = NotQuery.Take(5).ToList();
                    UnreadNotifications = NotQuery.Count();
                    if (notifications == null) notifications = new List<Notification>();

                    int count;
                    messages = dbContext.GetUserUnreadMessages(PersonID, out count);

                    UnreadMessages = count;
                    if (messages == null) messages = new List<Message>();

                    return new Userbar
                    {
                        CurrentUser = dbContext.People.Where(p => p.UserID == WebSecurity.CurrentUserId).FirstOrDefault() ?? new Person(),
                        CurrentAssociation = dbContext.Memberships.Where(m => m.Person.PersonID == PersonID & m.AssociationID == AssociationID).Select(m => m.Association).FirstOrDefault() ?? new Association(),
                        Notifications = notifications == null ? new List<Notification>() : notifications,
                        UnreadNotifications = UnreadNotifications,
                        Messages = messages == null ? new List<Message>() : messages,
                        UnreadMessages = UnreadMessages

                    };
                }

            }

        }


        public static void Clear()
        {
            if (HttpContext.Current.Session == null) return;
            HttpContext.Current.Session["UserID"] = null;
            HttpContext.Current.Session["Username"] = null;
            HttpContext.Current.Session["Email"] = null;
            HttpContext.Current.Session["Mobile"] = null;
            HttpContext.Current.Session["PersonID"] = null;
            HttpContext.Current.Session["isSeniorInstructor"] = null;

            HttpContext.Current.Session["ListLines"] = null;
            HttpContext.Current.Session["EmailNewsLetter"] = null;
            HttpContext.Current.Session["PrintNewslettet"] = null;
            HttpContext.Current.Session["MailUndeliverable"] = null;

            HttpContext.Current.Session["hasMembership"] = null;

            HttpContext.Current.Session["AssociationID"] = null;
            HttpContext.Current.Session["AssociationName"] = null;
            HttpContext.Current.Session["TextServiceProvider"] = null;
            HttpContext.Current.Session["TextServiceProviderUserName"] = null;
            HttpContext.Current.Session["TextServiceProviderPassword"] = null;

            HttpContext.Current.Session["NetworkID"] = null;

            HttpContext.Current.Session["isTeamleader"] = null;
            HttpContext.Current.Session["isPlanner"] = null;
            HttpContext.Current.Session["isSecretary"] = null;
            HttpContext.Current.Session["isEditor"] = null;

            HttpContext.Current.Session["usePlanning"] = null;
            HttpContext.Current.Session["useTakeTeamSpot"] = null;
            HttpContext.Current.Session["UseKeyBox"] = null;
        }

        private static void CheckData()
        {
            if (HttpContext.Current.Session == null || !WebSecurity.IsAuthenticated)
            {
                Clear();
                return;
            }
            if (HttpContext.Current.Session["UserID"] != null && (int)HttpContext.Current.Session["UserID"] == WebSecurity.CurrentUserId) return;

            Clear();

            using (var dbContext = new Repository())
            {
                Person person = dbContext.People.Where(p => p.UserID == WebSecurity.CurrentUserId).FirstOrDefault();
                NRMembership membership = null;

                if (person != null)
                {

                    if (person.CurrentAssociation != Guid.Empty)
                        membership = dbContext.Memberships.Where(m => m.Person.PersonID == person.PersonID & m.Association.AssociationID == person.CurrentAssociation).Include(m => m.Association).FirstOrDefault();

                    if (membership == null)
                        membership = (from m in dbContext.Memberships where m.Person.PersonID == person.PersonID & m.AbsentDate == null select m).Include(m => m.Association).FirstOrDefault();

                    if (membership != null && membership.Association != null && person.CurrentAssociation != membership.Association.AssociationID)
                    {
                        person.CurrentAssociation = membership.AssociationID;

                        dbContext.Entry(person).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }


                    HttpContext.Current.Session["UserID"] = person.UserID;
                    HttpContext.Current.Session["Username"] = person.UserName;
                    HttpContext.Current.Session["Email"] = person.Email;
                    HttpContext.Current.Session["Mobile"] = person.Mobile;
                    HttpContext.Current.Session["PersonID"] = person.PersonID;
                    HttpContext.Current.Session["isSeniorInstructor"] = person.SeniorInstructor;

                    HttpContext.Current.Session["ListLines"] = person.ListLines < 10 ? 10 : person.ListLines;
                    HttpContext.Current.Session["EmailNewsLetter"]  = person.EmailNewsLetter;
                    HttpContext.Current.Session["PrintNewslettet"] = person.PrintNewslettet;
                    HttpContext.Current.Session["MailUndeliverable"] = person.MailUndeliverable;


                    if (membership != null && !membership.Absent && membership.Association != null)
                    {
                        HttpContext.Current.Session["hasMembership"] = true;

                        HttpContext.Current.Session["AssociationID"] = membership.AssociationID;
                        HttpContext.Current.Session["AssociationName"] = membership.Association.Name;
                        HttpContext.Current.Session["TextServiceProvider"]   = membership.Association.TextServiceProvider;
                        HttpContext.Current.Session["TextServiceProviderUserName"]  = membership.Association.TextServiceProviderUserName;
                        HttpContext.Current.Session["TextServiceProviderPassword"] = membership.Association.TextServiceProviderPassword;

                        HttpContext.Current.Session["NetworkID"] = membership.Association == null ? Guid.Empty : membership.Association.NetworkID;

                        HttpContext.Current.Session["isTeamleader"] = membership.Teamleader;
                        HttpContext.Current.Session["isPlanner"] = membership.Planner;
                        HttpContext.Current.Session["isSecretary"] = membership.Secretary;
                        HttpContext.Current.Session["isEditor"] = membership.Editor;
                        

                        HttpContext.Current.Session["usePlanning"] = membership.Association.UseSchedulePlanning;
                        HttpContext.Current.Session["useTakeTeamSpot"] = membership.Association.UseTakeTeamSpot;
                        HttpContext.Current.Session["UseKeyBox"] = membership.Association.UseKeyBox;
                    }
                }
               
            }
        }

        private static string AsString(string prop)
        {
            if (HttpContext.Current.Session == null) return string.Empty;
            var value = HttpContext.Current.Session[prop];
            return (value ?? string.Empty).ToString();
        }

        private static bool AsBool(string prop)
        {
            if (HttpContext.Current.Session == null) return false;
            var value = HttpContext.Current.Session[prop]; 
            if (value is bool)
                return (bool)value;
            else
                return false;
        }

        private static Guid AsGuid(string prop)
        {
            if (HttpContext.Current.Session == null) return Guid.Empty; 
            var value = HttpContext.Current.Session[prop];
            return value == null ? Guid.Empty : (Guid)value;
        }

        private static int AsInt(string prop)
        {
            if (HttpContext.Current.Session == null) return 0;
            var value = HttpContext.Current.Session[prop];
            return value == null ? 0 : (int)value;
        }
    }
}