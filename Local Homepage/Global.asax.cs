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
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ASP.NET_MVC5_Bootstrap3_3_1_LESS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);




        }


         protected void  Application_BeginRequest()
        {
            //var url = HttpContext.Request.Headers["HOST"];
            //var index = url.IndexOf(".");

            ////if (index < 0)
            ////    return null;

            //var subDomain = url.Substring(0, index);
        }

         public override string GetVaryByCustomString(HttpContext context, string arg)
         {
             if (arg == "host")
             {
                 return context.Request.Headers["host"];
             }

             // whatever you have already, or just String.Empty
             return String.Empty;
         }

    }
}
