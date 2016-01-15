using NR.Entity;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using DTA;

namespace Local_Homepage.Code
{
    public class AssociationRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {

            var fullAddress = httpContext.Request.Headers["Host"].Split('.');


            if (fullAddress.Length < 2 | fullAddress.Length > 4) return false;
            if (fullAddress.Length == 4 & fullAddress[0].ToLower() != "www") return false;

            if (!values.ContainsKey("associationId"))
            {
                var associationSubdomain = fullAddress[0];
                if (fullAddress.Length == 4) associationSubdomain = fullAddress[1];

                var subDomainList = new Dictionary<string, Guid>();
                Guid associationId = Guid.Empty;


                if (HttpContext.Current.Cache["subDomainList"] == null)
                {
                    using (var db = new NRDbContext())
                    {
                        var Associations = db.Associations.Where(A => A.Status == NR.Models.AssociationStatus.Active);
                        IdnMapping idn = new IdnMapping();
                        foreach (Association A in Associations)
                        {
                            if (!subDomainList.ContainsKey(A.Name.ValidDKDomainName())) subDomainList.Add(idn.GetAscii(A.Name.ValidDKDomainName()), A.AssociationID);
                            if (!subDomainList.ContainsKey(A.Name.ValidDomainName())) subDomainList.Add(idn.GetAscii(A.Name.ValidDomainName()), A.AssociationID);

                            if (A.URL != null)
                            {
                                string[] Urls = A.URL.Split(',');
                                foreach (string url in Urls)
                                {
                                    if (!string.IsNullOrWhiteSpace(url) && !subDomainList.ContainsKey(url.ValidDKDomainName())) subDomainList.Add(idn.GetAscii(url.ValidDKDomainName()), A.AssociationID);
                                }
                            }
                        }
                    }
                    subDomainList.Add("lokal", new Guid("9fdf690d-5b04-e511-8272-005056aa2abc"));
                    subDomainList.Add("lokaltest", new Guid("2737ed51-d5d6-e411-826d-005056aa2abc"));
                    HttpContext.Current.Cache.Insert("subDomainList", subDomainList, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    subDomainList = (Dictionary<string, Guid>)HttpContext.Current.Cache["subDomainList"];
                }


                if (subDomainList.ContainsKey(associationSubdomain.ToLower()))
                {
                    associationId = subDomainList[associationSubdomain.ToLower()];
                }
                else
                {
                    return false;
                }

                if (httpContext.Request.Headers["Host"].Contains("natteravnene.dk")) values.Add("SEO", true);

                values.Add("associationId", associationId);
            }

            return true;
        }
    }



}