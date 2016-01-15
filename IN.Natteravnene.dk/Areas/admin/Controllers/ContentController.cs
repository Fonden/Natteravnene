/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NR.Models;
using NR.Infrastructure;
using NR.Localication;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class ContentController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public ContentController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        // GET: Admin/Content
        public ActionResult Index(Guid Id)
        {
            Content content = reposetory.GetContent(Id);
            if (content == null) return HttpNotFound();

            return View(content);
        }

        public ActionResult Edit(Guid Id)
        {
            Content content = reposetory.GetContent(Id);
            if (content == null) return HttpNotFound();

            return View(content);

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Content result)
        {

            if (ModelState.IsValid)
            {
                Content content = reposetory.GetContent(result.ContentID);
                if (content == null) return HttpNotFound();

                content.Title = result.Title;
                content.Body = result.Body;
                content.AuthorID = CurrentProfile.PersonID;
                
                if (reposetory.Save(content))
                {
                    ViewBag.FormSucces = true;
                    ModelState.Clear();
                    Notification not = reposetory.Notify(content, String.Format(Notifications.ContentEdited, CurrentProfile.Username));
                    reposetory.NotifyAddAdministration(not);
                    reposetory.NotifySave(not);
                    return View(content);
                }
                else
                {
                    ViewBag.CaughtException = true;
                }
            }

            return View(result);

        }

    }
}