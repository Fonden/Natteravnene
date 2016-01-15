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

namespace NR.Controllers
{
    public class MinutesController : Controller
    {
        
        //Repository
        private INRRepository reposetory;

        public MinutesController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: Minutes
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {

           return null;
        }

    }
}