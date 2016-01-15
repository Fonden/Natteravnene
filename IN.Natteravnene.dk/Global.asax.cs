/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Controllers;
using NR.Entity;
using NR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;
using System.Web.Caching;
using System.Net;
using dotless.Core.Parser.Tree;
using System.Configuration;
using System.IO;
using DTA;
using System.Globalization;

namespace IN.Natteravnene.dk
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private static string PingUrl;
        private static string HandshakeUrl;

        public static Repository GlobalDbContext;

        protected void Application_Start()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("da-DK");
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            if (Database.Exists("Repository"))
            {
                Database.SetInitializer<Repository>(new MigrateDatabaseToLatestVersion<Repository, IN.Natteravnene.dk.Migrations.Configuration>()); ;
            }
            else
            {
                Database.SetInitializer(new DataContextInitializer());
            }
            GlobalDbContext = new Repository();
            GlobalDbContext.Database.Initialize(true);


            if (!WebSecurity.Initialized) WebSecurity.InitializeDatabaseConnection("Repository", "People", "UserID", "UserName", autoCreateTables: true);
            GlobalDbContext.Dispose();

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
            //ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactoryScope(GlobalDbContext));

            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableDateTimeBinder());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {


            //301 redirect if domain name contains www. (www.donain -> domain)
            if (Request.Url.Host.StartsWith("www.") && !Request.Url.IsLoopback && !Request.IsLocal)
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Host = Request.Url.Host.Remove(0, 4);
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }

            //Scheduler for Sending Team Texts and Message Notifications
            if (HttpContext.Current.Cache["SendTexts"] == null | HttpContext.Current.Cache["SendMessages"] == null)
            {
                DateTime FinishNotification = DateTime.ParseExact(ConfigurationManager.AppSettings["FinishNotifications"], "HH:mm", CultureInfo.InvariantCulture);
                DateTime StartNotification = DateTime.ParseExact(ConfigurationManager.AppSettings["StartNotifications"], "HH:mm", CultureInfo.InvariantCulture);


                if (string.IsNullOrWhiteSpace(PingUrl) | string.IsNullOrWhiteSpace(HandshakeUrl))
                {
                    HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                    UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, RouteTable.Routes.GetRouteData(httpContextWrapper)));
                    PingUrl = urlHelper.Action("Ping", "Home", new { area = "" }, "http");
                    HandshakeUrl = urlHelper.Action("TextXStatus", "Account", new { ID = "##", area = "" }, "http");
                }
                if (HttpContext.Current.Cache["SendTexts"] == null)
                {
                    int minutes;
                    if (!int.TryParse(ConfigurationManager.AppSettings["SpanSendNotification"], out minutes)) minutes = 240;
                    HttpContext.Current.Cache.Add("SendTexts", "jobvalue", null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, JobCacheRemovedText);
                }
                if (HttpContext.Current.Cache["SendMessages"] == null)
                {
                    int minutes;
                    if (!int.TryParse(ConfigurationManager.AppSettings["SpanSendMessage"], out minutes)) minutes = 1440;
                    if (DateTime.Now.AddMinutes(minutes).TimeOfDay > FinishNotification.TimeOfDay & DateTime.Now.AddMinutes(minutes).TimeOfDay < StartNotification.TimeOfDay)
                    {
                        HttpContext.Current.Cache.Add("SendMessages", "jobvalue", null, DateTime.Now.AddMinutes(minutes).Date.Add(StartNotification.TimeOfDay), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, JobCacheRemovedMessages);
                    }
                    else
                    {
                        HttpContext.Current.Cache.Add("SendMessages", "jobvalue", null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, JobCacheRemovedMessages);
                    }
                }
            }
        }

        protected void Application_EndRequest()
        {
            //Show 404 layoutet page
            if (Context.Response.StatusCode == 404)
            {
                string systemInfo = "404 not founf";
                systemInfo += "> > > Requested URI: " + HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                systemInfo += "\n\r> > > Referrer: " + HttpContext.Current.Request.UrlReferrer;
                systemInfo += "\n\r> > > Authenticated: " + HttpContext.Current.User.Identity.IsAuthenticated;
                systemInfo += "\n\r> > >▶ UserName: " + HttpContext.Current.User.Identity.Name;

                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = ""; // In case controller is in another area
                rd.Values["controller"] = "Error";
                rd.Values["action"] = "PageNotFound";

                IController c = new ErrorController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Log Exceptions
            Exception unhandledException = Server.GetLastError();


            if (unhandledException != null && unhandledException.GetBaseException() != null)
            {
                unhandledException = unhandledException.GetBaseException();
            }


            string systemInfo = "";
            if (HttpContext.Current != null)
            {
                systemInfo = "> > > Requested URI: " + HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                systemInfo += "\n\r> > > Referrer: " + HttpContext.Current.Request.UrlReferrer;
                if (HttpContext.Current.User != null) systemInfo += "\n\r> > > Authenticated: " + HttpContext.Current.User.Identity.IsAuthenticated;
                if (HttpContext.Current.User != null) systemInfo += "\n\r> > > UserName: " + HttpContext.Current.User.Identity.Name;
            }

            LogFile.Write(unhandledException, "Application_Error: " + systemInfo);


            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (unhandledException is HttpException)
            {
                var httpEx = unhandledException as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;

                    // others if any

                    default:
                        action = "Index";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = unhandledException is HttpException ? ((HttpException)unhandledException).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData.Model = new HandleErrorInfo(unhandledException, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        //Team text processes
        private static void JobCacheRemovedText(string key, object value, CacheItemRemovedReason reason)
        {
            LogFile.Write("| # | # | # | # | # | # | # | # | # | # | # | # " + DateTime.Now.ToString() + " TEAMMESSAGE START  # | # | # | # | # | # | # | # | # | # | # | # |");

            var Client = new WebClient();
            try
            {
                Client.DownloadData(PingUrl);
            }
            catch (Exception ex)
            {
                LogFile.Write(ex, PingUrl);
            }

            var Update = new Processes(new Repository());
            //var Update = new Processes(GlobalDbContext);
            Update.HandshakeUrl = HandshakeUrl;
            Update.SendTeamTexts();
            LogFile.Write("| # | # | # | # | # | # | # | # | # | # | # | # " + DateTime.Now.ToString() + " TEAMMESSAGE FINISH # | # | # | # | # | # | # | # | # | # | # | # |\r\n");
        }

        //Message Notifications processes
        private static void JobCacheRemovedMessages(string key, object value, CacheItemRemovedReason reason)
        {
            LogFile.Write("| % | % | % | % | % | % | % | % | % | % | % | % " + DateTime.Now.ToString() + " NOTIFICATION START  % | % | % | % | % | % | % | % | % | % | % | % |");

            var Client = new WebClient();
            try
            {
                Client.DownloadData(PingUrl);
            }
            catch (Exception ex)
            {
                LogFile.Write(ex, PingUrl);
            }

            var Update = new Processes(new Repository());
            //var Update = new Processes(GlobalDbContext);
            Update.HandshakeUrl = HandshakeUrl;
            Update.SendMessages();
            LogFile.Write("| % | % | % | % | % | % | % | % | % | % | % | % " + DateTime.Now.ToString() + " NOTIFICATION FINISH % | % | % | % | % | % | % | % | % | % | % | % |\r\n");
        }
    }
}