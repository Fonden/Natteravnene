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
            return View();
        }

        //[HttpPost]
        public ActionResult _GetDirectory(string id)
        {
            string DirSetting = Url.Content(ConfigurationManager.AppSettings["BrowseDirAll"]);
            UrlHelper u = new UrlHelper(this.ControllerContext.RequestContext);

            if (string.IsNullOrWhiteSpace(id)) id = "/";


            string realPath = Server.MapPath(DirSetting + id);
            if (System.IO.Directory.Exists(realPath))
            {

                List<FulexTree> Dir = new List<FulexTree>();
                IEnumerable<string> dirList = Directory.EnumerateDirectories(realPath);
                foreach (string dir in dirList)
                {
                    DirectoryInfo d = new DirectoryInfo(dir);

                    FulexTree dirModel = new FulexTree
                    {
                        name = Path.GetFileName(dir),
                        type = "folder",
                        dataAttributes = new FulexdataAttributes
                        {
                            id = id + Path.GetFileName(dir) + "/",
                            children = d.GetDirectories().Any() ? true : false
                        }
                    };

                    Dir.Add(dirModel);

                }
                IEnumerable<string> fileList = Directory.EnumerateFiles(realPath);
                foreach (string file in fileList)
                {
                    FileInfo f = new FileInfo(file);

                    if ((f.Attributes & FileAttributes.Hidden) != 0) continue;

                    FulexTree dirModel = new FulexTree
                    {
                        name = Path.GetFileName(file),
                        type = "item",
                        url = u.Action("DownloadFile", "File") + id + Path.GetFileName(file).Replace(".", "|"),
                        dataAttributes = new FulexdataAttributes { id = Path.GetFileName(file) }
                    };

                    switch (f.Extension.ToLower())
                    {
                        case ".pdf":
                            dirModel.name = "<i class=\"fa fa-file-pdf-o\"></i> " + dirModel.name;
                            break;
                        case ".xls":
                        case ".xlt":
                        case ".xlm":
                        case ".xlsx":
                        case ".xlsm":
                        case ".xltx":
                            dirModel.name = "<i class=\"fa fa-file-excel-o\"></i> " + dirModel.name;
                            break;
                        case ".doc":
                        case ".dot":
                        case ".docx":
                        case ".docm":
                        case ".dotm":
                            dirModel.name = "<i class=\"fa fa-file-word-o\"></i> " + dirModel.name;
                            break;
                        case ".ppt":
                        case ".pot":
                        case ".pptx":
                        case ".potm":
                        case ".potx":
                            dirModel.name = "<i class=\"fa fa-file-powerpoint-o\"></i> " + dirModel.name;
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                            dirModel.name = "<i class=\"fa fa-file-image-o\"></i> " + dirModel.name;
                            break;
                        case ".zip":
                            dirModel.name = "<i class=\"fa fa-file-archive-o\"></i> " + dirModel.name;
                            break;
                        case ".mov":
                        case ".mpg":
                        case ".wmv":
                            dirModel.name = "<i class=\"fa fa-file-video-o\"></i> " + dirModel.name;
                            break;
                        case ".html":
                            dirModel.name = "<i class=\"fa fa-file-code-o\"></i> " + dirModel.name;
                            break;
                        default:
                            dirModel.name = "<i class=\"fa fa-file-o\"></i> " + dirModel.name;
                            break;

                    }

                    //dirModel.name = "<a href=\"" + u.Action("DownloadFile", "SI", new { src = System.Web.HttpUtility.UrlEncode(id + Path.GetFileName(file)) }) + "\" target=\"_blank\" >" + dirModel.name + "</a>";


                    Dir.Add(dirModel);





                }




                return Json(new
                {
                    error = "",
                    status = "OK",
                    data = Dir
                },
                JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult DownloadFile(string src)
        {
            string DirSetting = Url.Content(ConfigurationManager.AppSettings["BrowseDirAll"]);
            string fullName = Server.MapPath(DirSetting + "/" + src.Replace("|", "."));
            if (!System.IO.File.Exists(fullName)) return HttpNotFound();
            FileInfo f = new FileInfo(fullName);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, f.Name);
        }

        public ActionResult Download(Guid id)
        {
            AFile file = reposetory.GetFile(id);
            if (file == null) return HttpNotFound();
            
            string fullName = Server.MapPath(ConfigurationManager.AppSettings["FilesDir"] + "/" + id.ToString());
            if (!System.IO.File.Exists(fullName)) return HttpNotFound();
            FileInfo f = new FileInfo(fullName);
             

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Title.Trim() + file.Type);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

    }
}