/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/
using NR.Abstract;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DTA;

namespace NR.Areas.Admin.Controllers
{
    public class DrawController : Controller
    {
        
        //Repository
        private INRRepository reposetory;

        public DrawController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: Admin/Draw
        public ActionResult Index()
        {

            DateTime Threshold = DateTime.Now.AddYears(-1);
            var Persons = reposetory.People
                .Include("Memberships.Association")
                .Where(P => P.Teams.Count(T => T.Status == Models.TeamStatus.OK && T.Start > Threshold) > 3)
                .Select(P => new DrawModel
                {
                    Person = P,
                    Associations = P.Memberships.Select(A => A.Association).ToList(),
                    TCount = P.Teams.Count(T => T.Status == Models.TeamStatus.OK && T.Start > Threshold)
                })
                    .OrderBy(x => Guid.NewGuid())
                .ToList();

            Persons.Shuffle();

            return View(Persons);
            //return View(Persons.Take(10).ToList());
            //return View();
        }


        public ActionResult _Draw()
        {
            DateTime Threshold = DateTime.Now.AddYears(-1);
            var Persons = reposetory.People
                .Include("Memberships.Association")
                .Where(P => P.Teams.Count(T => T.Status == Models.TeamStatus.OK && T.Start > Threshold) > 3)
                .Select(P => new DrawModel { 
                    Person = P,
                    Associations = P.Memberships.Select(A => A.Association).ToList(),
                    TCount = P.Teams.Count(T => T.Status == Models.TeamStatus.OK && T.Start > Threshold) })
                    .OrderBy(x => Guid.NewGuid())
                .ToList();

            Persons.Shuffle();



            return PartialView(Persons.Take(4).ToList());
        }


    }
}