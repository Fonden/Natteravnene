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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class EventController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public EventController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        } 
        
        // GET: Admin/Event
        public ActionResult Index()
        {
            List<Event> Events = reposetory.GetEvents();
            return View(Events);
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

            if (ModelState.IsValid)
            {
                
                dbEvent.Headline = Event.Headline;
                dbEvent.Description = Event.Description;
                dbEvent.Location = Event.Location;
                dbEvent.IgnorDistribution = Event.IgnorDistribution;
                dbEvent.Distribution = Event.Distribution;
                dbEvent.DistributionLink = Event.DistributionLink;
                dbEvent.Source = Event.Source;
                dbEvent.SourceLink = Event.SourceLink;
                dbEvent.Start = Event.Start;
                dbEvent.Finish = Event.Finish;
                dbEvent.AuthorID = CurrentProfile.PersonID;
                dbEvent.Trim();

                if (reposetory.Save(dbEvent))
                {
                    Event.EventID = dbEvent.EventID;
                    ViewBag.FormSucces = true;
                    ModelState.Clear();
                }
            }




            return View(Event);
        }

        public ActionResult View(Guid Id)
        {
            if (Id == Guid.Empty) return HttpNotFound();
            Event Event = reposetory.GetEventItem(Id);
            if (Event == null) return HttpNotFound();

            return View(Event);
        }

    }
}