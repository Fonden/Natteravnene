/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using Local_Homepage.Controllers;
using Local_Homepage.Models;
using NR.Entity;
using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ASP.NET_MVC5_Bootstrap3_3_1_LESS.Controllers
{
    public class LocalController : BaseController
    {
        

        public ActionResult Index()
        {
                Initiate();

                string[] YoutubeEmbed = new string[] { "sKAKFFiUoDc", "gtwcLkw90Qw" };
                Random rnd = new Random();
                int r = rnd.Next(YoutubeEmbed.Count());

                ViewBag.EmbedCode = (string)YoutubeEmbed[r];
                return View(Basedata);
        }

        public ActionResult Kontakt()
        {
            Initiate();
            //GetBoard();
            return View(Basedata);
        }

        public ActionResult Sponsorer()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult Nyheder(int? id)
        {
            Initiate();

            News(id ?? 0);


             // List<News> news = reposetory.GetUserNews(id ?? 0).ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_newsListing", Basedata.News);
                }

            return View(Basedata);
        }

        public ActionResult Nyhed(Guid ID, string Titel)
        {
            Initiate();
            GetNews(ID);
            if (Basedata.theNews == null) return HttpNotFound();
            return View(Basedata);
        }

        public ActionResult Aktiviteter()
        {
            Initiate();
            Events(0);
            return View(Basedata);
        }

        public ActionResult Aktivitet(Guid ID, string Titel)
        {
            Initiate();
            GetEvent(ID);
            if (Basedata.Event == null) return HttpNotFound();


            return View(Basedata);
        }

        public ActionResult Links()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult Presse()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult NoLocal()
        {
            return RedirectPermanent("http://natteravnene.dk");
        }

        public ActionResult OldPath(string pathInfo)
        {
            
            return RedirectToActionPermanent("Index");
        }                                                                                                   

        [HttpPost]
        public ActionResult BlivNatteravn(TrialFormModel result)
        {
            Initiate();
            if (ModelState.IsValid)
            {
                Lead lead = new Lead();

                lead.FirstName = result.FirstName;
                lead.FamilyName = result.LastName;
                lead.Zip = result.Zip;
                lead.Email = result.Email;
                lead.Phone = result.Phone;
                lead.AssociationID = Basedata.AssociationID;
                lead.Created = DateTime.Now;
                lead.Lastchanged = DateTime.Now;

                if (lead.Zip.Length != 0 & lead.Zip.Length < 2)
                {
                    lead.Zip += "_";
                }

                WriteLeadLogFile(lead);

                using (var db = new NRDbContext())
                {
                    db.Entry(lead).State = EntityState.Added;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        LogFile.Write(e, "Save file");
                        return PartialView("_BlivNatteravnError", result);

                    }
                }

                return PartialView("_BlivNatteravnSucces", result);
            }
            
            return PartialView("_BlivNatteravn", result);
        }

        
        private static void WriteLeadLogFile(Lead lead)
        {
            WriteLeadLogFile(lead.FirstName + "," +
                lead.FamilyName + "," +
                lead.Address  + "," +
                lead.City + "," +
                lead.Zip  + "," +
                lead.Mobile   + "," +
                lead.Phone  + "," +
                lead.Email   + "," +
                lead.AssociationID    + "," +
                DateTime.Now.ToString() + "\r\n"
                );

        }
        
        private static void WriteLeadLogFile(string message)
        {
            int NumTry = 0;
            while (true)
            {
                try
                {
                    String LogFilePath = HostingEnvironment.MapPath(@"~/log/LeadLog.txt");

                    if (Directory.Exists(Path.GetDirectoryName(LogFilePath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                    }
                    System.IO.File.AppendAllText(LogFilePath, message);
                    break;
                }
                catch (IOException)
                {
                    // check whether it's a lock problem
                    Thread.Sleep(100);
                }
                NumTry++;
                if (NumTry >= 10) break;
            }

        }

    }
}