/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System.Collections.Generic;
using System.Web;
using MvcSiteMapProvider;
using System;
using System.Linq;
using System.Web.Security;

namespace NR.Infrastructure
{
    public class NRVisibilityProvider : SiteMapNodeVisibilityProviderBase
    {
        public override bool IsVisible(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {

            return visability(node, sourceMetadata) & rights(node, sourceMetadata) & module(node, sourceMetadata);



        }


        private bool visability(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            // Is a visibility attribute specified?

            // Is a visibility attribute specified?
            string visibility = string.Empty;
            if (node.Attributes.ContainsKey("visibility"))
            {
                visibility = node.Attributes["visibility"].GetType().Equals(typeof(string)) ? node.Attributes["visibility"].ToString() : string.Empty;
            }
            if (string.IsNullOrEmpty(visibility))
            {
                return true;
            }
            visibility = visibility.Trim().Replace(" ", "");

            // Check for the source HtmlHelper
            if (sourceMetadata["HtmlHelper"] == null)
            {
                return true;
            }
            string htmlHelper = sourceMetadata["HtmlHelper"].ToString();
            htmlHelper = htmlHelper.Substring(htmlHelper.LastIndexOf(".") + 1);

            string name = null;
            if (sourceMetadata["name"] != null)
            {
                name = sourceMetadata["name"].ToString();
            }

            // All set. Now parse the visibility variable.
            foreach (string visibilityKeyword in visibility.Split(new[] { ',', ';' }))
            {

                if (visibilityKeyword == htmlHelper || visibilityKeyword == name || visibilityKeyword == "*")
                {
                    return true;
                }
                else if (visibilityKeyword == "!" + htmlHelper || visibilityKeyword == "!" + name || visibilityKeyword == "!*")
                {
                    return false;
                }
            }

            // Still nothing? Then it's OK!
            return true;

        }



        private bool rights(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            // Is a rights attribute specified?

            string rights = string.Empty;
            if (node.Attributes.ContainsKey("rights"))
            {
                rights = node.Attributes["rights"].GetType().Equals(typeof(string)) ? node.Attributes["rights"].ToString() : string.Empty;
            }
            if (string.IsNullOrEmpty(rights))
            {
                return true;
            }
            rights = rights.Trim().Replace(" ", "");

            // Check for the source HtmlHelper
            if (sourceMetadata["HtmlHelper"] == null)
            {
                return false;
            }
            string htmlHelper = sourceMetadata["HtmlHelper"].ToString();
            htmlHelper = htmlHelper.Substring(htmlHelper.LastIndexOf(".") + 1);

            // All set. Now parse the rights variable.
            foreach (string rightsKeyword in rights.Split(new[] { ',', ';' }))
            {
                if (rightsKeyword == "association" && CurrentProfile.hasMembership) return true;
                if (rightsKeyword == "si" && CurrentProfile.isSeniorInstructor) return true;
                if (rightsKeyword == "planner" && CurrentProfile.isPlanner) return true;
                if (rightsKeyword == "secretary" && CurrentProfile.isSecretary) return true;
                if (rightsKeyword == "editor" && CurrentProfile.isEditor) return true;
                if (rightsKeyword == "admin" && Roles.IsUserInRole("Administrator")) return true;
                if (rightsKeyword == "management" && Roles.IsUserInRole("Management")) return true;


            }

            // Still nothing? Then it's not OK!
            return false;

        }


        private bool module(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            // Is a visibility attribute specified?
            string module = string.Empty;
            if (node.Attributes.ContainsKey("module"))
            {
                module = node.Attributes["module"].GetType().Equals(typeof(string)) ? node.Attributes["module"].ToString() : string.Empty;
            }
            if (string.IsNullOrEmpty(module))
            {
                return true;
            }
            module = module.Trim().Replace(" ", "");

            // Check for the source HtmlHelper
            if (sourceMetadata["HtmlHelper"] == null)
            {
                return true;
            }
            string htmlHelper = sourceMetadata["HtmlHelper"].ToString();
            htmlHelper = htmlHelper.Substring(htmlHelper.LastIndexOf(".") + 1);

            string name = null;
            if (sourceMetadata["name"] != null)
            {
                name = sourceMetadata["name"].ToString();
            }

            // All set. Now parse the visibility variable.
            foreach (string visibilityKeyword in module.Split(new[] { ',', ';' }))
            {
                if (visibilityKeyword == "planning" && CurrentProfile.usePlanning) return true;
                if (visibilityKeyword == "keybox" && CurrentProfile.UseKeyBox) return true;
            }

            // Still nothing? Then it's OK!
            return false;

        }



    }
}