/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DTA;
using System.Text.RegularExpressions;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class HomeController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public HomeController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        // GET: Admin/Home
        public ActionResult Index()
        {
            //throw new NullReferenceException("test");
            return View();
        }

        public ActionResult Communication()
        {
            return View();
        }

        public ActionResult VersionNotification(string text)
        {
            Notification not = reposetory.Notify(new NewVersion() , text);
            reposetory.NotifyAddSystem(not);
            reposetory.NotifySave(not);
            return Content("OK");
        }

        public ActionResult TroubleShooting()
        {
            return View();
        }

        public ActionResult _fixaccount(string Username)
        {
            string content = string.Empty;
            Person P = reposetory.GetPerson(Username);
            if (P != null)
            {
                content += string.Format("<p>Navn: {0}<br/>E-mail: {1}<br/>mobil: {2} </p>", P.FullName, P.Email, P.Mobile);

                if (string.IsNullOrWhiteSpace(P.Password))
                {
                    content += string.Format("<p>Kodeord eksistere ikke - oprettet</p>");
                    P.Password.GeneratePassword();
                    reposetory.SavePerson(P);
                }
                else if (Regex.IsMatch(P.Password, @"^[a-zA-Z0-9æøåÆØÅ*!#%&]{6,10}$"))
                {
                    content += string.Format("<p>Kodeord ikke rigtig format - nyt lavet</p>"); 
                    P.Password.GeneratePassword();
                    reposetory.SavePerson(P);
                }
                else
                {
                    content += string.Format("<p>Kodeord OK</p>");
                }

                if (!WebSecurity.IsConfirmed(P.UserName))
                {
                    
                    WebSecurity.CreateAccount(P.UserName, P.Password);
                    content += string.Format("<p>Konto oprettet</p>");
                }
                else
                {
                    content += string.Format("<p>Konto eksistere</p>");
                    if (!WebSecurity.Login(P.UserName, P.Password, persistCookie: false))
                    {
                        if (WebSecurity.ResetPassword(WebSecurity.GeneratePasswordResetToken(P.UserName), P.Password))
                        {
                            content += string.Format("<p>Fejl på kontokodeord - rettet</p>");
                        }
                    }
                    else
                    {
                        WebSecurity.Logout();
                    }
                }
                    
            }
            else
            {
                content += string.Format("<p>Bruger eksistere ikke</p>");
            }


            return Content(content);
        }

        public ActionResult Access()
        {

            var roles = Roles.IsUserInRole("Administrator") ? Roles.GetAllRoles().ToList().OrderBy(x => x).ToList() : Roles.GetAllRoles().ToList().Where(x => x != "Administrator").OrderBy(x => x).ToList();

            AdminAccessModel Model = new AdminAccessModel
            {
                DropRoles = new SelectList(roles),
                UserInAdminRole = Roles.GetUsersInRole("Administrator").OrderBy(x => x).ToList(),
                UserInManagementRole = Roles.GetUsersInRole("Management").OrderBy(x => x).ToList()
            };


            return View(Model);
        }

        public ActionResult Delete(string UserName, string Role)
        {
            if (!Roles.IsUserInRole("Administrator") & Role == "Administrator") RedirectToAction("Access");
            Roles.RemoveUserFromRole(UserName, Role);
            return RedirectToAction("Access");
        }

        public ActionResult Assign(string UserName, string Role)
        {
            if (!Roles.IsUserInRole("Administrator") & Role == "Administrator") RedirectToAction("Access");
            Roles.AddUserToRole(UserName, Role); //TODO: Make error proof
            return RedirectToAction("Access");
        }

        [AllowAnonymous]
        public ActionResult ScheduleTasks()
        {
            var Update = new Processes(reposetory);
            Update.HandshakeUrl = Url.Action("TextXStatus", "Account", new { ID = "##" }, "http");
            Update.SendTeamTexts();
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult Tests()
        {
            return View();
        }

        public ActionResult _TestText(string Mobile)
        {
            if (!string.IsNullOrWhiteSpace(Mobile))
            {
                Person person = new Person
                {
                    Mobile = Mobile,
                    Country = Country.DK
                };

                ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(null, null, null);
                SMSGateway.FromText = General.SystemTextMessagesFrom;
                SMSGateway.Message = "Test text";
                SMSGateway.Recipient = new List<Person> { person };
                //reposetory.NewTextMessage(SMSGateway, association.AssociationID);

                //if (HandshakeUrl != null && HandshakeUrl.Contains("##")) SMSGateway.HandShakeUrl = HandshakeUrl.Replace("##", SMSGateway.TextId);

                try
                {
                    if (SMSGateway.Send())
                    {

                    };
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }

            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult _TestEmail(string email)
        {
            if (!String.IsNullOrWhiteSpace(email))
            {
                var mail = new ForgotLoginEmail
                {
                    To = email,
                    UserName = "Test Email",
                    Password = "Test"
                };
                try
                {
                    mail.Send();
                    //throw new NullReferenceException("");
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult _TestMessage(string UserName)
        {
            if (!String.IsNullOrWhiteSpace(UserName))
            {
                Person P = reposetory.GetPerson(UserName);

                if (P == null) return Content("UserName does not exists");

                Message Msg = new Message
                {
                    Recivers = new List<MessageReciver> { new MessageReciver {  Reciver = P }},
                    SenderID = CurrentProfile.PersonID,
                    Body = "Test mesage body. \n\r\n\rFrivillig.natteravnene.dk",
                    Head = "Test message Head",
                    Type = MessageType.shortMessage
                };
            try
                {
                    reposetory.MessageSend(Msg);
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }


            Message MsgLong = new Message
                {
                    Recivers = new List<MessageReciver> { new MessageReciver {  Reciver = P }},
                    SenderID = CurrentProfile.PersonID,
                    Body = "Test mesage body",
                    Head = "Test message Head",
                    Type = MessageType.LongMessage
                };
                
                
                try
                {
                    reposetory.MessageSend(MsgLong);
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult Restart()
        {
            return null;
        }


        


    }
}