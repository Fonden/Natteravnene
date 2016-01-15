/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class NotificationController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public NotificationController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        public ActionResult Index()
        {

            List<Notification> Notefications = reposetory.GetNotifications(CurrentProfile.PersonID);

            return View(Notefications);
        }

        /// <summary>
        /// Displays a News item
        /// </summary>
        /// <param name="id">Guid for news item</param>
        /// <param name="name">Name (not used SEO only)</param>
        public ActionResult Item(Guid id)
        {
            News news = reposetory.GetNewsItem(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        public ActionResult Clear()
        {
            reposetory.NotificationClear(CurrentProfile.PersonID);
            
            return RedirectToAction("Index");
        }

       

    }
}