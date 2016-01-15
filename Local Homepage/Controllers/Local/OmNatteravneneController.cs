/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using Local_Homepage.Models;
using NR.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NR.Models;
using System.Configuration;
using System.IO;
using DTA;

namespace Local_Homepage.Controllers
{
    public class OmNatteravneneController : BaseController
    {

        public ActionResult Konceptet()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult Fakta()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult Regler()
        {
            Initiate();
            using (var db = new NRDbContext())
            {
                Content con = db.Content.Find(new Guid("00A8EAD6-0A74-E411-BF1A-C485084553DE"));
                Basedata.Content = con == null ? new Content() : con;
            }
            //00A8EAD6-0A74-E411-BF1A-C485084553DE
            return View(Basedata);
        }

        public ActionResult Logo()
        {
            Initiate();
            return View(Basedata);
        }

        public ActionResult DownloadLogo(string type)
        {
            Initiate();

            

            string LogoDirSetting = ConfigurationManager.AppSettings["Logos"];

            if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

            string extension = type.ToLower();
            string contentType = "";

            if (type.ToLower() != "jpg" & type.ToLower() != "pdf" & type.ToLower() != "emf") extension = "pdf";

            string Filename =   Basedata.AssociationNameGenitive.ValidFileName() + "-logo." + extension;

            switch  (extension)
            {
                case "jpg":
                    contentType = "image/jpeg";
                    break;
                case "emf":
                    contentType = "image/x-emf";
                    break;
                default:
                    contentType = "application/pdf";
                    break;
 
                    }

            var LogoFile = Path.Combine(LogoDirSetting, Basedata.AssociationID + "." + extension);

            if (!System.IO.File.Exists(Server.MapPath(LogoFile)))
            {
                LogoFile = Path.Combine(LogoDirSetting, "Natteravnene_logo." + extension);
                Filename = "Natteravnenes-logo." + extension;
            }

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = LogoFile,
               // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            //Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(LogoFile, contentType, Filename);
        }

        public ActionResult Landssekretariatet()
        {
            Initiate();
            return View(Basedata);
        }
    }
}