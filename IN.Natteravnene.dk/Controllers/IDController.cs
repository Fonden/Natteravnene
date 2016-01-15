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

namespace IN.Natteravnene.dk.Controllers
{
    [HandleError500]
    public class IDController : Controller
    {
        
         //Repository
        private INRRepository reposetory;

        public IDController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: ID
        [AllowAnonymous]
        public ActionResult Verify(Guid Id)
        {
            Person CU = reposetory.GetPersonComplete(Id); //TODO: Refine

            return View(CU);
        }
    }
}