/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class EventController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public EventController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: Event
        [AllowAnonymous]
        public ActionResult Index(int? id)
        {
            if (WebSecurity.IsAuthenticated)
            {
                List<Event> events = reposetory.GetUserEvents(id ?? 0).ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_EventListing", events);
                }
               return View(events);
            }
            if (Request.IsAjaxRequest())
            {
                return null;
            }
            return View("~/Views/Account/Login.cshtml");
        }

        public ActionResult View(Guid Id)
        {
            if (Id == Guid.Empty) return HttpNotFound();
            Event Event = reposetory.GetEventItem(Id);
            if (Event == null) return HttpNotFound();

            return View(Event);
        }

        public ActionResult Edit(Guid? Id)
        {
            Event Event = new Event
            {
                Start = DateTime.Now.AddDays(10),
                Finish = DateTime.Now.AddDays(10).AddHours(4),
                Source = LevelType.National,
                Distribution = LevelType.National
            };
            if (Id != null) Event = reposetory.GetEventItem((Guid)Id);
            if (Event == null) return HttpNotFound();

            return View(Event);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Event Event)
        {

            Event dbEvent = new Event
            {
                Start = DateTime.Now.AddDays(10),
                Finish = DateTime.Now.AddDays(10).AddHours(4),
                Source = LevelType.National,
                Distribution = LevelType.National
            };
            if (Event.EventID != Guid.Empty) dbEvent = reposetory.GetEventItem(Event.EventID);
            if (dbEvent == null) return HttpNotFound();
            if (Event.Start > Event.Finish) ModelState.AddModelError("", General.ErrorStartGreaterFinish);

            if (ModelState.IsValid)
            {
                dbEvent.Headline = Event.Headline;
                dbEvent.Description = Event.Description;
                dbEvent.Location = Event.Location;
                dbEvent.IgnorDistribution = Event.IgnorDistribution;
                dbEvent.Distribution = LevelType.Local;
                dbEvent.DistributionLink = CurrentProfile.AssociationID;
                dbEvent.Source = LevelType.Local;
                dbEvent.SourceLink = CurrentProfile.AssociationID;
                dbEvent.Start = Event.Start;
                dbEvent.Finish = Event.Finish;
                dbEvent.AuthorID = CurrentProfile.PersonID;
                dbEvent.Trim();

                if (reposetory.Save(dbEvent))
                {
                    Event.EventID = dbEvent.EventID;
                    ModelState.Clear();
                    ViewBag.FormSucces = true;
                }
            }




            return View(Event);
        }

    }
}