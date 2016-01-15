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
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class EmnerController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public EmnerController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        
        // GET: Admin/Lead
        public ActionResult Index()
        {
            return View(reposetory.GetLeads().Where(L => L.Lastchanged > DateTime.Now.AddDays(-7) || L.Status <= LeadStatus.Trial).ToList());
        }


        public ActionResult SPAM(Guid ID)
        {
            Lead dbLead = new Lead();
            
                dbLead = reposetory.GetLead(ID);
                if (dbLead == null) return RedirectToAction("Index");

            dbLead.Status = LeadStatus.SPAM;

            if (reposetory.Save(dbLead))
                {
                    ViewBag.FormSucces = true;
                }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? ID)
        {
            Lead lead;
            if (ID == null)
            {
                lead = new Lead();
            }
            else
            {
                lead = reposetory.GetLead((Guid)ID);
                if (lead == null) return RedirectToAction("Index");
            }
            List<AssociationListModel> tmp = reposetory.GetAssociationList();
            ViewBag.Attach = new SelectList(tmp, "AssociationID", "Name");

            return View(lead);
        }

        [HttpPost]
        public ActionResult Edit (Lead lead)
        {

            Lead dbLead = new Lead();
            if (lead.LeadID != Guid.Empty)
            {
                dbLead = reposetory.GetLead(lead.LeadID);
                if (dbLead == null) return RedirectToAction("Index");
            }
           
            if (ModelState.IsValid)
            {
                LeadStatus oldStatus = dbLead.Status;
                
                
                
                dbLead.FirstName = lead.FirstName;
                dbLead.FamilyName = lead.FamilyName;
                dbLead.Status = lead.Status;
                dbLead.AssociationID = lead.AssociationID == Guid.Empty ? null : lead.AssociationID;
                dbLead.Address = lead.Address;
                dbLead.City = lead.City;
                dbLead.Zip = lead.Zip;
                dbLead.Phone = lead.Phone;
                dbLead.Mobile = lead.Mobile;
                dbLead.Email = lead.Email;
                dbLead.Age = lead.Age;
                dbLead.Comments = lead.Comments;
                dbLead.RequestUpdateMail = false;

                if (reposetory.Save(dbLead))
                {
                    if ((oldStatus == LeadStatus.New | oldStatus == LeadStatus.Acknowledge | 
                        oldStatus == LeadStatus.Clarification  | lead.LeadID == Guid.Empty) 
                        & dbLead.Status == LeadStatus.Assigned & dbLead.AssociationID != null)
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
                                NRMembership p =  reposetory.GetMembership(M.FunctionID);
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
                        
                        
                        
                        var mail = new LeadAssign
                        {
                            to = to,
                            cc = cc,
                            lead  = dbLead
                        };

                        //return new EmailViewResult(mail);

                        mail.Send();


                    }
                    lead.LeadID = dbLead.LeadID;
                    ModelState.Clear();
                    
                    
                    ViewBag.FormSucces = true;
                }






            }
            
            List<AssociationListModel> tmp = reposetory.GetAssociationList(); //.RemoveAll(item => CU.Memberships.ToList().Exists(p => p.Association.AssociationID == item.AssociationID));
            ViewBag.Attach = new SelectList(tmp, "AssociationID", "Name");
            return View(lead);
        }


        public ActionResult ListLeads()
        {
            Response.ClearContent();
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");

            return View(reposetory.GetLeads());
        }

        public ActionResult StatusUpdateRequest(Guid ID)
        {
            Lead dbLead = new Lead();

            dbLead = reposetory.GetLead(ID);
            if (dbLead == null) return null;
            if (dbLead.AssociationID == null) return null;


            dbLead.RequestUpdateMail = true;

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



                    var mail = new LeadRequestUpdate
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

    }
}