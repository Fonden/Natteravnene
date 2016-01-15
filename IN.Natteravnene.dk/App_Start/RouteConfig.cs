using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IN.Natteravnene.dk
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "IDCARD",
                "ID/{id}",
                new { controller = "ID", action = "Verify" },
                namespaces: new[] { "NR.Controllers" } //,
                //new {src = @"(.*?)\.(js|htm|aspx|asp)"},
            );

            routes.MapRoute(
               "SIFiles",
                "SIFil/{*src}",
                new { controller = "SI", action = "DownloadFile" } //,
                //new {src = @"(.*?)\.(js|htm|aspx|asp)"},
            );

            routes.MapRoute(
                "Files",
                "Fil/{*src}",
                 new { area = "", controller = "File", action = "DownloadFile" },
                //new {src = @"(.*?)\.(js|htm|aspx|asp)"},
                namespaces: new[] { "NR.Controllers" }
                );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "NR.Controllers" }
            );


            routes.MapRoute(
                   name: "404-PageNotFound",
                    url: "{*url}",
                    defaults: new { controller = "Error", action = "PageNotFound" }
            );
        }
    }
}