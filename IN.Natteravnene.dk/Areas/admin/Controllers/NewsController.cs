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

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class NewsController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public NewsController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        } 
        
        // GET: Admin/News
        public ActionResult Index()
        {

            List<NewsAdminViewModel> result = new List<NewsAdminViewModel>();

            foreach (News N in reposetory.GetNews())
            {
                NewsAdminViewModel tmp = new NewsAdminViewModel
                {
                    News = N
                };
                if (N.Source == LevelType.Local)
                { 
                    tmp.Association = reposetory.GetAssociation(N.SourceLink);
                    tmp.SourceName = tmp.Association == null ? "-" : tmp.Association.Name;
                }
                if (N.Source == LevelType.Network)
                { 
                    tmp.Network = reposetory.GetNetwork(N.SourceLink);
                    tmp.SourceName = tmp.Network == null ? "-" : tmp.Network.NetworkName;
                }
                result.Add(tmp);
            }

            return View(result);
        }

        public ActionResult Edit(Guid? Id)
        {
            News News = new News
            {
                Publish = DateTime.Now.Date.AddHours(DateTime.Now.Hour),
                Source = LevelType.National,
                Distribution = LevelType.National
            };
            if (Id != null) News = reposetory.GetNewsItem((Guid)Id);
            if (News == null) return HttpNotFound();

            return View(News);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(News News)
        {

            News dbNews = new News
            {
                Publish = DateTime.Now.Date.AddHours(DateTime.Now.Hour),
                Source = LevelType.National,
                Distribution = LevelType.National
            };
            if (News.NewsId != Guid.Empty) dbNews = reposetory.GetNewsItem(News.NewsId);
            if (dbNews == null) return HttpNotFound();

            if (ModelState.IsValid)
            {

                dbNews.Headline = News.Headline;
                dbNews.Teaser = News.Teaser;
                dbNews.Internal = News.Internal;
                dbNews.Content = News.Content;
                dbNews.Distribution = News.Distribution;
                dbNews.DistributionLink = News.DistributionLink;
                dbNews.Source = News.Source;
                dbNews.SourceLink = News.SourceLink;
                dbNews.Publish = News.Publish;
                dbNews.Depublish = News.Depublish;
                dbNews.AuthorID = CurrentProfile.PersonID;
                dbNews.Trim();

                if (reposetory.Save(dbNews))
                {
                    News.NewsId = dbNews.NewsId;
                    ModelState.Clear();
                    ViewBag.FormSucces = true;
                }
            }




            return View(News);
        }

        [HttpPost]
        public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files, Guid NewsId)
        {
            
            string errorMessage = "";

            if (files != null && files.Count() > 0)
            {
                News News = reposetory.GetNewsItem(NewsId);
                if (News == null) return Json(new { success = false, errorMessage = "" });
                // Get one only
                var file = files.FirstOrDefault();
                // Check if the file is an image
                if (file != null && IsImage(file))
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        //var webPath = SaveTemporaryFile(file, id);
                        //var fn = Path.Combine(Server.MapPath(Url.Content(ConfigurationManager.AppSettings["NewsImage"])), file.FileName);
                        
                        var webPath = SaveImageFile(file, News.NewsId) +"?update=" + DateTime.Now.Ticks.ToString();
                        return Json(new { success = true, fileName = webPath.Replace("/", "\\") }); // success
                    }
                    errorMessage = General.FileUploadZeroLength; //failure
                }
                errorMessage = General.FileUploadWrongFormat; //failure
            }
            errorMessage = General.FileUploadNoUploaded; //failure

            return Json(new { success = false, errorMessage = errorMessage });
        }

        public ActionResult Delete(Guid Id)
        {
            reposetory.DeleteNews(Id);

            return RedirectToAction("Index");
        }

        public ActionResult View(Guid Id)
        {
            if (Id == Guid.Empty) return HttpNotFound();
            News News = reposetory.GetNewsItem(Id);
            if (News == null) return HttpNotFound();

            return View(News);
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            var extensions = new string[] { ".jpg", ".jpeg" }; // add more if you like...

            // linq from Henrik Stenbæk
            return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string SaveImageFile(HttpPostedFileBase file, Guid id)
        {
            // Define destination
            string folderName = Url.Content(ConfigurationManager.AppSettings["NewsImage"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }

            string FileUrl = string.Format(folderName, id.ToString());
            string fullFileName = HttpContext.Server.MapPath(FileUrl);
            var serverPath = Path.GetDirectoryName(fullFileName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            var img = new WebImage(file.InputStream);
            int width = 1280;
            int height = 750;
            int minheight = 400;

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
            
            if (System.IO.File.Exists(fullFileName))
                System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return FileUrl;
        }

    }
}