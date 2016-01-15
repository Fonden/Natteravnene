using Local_Homepage.Code;
using System.Web.Mvc;
using System.Web.Routing;

namespace ASP.NET_MVC5_Bootstrap3_3_1_LESS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Sitemap", "sitemap",
                 new { controller = "XML", action = "Sitemap" },
                constraints: new { LocalAccess = new AssociationRouteConstraint() }
            );

            routes.MapRoute(
                "GetRobotsText", "robots.txt",
                 new { controller = "XML", action = "RobotsText" },
                constraints: new { LocalAccess = new AssociationRouteConstraint() }
            );

            routes.MapRoute(
                name: "Regler",
                url: "Om-Natteravnene/de-5-gyldne-regler/{id}",
                defaults: new { controller = "OmNatteravnene", action = "Regler", id = UrlParameter.Optional },
                constraints: new { LocalAccess = new AssociationRouteConstraint() } 
            );

            routes.MapRoute(
                name: "OmNatteravnene",
                url: "Om-Natteravnene/{action}/{id}",
                defaults: new { controller = "OmNatteravnene", action = "Konceptet", id = UrlParameter.Optional },
                constraints: new { LocalAccess = new AssociationRouteConstraint() } 
            );

            routes.MapRoute(
                name: "Nyhed",
                url: "nyhed/{id}/{titel}",
                defaults: new { controller = "Local", action = "Nyhed" },
                constraints: new { LocalAccess = new AssociationRouteConstraint() }
            );

            routes.MapRoute(
                name: "Aktivitet",
                url: "aktivitet/{id}/{titel}",
                defaults: new { controller = "Local", action = "Aktivitet" },
                constraints: new { LocalAccess = new AssociationRouteConstraint() }
            );

            routes.MapRoute(
                name: "DefaultASP",
                url: "{name}.asp/{*pathInfo}",
                defaults: new { controller = "Local", action = "OldPath", pathInfo = UrlParameter.Optional },
                constraints: new { LocalAccess = new AssociationRouteConstraint() }
            );

            routes.MapRoute(
                name: "TopMenu",
                url: "{action}/{id}",
                defaults: new { controller = "Local", action = "Index", id = UrlParameter.Optional },
                constraints: new { LocalAccess = new AssociationRouteConstraint() } 
            );

 
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Local", action = "Index", id = UrlParameter.Optional },
                constraints: new { LocalAccess = new AssociationRouteConstraint() } 
            );

            routes.MapRoute(
               name: "CatchAll",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Local", action = "NoLocal", id = UrlParameter.Optional }
           );
        }
    }
}