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
using DTA;

namespace NR.Controllers
{
    [Authorize]
    public class EmneController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public EmneController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        // GET: Admin/Lead
        public ActionResult Index()
        {
            ViewBag.Create = TempData["Create"];
            ViewBag.ID = TempData["ID"];

            // List<AssociationListModel> tmp = reposetory.GetAssociationList();
            ViewBag.Attach = from LeadStatus d in Enum.GetValues(typeof(LeadStatus))
                             where (d != LeadStatus.New & d != LeadStatus.Acknowledge & d != LeadStatus.Clarification)
                             select new SelectListItem
                             {
                                 Value = ((int)d).ToString(),
                                 Text = d.DisplayName()
                             };
            return View(reposetory.GetLeads(CurrentProfile.AssociationID));
        }

        [AllowAnonymous]
        public ActionResult StatusUpdateRecived(Guid ID)
        {
            Lead dbLead = new Lead();

            dbLead = reposetory.GetLead(ID);
            if (dbLead == null) return null;

            dbLead.Status = LeadStatus.Recived;
            dbLead.RequestUpdateMail = false;

            if (reposetory.Save(dbLead))
            {

                if (dbLead.AssociationID != null)
                {
                    string to = string.Empty;
                    List<string> cc = new List<string>();

                    AccessModel Access = reposetory.GetAccess((Guid)dbLead.AssociationID);

                    BoardModelView Board = reposetory.GetBoardView((Guid)dbLead.AssociationID);

                    to = string.Format("{0} <{1}>", Board.Chairmann.FullName, Board.Chairmann.Email);

                    foreach (PersonAccess M in Access.Form)
                    {
                        if (M.Secretary)
                        {
                            NRMembership p = reposetory.GetMembership(M.FunctionID);
                            //cc += string.Format("{0},", p.Person.Email);
                            cc.Add(string.Format("{0} <{1}>", p.Person.FullName, p.Person.Email));
                        }
                        else if (M.Planner)
                        {
                            NRMembership p = reposetory.GetMembership(M.FunctionID);
                            //cc += string.Format("{0},", p.Person.Email);
                            cc.Add(string.Format("{0} <{1}>", p.Person.FullName, p.Person.Email));
                        }
                    }



                    var mail = new LeadRecived
                    {
                        to = to,
                        cc = cc,
                        lead = dbLead
                    };

                    //return new EmailViewResult(mail);

                    mail.Send();

                }
















                ViewBag.FormSucces = true;
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        public ActionResult StatusUpdate(Guid ID, LeadStatus Status)
        {
            Lead dbLead = new Lead();

            dbLead = reposetory.GetLead(ID);
            if (dbLead == null) return null;

            LeadStatus Oldstatus = dbLead.Status;

            dbLead.Status = Status;
            dbLead.RequestUpdateMail = false;

            if (reposetory.Save(dbLead))
            {
                if (Oldstatus != LeadStatus.Succes & dbLead.Status == LeadStatus.Succes)
                {
                    TempData["Create"] = true;
                    TempData["ID"] = dbLead.LeadID;
                }
                else
                {
                   ViewBag.FormSucces = true;
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult CreatePerson(Guid ID)
        {
            Lead dbLead = new Lead();

            dbLead = reposetory.GetLead(ID);
            if (dbLead == null) return RedirectToAction("Index");


            NRMembership CU = new NR.Models.NRMembership
            {
                Association = reposetory.GetAssociation(CurrentProfile.AssociationID),
                SignupDate = DateTime.Now,
                Type = PersonType.Active,
                Person = new Person
                {
                    Country = Country.DK,
                    FirstName = dbLead.FirstName,
                    FamilyName = dbLead.FamilyName,
                    Address = dbLead.Address,
                    Zip = dbLead.Zip,
                    City = dbLead.City,
                    Phone = dbLead.Phone,
                    Mobile = dbLead.Mobile

                }
            };
            return View("../People/edit", CU);
        }

    }
}