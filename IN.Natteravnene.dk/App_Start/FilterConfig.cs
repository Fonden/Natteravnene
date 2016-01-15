using NR.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace IN.Natteravnene.dk
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute(), 99);
        }
    }
}