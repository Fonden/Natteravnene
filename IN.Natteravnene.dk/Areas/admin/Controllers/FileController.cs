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
    public class FileController : Controller
    {
      
         //Repository
        private INRRepository reposetory;

        public FileController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }


        public ActionResult Index()
        {
            List<AFile> Files = reposetory.GetFiles().Select(x => { x.Exists = FileExists(x); return x; }).ToList();
            

            return View(Files);
        }

        public ActionResult Edit(Guid? id)
        {
            AFile file = new AFile();
            if (id != null)
            {
                file = reposetory.GetFile((Guid)id);

            }
            return View(file);
        }

        private bool FileExists(AFile file)
        {
            string fullName = Server.MapPath(ConfigurationManager.AppSettings["FilesDir"] + "/" + file.FileID.ToString());
            return System.IO.File.Exists(fullName);

        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _Upload()
        {
            return PartialView();
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(AFile postFile, IEnumerable<HttpPostedFileBase> files)
        {
            string errorMessage = "";

            if (files != null && files.Count() > 0)
            {
                // Get one only
                var file = files.FirstOrDefault();
                // Check if the file is an image
                if (file != null && IsValid(file))
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        AFile theFile = new AFile();
                        if (postFile.FileID != Guid.Empty) theFile = reposetory.GetFile(postFile.FileID);
                        if (theFile == null) theFile = new AFile();
                        switch (Path.GetExtension(file.FileName).ToLower())
                        {
                            case ".jpeg":
                            case ".jpg":
                                theFile.Type = ".jpg";
                                break;
                            case ".pdf":
                                theFile.Type = ".pdf";
                                break;
                            case ".ppt":
                                theFile.Type = ".ppt";
                                break;
                            case ".pptx":
                                theFile.Type = ".pptx";
                                break;
                            case ".doc":
                                theFile.Type = ".doc";
                                break;
                            case ".docx":
                                theFile.Type = ".docx";
                                break;
                            case ".xls":
                                theFile.Type = ".xls";
                                break;
                            case ".xlsx":
                                theFile.Type = ".xlsx";
                                break;

                        }
                        theFile.Title = string.IsNullOrWhiteSpace(postFile.Title) ? Path.GetFileNameWithoutExtension(file.FileName) : postFile.Title;
                        theFile.Description = postFile.Description;
                        if (reposetory.Save(theFile))
                        {
                            var webPath = SaveFile(file, theFile.FileID);
                            if (Request.IsAjaxRequest())
                            {
                                return Json(new { success = true, fileName = webPath.Replace("/", "\\") }); // success
                            }
                            ViewBag.FormSucces = true;
                            return View("Edit", postFile);
                        }
                    }
                    errorMessage = General.FileUploadZeroLength; //failure
                }
                errorMessage = General.FileUploadWrongFormat; //failure
            }
            errorMessage = General.FileUploadNoUploaded; //failure
            if (Request.IsAjaxRequest())
                            {
            return Json(new { success = false, errorMessage = errorMessage });
                            }
            return View("Edit", postFile);
        }



        private bool IsValid(HttpPostedFileBase file)
        {

            var extensions = new string[] { ".pdf", ".jpeg", ".jpg", ".docx", ".pptx", ".xlsx" }; // add more if you like...

            // linq from Henrik Stenbæk
            return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string SaveFile(HttpPostedFileBase file, Guid id)
        {
            // Define destination
            string folderName = Url.Content(ConfigurationManager.AppSettings["FilesDir"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }

            
            var serverPath = HttpContext.Server.MapPath(folderName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = id.ToString();                                //Path.GetFileName(file.FileName);
            file.SaveAs(Path.Combine(serverPath, fileName));
            
            return Path.Combine(folderName, fileName);
        }

        
    }
}