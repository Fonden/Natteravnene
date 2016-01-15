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
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NR.Infrastructure
{
     public class HandleError500 : SmartHandleErrorAttribute
     {
         public HandleError500()
             : base(typeof(Exception), 500, "Error")
         { }

     }

     public class HandleError404 : SmartHandleErrorAttribute
     {
         public HandleError404()
             : base(typeof(Exception), 404, "PageNotFound")
         { }

     }
    
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class SmartHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        Type exceptionType = typeof(Exception);
        int statusCode = 500;
        string viewName = "Error";


        public SmartHandleErrorAttribute(Type exceptionType, int statusCode, string viewName)
        {
            this.exceptionType = exceptionType;
            this.statusCode = statusCode;
            this.viewName = viewName;
        }

        public virtual void OnException(ExceptionContext filterContext)
        {
            //Ex.Assert<ArgumentNullException>(filterContext != null, "filterContext");
            

            var exception = filterContext.Exception;
            if (exception is TargetInvocationException)
                exception = (exception as TargetInvocationException).InnerException;
            if (!exceptionType.IsInstanceOfType(exception)) return; //it's not our exception

            string systemInfo = "> > > Requested URI: " + HttpContext.Current.Request.Url.AbsoluteUri.ToString();
            systemInfo += "\n\r> > > Referrer: " + HttpContext.Current.Request.UrlReferrer;
            systemInfo += "\n\r> > > Authenticated: " + HttpContext.Current.User.Identity.IsAuthenticated;
            systemInfo += "\n\r> > > UserName: " + HttpContext.Current.User.Identity.Name;


            LogFile.Write(exception, "Application_Error: " + systemInfo);

            if (filterContext.ExceptionHandled) return;
            if (!filterContext.HttpContext.IsCustomErrorEnabled) return;
            if (filterContext.Exception == null) return;

            filterContext.Result = new ViewResult
            {
                ViewData = filterContext.Controller.ViewData,
                TempData = filterContext.Controller.TempData,
                ViewName = viewName,
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            //Template method, override this in inherited class to execute custom logic
            //for this exception
            OnExceptionHandled(filterContext);
        }

        //Override this to add post-processing specific to the exception thrown
        protected virtual void OnExceptionHandled(ExceptionContext filterContext)
        { }
    }
}