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
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class CauseController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public CauseController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: Admin/Cause
        public ActionResult Index()
        {

            List<Cause> causes = reposetory.GetCauses().OrderByDescending(n => n.Name).ToList();
            return View(causes);
        }

        public ActionResult Edit(Guid? Id)
        {
            Cause Cause = new Cause
            {

            };
            if (Id != null) Cause = reposetory.GetCauseItem((Guid)Id);
            if (Cause == null) return HttpNotFound();

            return View(Cause);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Cause Cause)
        {

            Cause dbCause = new Cause
            {

            };
            if (Cause.CauseID != Guid.Empty) dbCause = reposetory.GetCauseItem(Cause.CauseID);
            if (dbCause == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                dbCause.Name = Cause.Name;
                dbCause.Description = Cause.Description;

                dbCause.Trim();

                if (reposetory.Save(dbCause))
                {
                    Cause.CauseID = dbCause.CauseID;
                    ViewBag.FormSucces = true;
                }
            }

            return View(Cause);
        }

    }
}