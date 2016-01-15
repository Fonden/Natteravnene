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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IN.Natteravnene.dk.Controllers
{
    [HandleError500]
    public class KeyBoxController : Controller
    {
        
          //Repository
        private INRRepository reposetory;

        public KeyBoxController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: KeyBox
        public ActionResult Index()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);

            return View(association);
        }

        [HttpPost]
        public ActionResult Index(Association result)
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            ModelState.Clear();

            if (Regex.IsMatch(result.KeyBoxcode.Trim().Replace(" ", ""), @"^[a-zA-Z0-9æøåÆØÅ*!#%&]{0,10}$"))
            {
                association.KeyBoxcode = result.KeyBoxcode.Trim().Replace(" ", "");
                ViewBag.FormSucces = true;
                reposetory.SaveAssociation(association);
            }
            else
            {
                ModelState.AddModelError("", @General.ErrorKeybox);
            }

            return View(association);
        }

    }
}