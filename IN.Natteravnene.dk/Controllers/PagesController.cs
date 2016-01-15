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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class PagesController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public PagesController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        // GET: Pages
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult EditAssociationpages()
        {
            Association association = reposetory.GetAssociationWithPages(CurrentProfile.AssociationID);

            AssociationPagesModel viewModel = new AssociationPagesModel(association);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAssociationpages(AssociationPagesModel result)
        {
            Association association = reposetory.GetAssociationWithPages(CurrentProfile.AssociationID);
            if (ModelState.ContainsKey("PageAbout.Title")) ModelState["PageAbout.Title"].Errors.Clear();
            if (ModelState.ContainsKey("PagePress.Title")) ModelState["PagePress.Title"].Errors.Clear();
            if (ModelState.ContainsKey("PageLink.Title")) ModelState["PageLink.Title"].Errors.Clear();
            if (ModelState.ContainsKey("PageSponsor.Title")) ModelState["PageSponsor.Title"].Errors.Clear();
 


            if (ModelState.IsValid)
            {
                association.PageAbout.Body = result.PageAbout.Body;
                if (string.IsNullOrWhiteSpace(association.PageAbout.Title)) association.PageAbout.Title = DefaultForening.PageContentAboutTitle;
                association.PageLink.Body = result.PageLink.Body;
                if (string.IsNullOrWhiteSpace(association.PageLink.Title)) association.PageLink.Title = DefaultForening.PageContentLinkTitle;
                association.PagePress.Body = result.PagePress.Body;
                if (string.IsNullOrWhiteSpace(association.PagePress.Title)) association.PagePress.Title = DefaultForening.PageContentPressTitle;
                association.PageSponsor.Body = result.PageSponsor.Body;
                if (string.IsNullOrWhiteSpace(association.PageSponsor.Title)) association.PageSponsor.Title = DefaultForening.PageContentSponsorTitle;
                association.Sponsors = result.Sponsors;

                if (reposetory.SaveAssociation(association))
                {
                    ViewBag.FormSucces = true;
                    ModelState.Clear();
                }
                
                AssociationPagesModel viewModel = new AssociationPagesModel(association);

                return View(viewModel);
            }


            return View(result);
        }


        public ActionResult _ResetLocalPage()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association.PageAbout == null) association.PageAbout = new Content();
            association.PageAbout.Title = DefaultForening.PageContentAboutTitle;
            association.PageAbout.Body = DefaultForening.PageContentAbout;

            if (association.PagePress == null) association.PagePress = new Content();
            association.PagePress.Title = DefaultForening.PageContentPressTitle;
            association.PagePress.Body = DefaultForening.PageContentPress;

            if (association.PageSponsor == null) association.PageSponsor = new Content();
            association.PageSponsor.Title = DefaultForening.PageContentSponsorTitle;
            association.PageSponsor.Body = DefaultForening.PageContentSponsor;

            if (association.PageLink == null) association.PageLink = new Content();
            association.PageLink.Title = DefaultForening.PageContentLinkTitle;
            association.PageLink.Body = DefaultForening.PageContentLink;


            association.UsePressPage = false;
            association.UseSponsorPage = false;
            association.UseLinksPage = false;
            reposetory.SaveAssociation(association);
            return RedirectToAction("EditAssociationpages");
        }

        public ActionResult Sponsors()
        {
            Association association = reposetory.GetAssociationWithPages(CurrentProfile.AssociationID);

            return View(association.Sponsors);
        }

        public ActionResult EditSponsor(Guid? ID)
        {
            Sponsor S = null; ;

            if (ID != null) S = reposetory.GetSponsor((Guid)ID);

            if (S == null) S = new Sponsor();

            return View(S);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditSponsor(Sponsor Sponsor, HttpPostedFileBase LogoUpload)
        {
            Sponsor dbSponsor = new Sponsor
            {
                AssociationID = CurrentProfile.AssociationID
            };
            if (Sponsor.SponsorID != Guid.Empty) dbSponsor = reposetory.GetSponsor(Sponsor.SponsorID);
            if (dbSponsor == null) return HttpNotFound();

           

            if (ModelState.IsValid)
            {
                dbSponsor.Name = Sponsor.Name;
                dbSponsor.Body = Sponsor.Body;
                dbSponsor.Finish = Sponsor.Finish;
                dbSponsor.URL = Sponsor.URL;
                dbSponsor.Sequence = Sponsor.Sequence;
                
                //dbEvent.Trim();

                if (reposetory.Save(dbSponsor))
                {
                    Sponsor.SponsorID = dbSponsor.SponsorID;
                    ModelState.Clear();
                    ViewBag.FormSucces = true;
                    if (LogoUpload != null && LogoUpload.ContentLength > 0 && IsImage(LogoUpload))
                    {
                        String url = SaveImageFile(LogoUpload, Sponsor.SponsorID);


                    }
                }
            }




            return View(Sponsor);
           
        }

        public ActionResult DeleteSponsor(Guid ID)
        {
            Sponsor dbSponsor = reposetory.GetSponsor(ID);



            if (dbSponsor != null && reposetory.DeleteSponsor(dbSponsor.SponsorID))
            { }

            return RedirectToAction("Sponsors");

        }


        private string SaveImageFile(HttpPostedFileBase file, Guid id)
        {
            // Define destination
            string folderName = Url.Content(ConfigurationManager.AppSettings["SponsorLogo"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }

            string FileUrl = string.Format(folderName, id.ToString() + Path.GetExtension(file.FileName));
            string fullFileName = HttpContext.Server.MapPath(FileUrl);
            var serverPath = Path.GetDirectoryName(fullFileName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }


            var img = new WebImage(file.InputStream);
            int width = 250;
            int height = 250;
            int minheight = 20;

            double ratio = (double)img.Width / img.Height;
            double desiredRatio = (double)width / height;

            if (ratio > desiredRatio)
            {
                height = Convert.ToInt32(width / ratio);
                if (height < minheight)
                {
                    int delta = Convert.ToInt32((minheight * ratio - img.Width) / 2);
                    img.Crop(0, delta, 0, delta);
                }
            }
            if (ratio < desiredRatio)
            {
                int delta = Convert.ToInt32((img.Height - img.Width / desiredRatio) / 2);
                img.Crop(delta, 0, delta, 0);
            }


            img.Resize(width, height, true, true);

            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(serverPath+"/"));
            FileInfo[] files = dir.GetFiles(id.ToString() + ".*");
            if (files.Any())
            {
                foreach (FileInfo dfile in files)
                {
                    System.IO.File.Delete(dfile.FullName);
                }

            }

            //if (System.IO.File.Exists(fullFileName))
            //    System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return FileUrl;
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            var extensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" }; // add more if you like...

            if (file.ContentType.Contains("image"))
            {
                return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));   // linq from Henrik Stenbæk

            }

            return false;
        }

    }
}