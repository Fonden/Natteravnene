﻿/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using NR.Infrastructure;
using NR.Localication;
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
    public class AvatarController : Controller
    {
        private int _avatarWidth = 428; // ToDo - Change the size of the stored avatar image
        private int _avatarHeight = 550; // ToDo - Change the size of the stored avatar image

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

        [ValidateAntiForgeryToken]
        public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files, Guid id)
        {
            string errorMessage = "";

            if (files != null && files.Count() > 0)
            {
                // Get one only
                var file = files.FirstOrDefault();
                // Check if the file is an image
                if (file != null && IsImage(file))
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        var webPath = SaveTemporaryFile(file, id);
                        return Json(new { success = true, fileName = webPath.Replace("/", "\\") }); // success
                    }
                    errorMessage = General.FileUploadZeroLength; //failure
                }
                else
                {
                    errorMessage = General.FileUploadWrongFormat; //failure
                }
            }
            else
            {
                errorMessage = General.FileUploadNoUploaded; //failure
            }
            return Json(new { success = false, errorMessage = errorMessage });
        }

        [HttpPost]
        public ActionResult Save(int x, int y, int x2, int y2, string fileName)
        {
            string ProfilDirSetting = Url.Content(ConfigurationManager.AppSettings["ProfileImage"]);

            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }

            string folderName = Url.Content(ConfigurationManager.AppSettings["TempDir"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }

            try
            {
                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath(Url.Content(ConfigurationManager.AppSettings["TempDir"])), Path.GetFileName(fileName));

                // Calculate dimesnions
                //int top = Convert.ToInt32(t);
                //int left = Convert.ToInt32(l);
                //int height = Convert.ToInt32(h);
                //int width = Convert.ToInt32(w);

                // Get image and resize it, ...
                var img = new WebImage(fn);

                // ... crop the part the user selected, ...
                img.Crop(y, x, (img.Height - y2) < 0 ? 0 : img.Height - y2, (img.Width - x2) < 0 ? 0 : img.Width - x2);
                img.Resize(_avatarWidth, _avatarHeight);
                // ... delete the temporary file,...
                System.IO.File.Delete(fn);
                // ... and save the new one.
                string newFileName = string.Format(ProfilDirSetting, Path.GetFileNameWithoutExtension(fn));
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }

                img.Save(newFileLocation);

                return Json(new { success = true, avatarFileLocation = newFileName + "?" + DateTime.Now.Millisecond.ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " + ex.Message });
            }
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            var extensions = new string[] { ".jpg", ".jpeg" }; // add more if you like...

            if (file.ContentType.Contains("image"))
            {
                return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));   // linq from Henrik Stenbæk

            }

            return false;
        }

        private string SaveTemporaryFile(HttpPostedFileBase file, Guid id)
        {
            // Define destination
            string folderName = Url.Content(ConfigurationManager.AppSettings["TempDir"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }


            var serverPath = HttpContext.Server.MapPath(folderName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = id.ToString() + ".jpg";                                //Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);

            return Path.Combine(folderName, fileName);
        }

        private string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            double ratio = (double)img.Height / (double)img.Width;

            string fullFileName = Path.Combine(serverPath, fileName);

            //img.Resize(400, (int)(400 * ratio)); // ToDo - Change the value of the width of the image oin the screen

            if (System.IO.File.Exists(fullFileName))
                System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return Path.GetFileName(img.FileName);
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                DateTime fileCreationTime;
                DateTime currentUtcNow = DateTime.UtcNow;

                var serverPath = HttpContext.Server.MapPath("/Temp");
                if (Directory.Exists(serverPath))
                {
                    string[] fileEntries = Directory.GetFiles(serverPath);
                    foreach (var fileEntry in fileEntries)
                    {
                        fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                        var res = currentUtcNow - fileCreationTime;
                        if (res.TotalHours > hoursOld)
                        {
                            System.IO.File.Delete(fileEntry);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}