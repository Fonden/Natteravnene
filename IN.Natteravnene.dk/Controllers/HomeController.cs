/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Infrastructure;
using NR.Models;
using OptimizeResponseStream;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Linq;
using System.Web.Script.Serialization;
using NR.Localication;
using System.Configuration;

namespace NR.Controllers
{
    //[Compress]
    //[RemoveWhitespaces]
    [Authorize]
    [HandleError500]
    public class HomeController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public HomeController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!WebSecurity.IsAuthenticated) return View("~/Views/Account/Login.cshtml", new LoginModel());
            
           
            
            FrontModel front = new FrontModel
            {

                News = reposetory.GetFrontNews() ,
                Events = reposetory.GetFrontEvents(),
                MyTeams = reposetory.GetMyTeams(CurrentProfile.PersonID)

            };
            if (CurrentProfile.useTakeTeamSpot)
            {
                int minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
                List<Team> teamsToTake = reposetory.GetOpenTeams(CurrentProfile.PersonID, CurrentProfile.AssociationID, minTeammembers);
                if (teamsToTake != null) front.MyTeams.AddRange(teamsToTake); 
            }

            return View(front);
        }

        public ActionResult AssociationMap()
        {
            var Map = reposetory.GetNationalLocations().Select(x => new
            {
                Name = x.association.Name.Trim() == x.Name.Trim() ? string.Format(General.BrandTitleAssociation, x.association.Name.Trim()) : string.Format(General.BrandTitleAssociation, x.association.Name.Trim()) + " (" + x.Name.Trim() + ")",
                Url = Url.Action("Index", "Association",new { id = x.association.AssociationID}),
                Lat = x.latstring,
                Lng = x.lngstring 
            });

            var serializer = new JavaScriptSerializer();
            ViewBag.JsonLocations = serializer.Serialize(Map);

            return View();
        }



        public ActionResult Content(Guid ID)
        {
            Content content = reposetory.GetContent(ID);
            if (content == null) content = reposetory.GetContent(new Guid("9675bf6c-ddf6-e411-9abb-005056aa2abc")); //TODO: Lave en mangler content
            return View(content);
        }

        public ActionResult GetHelp(Guid ID)
        {
            Content content = reposetory.GetContent(ID);
            //if (content == null) return Http;
            return PartialView(content);
        }
        
        public ActionResult About()
        {
            return View();

        }

        [AllowAnonymous]
        public ActionResult Ping()
        {
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}
