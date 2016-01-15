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
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using DTA;

namespace Local_Homepage.Controllers.Local
{
    public class XMLController : BaseController
    {
        [OutputCache(Duration = 3600, VaryByCustom = "host")]
        public ContentResult Sitemap()
        {
            Initiate();
            AllNewsActivity();


            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            const string url = "http://www.mikesdotnetting.com/Article/Show/{0}/{1}";

            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
               new XElement(ns + "urlset",
                   new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Index", "Local")),
                       Basedata.PageAbout != null ?
                           new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", Basedata.PageAbout.Lastchanged)) :
                           new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddMonths(-3))),

                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                       ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Kontakt", "Local")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.9")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Nyheder", "Local")),
                       new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-2))),
                       new XElement("changefreq", "weekly"),
                       new XElement("priority", "0.9")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Aktiviteter", "Local")),
                       new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-2))),
                       new XElement("changefreq", "weekly"),
                       new XElement("priority", "0.9")
                    ),
                    Basedata.UsePressPage ?
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Presse", "Local")),
                       new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-2))),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.7")
                    ) : null,
                    Basedata.UseLinksPage ?
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Links", "Local")),
                       new XElement("changefreq", "monthly"),
                       new XElement("priority", "0.7")
                    ) : null,
                    Basedata.UseSponsorPage ?
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Sponsorer", "Local")),
                       new XElement("changefreq", "monthly"),
                       new XElement("priority", "0.7")
                    ) : null,
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Konceptet", "OmNatteravnene")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Fakta", "OmNatteravnene")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Regler", "OmNatteravnene")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Logo", "OmNatteravnene")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                    ),
                    new XElement("url",
                       new XElement("loc", Basedata.Host + Url.Action("Landssekretariatet", "OmNatteravnene")),
                       new XElement("changefreq", "yearly"),
                       new XElement("priority", "0.5")
                    ),
                    from item in Basedata.News
                    select
                    new XElement("url",
                      new XElement("loc", Basedata.Host + Url.Action("Nyhed", "Local", new { ID = GuidToUrl(item.NewsId), titel = item.Headline.ValidFileName() })),
                      new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", item.Lastchanged)),
                      new XElement("changefreq", "monthly"),
                      new XElement("priority", "0.5")
                      ),
                    from item in Basedata.Events
                    select
                    new XElement("url",
                      new XElement("loc", Basedata.Host + Url.Action("Aktivitet", "Local", new { ID = GuidToUrl(item.EventID), titel = item.Headline.ValidFileName() })),
                      new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", item.Lastchanged)),
                      new XElement("changefreq", "monthly"),
                      new XElement("priority", "0.5")
                      )

               )
            );



            //var items = repository.GetAllArticleTitles();
            //var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
            //    new XElement(ns + "urlset",
            //        from item in items
            //        select
            //        new XElement("url",
            //          new XElement("loc", string.Format(url, item.ID, UrlTidy.ToCleanUrl(item.Head))),
            //          item.DateAmended != null ?
            //              new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", item.DateAmended)) :
            //              new XElement("lastmod", String.Format("{0:yyyy-MM-dd}", item.DateCreated)),
            //          new XElement("changefreq", "monthly"),
            //          new XElement("priority", "0.5")
            //          )
            //        )
            //      );
            return Content(sitemap.ToString(), "text/xml");
        }


        [Route("robots.txt", Name = "GetRobotsText"), OutputCache(Duration = 86400, VaryByCustom = "host")]
        public ContentResult RobotsText()
        {
            Initiate();
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Format(@"#robots.txt
#
#Generated {1} for {0}
#
# This file is to prevent the crawling and indexing of certain parts
# of your site by web crawlers and spiders run by sites like Yahoo!
# and Google. By telling these 'robots' where not to go on your site,
# you save bandwidth and server resources.
#
# Also it informs the robots about the path to the sitemap file
#
# For more information about the robots.txt standard, see:
# http://www.robotstxt.org/wc/robots.html
#
# For syntax checking, see:
# http://www.sxw.org.uk/computing/robots/check.html
            ", Basedata.Host, DateTime.Now));
            
            stringBuilder.AppendLine("user-agent: *");
            //stringBuilder.AppendLine("disallow: /error/");
            if (Basedata.SEO)
            { 
                stringBuilder.AppendLine("Allow: /");
            }
            else
            {
                stringBuilder.AppendLine("Disallow: /");
            }
            stringBuilder.AppendLine("#Sitemap " + Basedata.Host);
            stringBuilder.Append("sitemap: ");
            stringBuilder.AppendLine(Basedata.Host + Url.Action("Sitemap", "XML"));

            return this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
        }

        //public ContentResult RSS()
        //{
        //    const string url = "http://www.mikesdotnetting.com/Article/Show/{0}/{1}";
        //    var items = repository.GetRSSFeed();
        //    var rss = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
        //      new XElement("rss",
        //        new XAttribute("version", "2.0"),
        //          new XElement("channel",
        //            new XElement("title", "Mikesdotnetting News Feed"),
        //            new XElement("link", "http://www.mikesdotnetting.com/rss"),
        //            new XElement("description", "Latest additions to Mikesdotnetting"),
        //            new XElement("copyright", "(c)" + DateTime.Now.Year + ", Mikesdotnetting. All rights reserved"),
        //          from item in items
        //          select
        //          new XElement("item",
        //            new XElement("title", item.Head),
        //            new XElement("description", item.Intro),
        //            new XElement("link", String.Format(url, item.ID, UrlTidy.ToCleanUrl(item.Head))),
        //            new XElement("pubDate", item.CreatedDate.ToString("R"))

        //          )
        //        )
        //      )
        //    );
        //    return Content(rss.ToString(), "text/xml");
        //}



        public static string GuidToUrl(Guid guid)
        {
            return guid.ToString().Replace("-", "");
        }

    }


    //public class ArticleSUmmery()
    //{
    //    public  string loc


    //}

}