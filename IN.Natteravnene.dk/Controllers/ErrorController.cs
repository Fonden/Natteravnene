/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using NR.Infrastructure;

namespace NR.Controllers
{
    
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            if (WebSecurity.IsAuthenticated) ViewBag.IsAuthenticated = true;
            return View("Error");
        }


        // GET: Error
        public ActionResult PageNotFound()
        {
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (WebSecurity.IsAuthenticated) ViewBag.IsAuthenticated = true;

            Response.StatusCode = 404;
            
            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                result = PartialView("_NotFound", model);

            return result;
        }

        public ActionResult GError()
        {
            throw new ArgumentNullException("TEST"); 

            return null;
        }

       
    }
}