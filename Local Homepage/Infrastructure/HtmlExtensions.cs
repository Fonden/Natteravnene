/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

<WORK'S NAME> is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using DTA;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Routing;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Web.Mvc.Html
{


    public static class HtmlExtensions
    {
        public static Uri FullyQualifiedUri(this HtmlHelper html, string relativeOrAbsolutePath, string BurI)
        {
            Uri baseUri = new Uri(BurI);
            string path = UrlHelper.GenerateContentUrl(relativeOrAbsolutePath, new HttpContextWrapper(HttpContext.Current));
            Uri instance = null;
            bool ok = Uri.TryCreate(baseUri, path, out instance);
            return instance;
        }

        public static Uri NewsImagePath<TModel>(this HtmlHelper<TModel> html, News news, string BurI)
        {
            Uri baseUri = new Uri(BurI);
            if (html == null) throw new ArgumentNullException("helper");
            if (news == null || news.NewsId == Guid.Empty)
            {
                return null;
            }

            string NewsDirSetting = ConfigurationManager.AppSettings["NewsImage"];

            if (string.IsNullOrWhiteSpace(NewsDirSetting)) { throw new ArgumentNullException(); }

            var NewsBilled = string.Format(NewsDirSetting, news.NewsId);

            if (!File.Exists(HttpContext.Current.Server.MapPath(NewsBilled)))
            {
                NewsBilled = string.Format(NewsDirSetting, "DefaultNews_" + news.NewsId.ToString().Substring(0, 1).ToUpper());
            }
            Uri instance = null;
            bool ok = Uri.TryCreate(baseUri, NewsBilled.Replace("~", ""), out instance);
            return instance;
        }

        /// <summary>
        /// Generating a img tag with the profile picture. Checking if profile image existe otherwise shows dummy profilepicture
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">Person model</param>
        /// <returns>img tag html string</returns>
        public static MvcHtmlString ProfilImage<TModel>(this HtmlHelper<TModel> html, Person person)
        {
            return ProfilImage(html, person, null);
        }

        /// <summary>
        /// Generating a img tag with the profile picture. Checking if profile image existe otherwise shows dummy profilepicture
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="person">Person model</param>
        /// <param name="htmlAttributes"></param>
        /// <returns>img tag html string</returns>
        public static MvcHtmlString ProfilImage<TModel>(this HtmlHelper<TModel> html, Person person, object htmlAttributes)
        {
            string mainClass = "main";
            if (person == null || person.PersonID == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];

            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }

            var ProfilBilled = string.Format(ProfilDirSetting, person.PersonID);

            if (!File.Exists(HttpContext.Current.Server.MapPath(ProfilBilled)))
            {
                ProfilBilled = string.Format(ProfilDirSetting, "billedemangler");
                mainClass = "main-no-scale";
            }
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);


            TagBuilder tag = new TagBuilder("img");
            //tag.MergeAttribute("class", "profilbillede");
            tag.Attributes.Add("src", urlHelper.Content(ProfilBilled));
            tag.Attributes.Add("alt", person.FullName);
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            //tag.SetInnerText(labelText + ":");

            TagBuilder tagMain = new TagBuilder("div");
            tagMain.MergeAttribute("class", mainClass);
            tagMain.InnerHtml = tag.ToString(TagRenderMode.SelfClosing);
            TagBuilder tagWrapper = new TagBuilder("div");
            tagWrapper.MergeAttribute("class", "profile-image");
            tagWrapper.InnerHtml = tagMain.ToString();

            return MvcHtmlString.Create(tagWrapper.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString NewsImage<TModel>(this HtmlHelper<TModel> html, News news, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (news == null || news.NewsId == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string NewsDirSetting = ConfigurationManager.AppSettings["NewsImage"];

            if (string.IsNullOrWhiteSpace(NewsDirSetting)) { throw new ArgumentNullException(); }

            var NewsBilled = string.Format(NewsDirSetting, news.NewsId);

            if (!File.Exists(HttpContext.Current.Server.MapPath(NewsBilled)))
            {
                NewsBilled = string.Format(NewsDirSetting, "DefaultNews_" + news.NewsId.ToString().Substring(0, 1).ToUpper());
            }
            //if (edit) NewsBilled = NewsBilled + "?update=" + DateTime.Now.Ticks.ToString();
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("img");
            tag.Attributes.Add("src", urlHelper.Content(NewsBilled));
            tag.Attributes.Add("alt", news.Headline);
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString SponsorLogoImage<TModel>(this HtmlHelper<TModel> html, Sponsor sponsor, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (sponsor == null || sponsor.SponsorID == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string SponsorLogoDirSetting = ConfigurationManager.AppSettings["SponsorLogo"];

            if (string.IsNullOrWhiteSpace(SponsorLogoDirSetting)) { throw new ArgumentNullException(); }

            var SponsorLogoBilled = string.Format(SponsorLogoDirSetting, sponsor.SponsorID);

            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(string.Format(SponsorLogoDirSetting, "")), sponsor.SponsorID + ".*");
            if (files.Any())
            {
                SponsorLogoBilled += Path.GetExtension(files[0]);

            }
            else
            {
                SponsorLogoBilled = string.Format(SponsorLogoDirSetting, "Default.png");
            }
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("img");
            tag.Attributes.Add("src", urlHelper.Content(SponsorLogoBilled));
            tag.Attributes.Add("alt", sponsor.Name + " sponsorere Natteravnene");
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Logo<TModel>(this HtmlHelper<TModel> html, Guid AssociationID, string AssociationName)
        {
            return Logo(html, AssociationID, AssociationName, null);
        }

        public static MvcHtmlString Logo<TModel>(this HtmlHelper<TModel> html, Guid AssociationID, string AssociationName, object htmlAttributes)
        {
            if (html == null) throw new ArgumentNullException("helper");
            if (AssociationID == Guid.Empty)
            {
                return MvcHtmlString.Empty;
            }

            string LogoDirSetting = ConfigurationManager.AppSettings["Logos"];

            if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

            var LogoBilled = Path.Combine(LogoDirSetting, AssociationID + ".jpg");

            if (!File.Exists(HttpContext.Current.Server.MapPath(LogoBilled)))
            {
                LogoBilled = Path.Combine(LogoDirSetting, "Natteravnene_logo.jpg");
            }
            //if (edit) NewsBilled = NewsBilled + "?update=" + DateTime.Now.Ticks.ToString();
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder tag = new TagBuilder("img");
            tag.Attributes.Add("src", urlHelper.Content(LogoBilled));
            tag.Attributes.Add("alt", AssociationName + "logo");
            tag.MergeAttributes(ToRouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ExternalURL<TModel>(this HtmlHelper<TModel> html, string URL)
        {

            string result;

            try
            {
                result = new UriBuilder(URL).Uri.ToString();
            }
            catch
            {
                result = URL;
            }
            return MvcHtmlString.Create(result);
        }

        public static string GuidToUrl(Guid guid)
        {
            return guid.ToString().Replace("-", "");
        }

        public static string ValidUrl<TModel>(this HtmlHelper<TModel> html, String str)
        {
            return str.ValidFileName();
        }


        private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
        {
            return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
        }

        
    }
}