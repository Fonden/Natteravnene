/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DTA;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Configuration;
using NR.Infrastructure;
using System.IO;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
    [HandleError500]
    public class AssociationController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public AssociationController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        //Admin Associations

        public ActionResult Index()
        {

            List<Association> associations = reposetory.GetAssociations().ToList();

            return View(associations);
        }

        public ActionResult SimulateAssociation(Guid Id)
        {
            Association association = reposetory.GetAssociation(Id);
            if (association != null) TempData["AdminAssociationID"] = association.AssociationID;

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? id)
        {
            Association association = new Association();
            if (id != null)
            {
                association = reposetory.GetAssociation((Guid)id);

            }
            else
            {
                association.TeamMessageTeamLeader = DefaultForening.TeamMessageTeamLeader;
                association.TeamMessage = DefaultForening.TeamMessage;
                association.SendTeamText = true;
                association.SendNoteTeamleader = true;
                association.NoteTextTime = Convert.ToInt32(DefaultForening.NoteTextTime);
                association.SendTeamTextDays = Convert.ToInt32(DefaultForening.SendTeamTextDays);
                association.TextServiceProviderUserName = DefaultForening.TextServiceProviderUserName;
                association.TextServiceProviderPassword = DefaultForening.TextServiceProviderPassword;
                association.Scheduletext = DefaultForening.Scheduletext;
                association.Country = Country.DK;
                association.PageSponsor = new Content { Title = DefaultForening.PageContentSponsorTitle, Body = DefaultForening.PageContentSponsor };
                association.PageAbout = new Content { Title = DefaultForening.PageContentAboutTitle, Body = DefaultForening.PageContentAbout };
                association.PagePress = new Content { Title = DefaultForening.PageContentPressTitle, Body = DefaultForening.PageContentPress };
                association.PageLink = new Content { Title = DefaultForening.PageContentLinkTitle, Body = DefaultForening.PageContentLink };
            }

            ViewBag.Networks = reposetory.GetActiveNetworks().Select(d => new SelectListItem { Value = d.NetworkID.ToString(), Text = d.NetworkName }).ToList();

            var SeniorInstructors = reposetory.GetSeniorInstructors().Select(d => new SelectListItem { Value = d.PersonID.ToString(), Text = d.FullName }).ToList();
            SeniorInstructors.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownChoose });
            ViewBag.SeniorInstructors = SeniorInstructors;

            return View(association);

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Association association)
        {
            Association CA = new Association();
            if (association.AssociationID != Guid.Empty)
            {
                CA = reposetory.GetAssociation(association.AssociationID);
                if (CA == null) return RedirectToAction("Index");
            }




            if (ModelState.IsValid)
            {
                bool StatusChanged = false;
                CA.Name = association.Name;
                if (CA.Status != association.Status)
                {
                    CA.Status = association.Status; //TODO: Implement notification if Status changes
                    StatusChanged = true;
                }
                CA.Governance = association.Governance;
                CA.AssociationEmail = association.AssociationEmail;
                CA.Number = association.Number;
                CA.URL = association.URL;
                CA.Established = association.Established;
                CA.CVRNR = association.CVRNR;
                CA.TeamPhone = association.TeamPhone;
                CA.ContactPhone = association.ContactPhone;
                CA.Address = association.Address;
                CA.Zip = association.Zip;
                CA.City = association.City;
                CA.TextServiceProviderUserName = association.TextServiceProviderUserName;
                CA.TextServiceProviderPassword = association.TextServiceProviderPassword;
                CA.SendTeamText = association.SendTeamText;
                CA.SendTeamTextDays = association.SendTeamTextDays;
                CA.TeamMessage = association.TeamMessage;
                CA.TeamMessageTeamLeader = association.TeamMessageTeamLeader;
                CA.SendNoteTeamleader = association.SendNoteTeamleader;
                CA.NoteTextTime = association.NoteTextTime;
                CA.Comments = association.Comments;
                CA.Scheduletext = association.Scheduletext.CleanHTML();
                //Setup
                CA.UseSchedulePlanning = association.UseSchedulePlanning;
                CA.UseShiftTeam = association.UseShiftTeam;
                CA.UseTakeTeamSpot = association.UseTakeTeamSpot;
                CA.UseTeamExchange = association.UseTeamExchange;
                CA.UseKeyBox = association.UseKeyBox;
                CA.UseLists = association.UseLists;
                CA.UsePolicePlanning = association.UsePolicePlanning;
                CA.NetworkID = association.NetworkID;
                CA.SeniorInstructorID = association.SeniorInstructorID;
                if (association.SeniorInstructorID == Guid.Empty) CA.SeniorInstructorID = null;

                CA.UsePressPage = association.UsePressPage;
                CA.UseLinksPage = association.UseLinksPage;
                CA.UseSponsorPage = association.UseSponsorPage;


                if (reposetory.SaveAssociation(CA))
                {
                    ViewBag.FormSucces = true;
                    association.AssociationID = CA.AssociationID;
                    ModelState.Clear();
                    if (StatusChanged)
                    {
                        Notification not = reposetory.Notify(CA, String.Format(Notifications.AssociationStatusChanged, CA.Name, CA.Status.DisplayName()));
                        reposetory.NotifyAddAdministration(not);
                        reposetory.NotifyAddAssociation(not, CA.AssociationID);
                        reposetory.NotifySave(not);
                    }
                }

            }
            ViewBag.Networks = reposetory.GetActiveNetworks().Select(d => new SelectListItem { Value = d.NetworkID.ToString(), Text = d.NetworkName }).ToList();

            var SeniorInstructors = reposetory.GetSeniorInstructors().Select(d => new SelectListItem { Value = d.PersonID.ToString(), Text = d.FullName }).ToList();
            SeniorInstructors.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownChoose });
            ViewBag.SeniorInstructors = SeniorInstructors;

            return View(association);

        }

        public ActionResult View(Guid id)
        {
            Association association = reposetory.GetAssociation(id);
            if (association == null) return RedirectToAction("Index");
            ViewBag.JsonLocations = new JavaScriptSerializer().Serialize(
                                from l in association.Locations
                                where (l.Lat != 0 & l.Lng != 0 & l.Active)
                                select new { Name = l.Name, Lat = l.latstring, Lng = l.lngstring });
            return View(association);
        }

        public ActionResult Location(Guid id)
        {
            Association association = reposetory.GetAssociation((Guid)id);
            if (association == null) return HttpNotFound();
            return View(association);
        }

        public ActionResult LocationCreate(Guid id)
        {
            Association association = reposetory.GetAssociation(id);
            Location location = new Location
            {
                association = association
            };

            return View("LocationEdit", location);
        }

        public ActionResult LocationEdit(Guid id)
        {
            Location location = reposetory.GetLocation(id);

            return View(location);
        }

        [HttpPost]
        public ActionResult LocationEdit(Location location, string Lat, string Lng)
        {
            Location CL = new Location();
            if (location.LocationID != Guid.Empty)
            {
                CL = reposetory.GetLocation(location.LocationID);
                if (CL == null) return HttpNotFound();
            }
            CL.association = location.association = reposetory.GetAssociation(location.association.AssociationID);


            if (ModelState.IsValid)
            {
                location.Trim();
                CL.Name = location.Name;
                CL.ShortName = location.ShortName;
                CL.Description = location.Description;
                CL.Active = location.Active;
                CL.ShowNationalMap = location.ShowNationalMap;
                CL.Lat = location.Lat;
                CL.Lng = location.Lng;
                if (reposetory.SaveLocation(CL)) ViewBag.FormSucces = true;


            }

            return View(CL);
        }

        public ActionResult LocationView(Guid id)
        {
            Location location = reposetory.GetLocation(id);
            if (location == null) return HttpNotFound();
            return View(location);
        }

        public ActionResult EditBoard(Guid id)
        {
            Association association = reposetory.GetAssociation((Guid)id);
            if (association == null) return HttpNotFound();
            BoardModel Management = reposetory.GetBoard(association.AssociationID);
            Management.Active.Insert(0, new Person { FirstName = "-----" });
            Management.All.Insert(0, new Person { FirstName = "-----" });

            int AddMemeber = Management.minMember - Management.BoardMembers.Count();

            for (int i = 1; i <= AddMemeber; i++)
            {
                Management.BoardMembers.Add(Guid.Empty);
            }

            if (Management.Alternate == null || !Management.Alternate.Any())
            {
                Management.Alternate = new List<Guid> { Guid.Empty };
            }

            return View(Management);
        }

        [HttpPost]
        public ActionResult EditBoard(BoardModel ReturnManagement, string Action)
        {
            BoardModel Management = reposetory.GetBoard(ReturnManagement.AssociationID);
            if (Management == null || ReturnManagement.AssociationID == Guid.Empty) return HttpNotFound();
            Management.Active.Insert(0, new Person { FirstName = "-----" });
            Management.All.Insert(0, new Person { FirstName = "-----" });

            Management.Chairmann = ReturnManagement.Chairmann;
            Management.Accountant = ReturnManagement.Accountant;
            Management.Auditor = ReturnManagement.Auditor;
            Management.AuditorAlternate = ReturnManagement.AuditorAlternate;
            Management.BoardMembers = ReturnManagement.BoardMembers == null ? new List<Guid>() : ReturnManagement.BoardMembers;
            Management.Alternate = ReturnManagement.Alternate == null ? new List<Guid>() : ReturnManagement.Alternate;

            int AddMemeber = Management.minMember - Management.BoardMembers.Count();

            for (int i = 1; i <= AddMemeber; i++)
            {
                Management.BoardMembers.Add(Guid.Empty);
            }

            if (Management.Alternate == null || !Management.Alternate.Any())
            {
                Management.Alternate = new List<Guid> { Guid.Empty };
            }

            if (Action == "addMember")
            {
                Management.BoardMembers.Add(Guid.Empty);
                ModelState.Clear();
            }
            if (Action.StartsWith("delMember"))
            {
                int Index = Convert.ToInt32(Action.Substring(10));
                Management.BoardMembers.RemoveAt(Index);
                ModelState.Clear();
            }
            if (Action == "addAlternate")
            {
                Management.Alternate.Add(Guid.Empty);
                ModelState.Clear();
            }
            if (Action.StartsWith("delAlternate"))
            {
                int Index = Convert.ToInt32(Action.Substring(13));
                Management.Alternate.RemoveAt(Index);
                ModelState.Clear();
            }
            if (Action == "EditBoard" & ModelState.IsValid)
            {
                reposetory.SaveBoard(Management);
                ViewBag.FormSucces = true;
            }




            return View(Management);
        }

        public ActionResult Access(Guid id)
        {
            AccessModel access = reposetory.GetAccess((Guid)id);
            access.SelectPersons.RemoveAll(item => access.Form.Exists(p => p.FunctionID == item.FunctionID));

            return View(access);
        }

        [HttpPost]
        public ActionResult Access(AccessModel access, string Action)
        {
            AccessModel dbAccess = reposetory.GetAccess(access.AssociationID);
            access.SelectPersons = dbAccess.SelectPersons;
            if (Action == "add")
            {
                if (access.Form == null) access.Form = new List<PersonAccess>();
                if (!access.Form.Where(f => f.FunctionID == access.AddPerson).Any())
                {
                    access.Form.Add(new PersonAccess
                    {
                        FunctionID = access.AddPerson,
                    });
                }
            }


            foreach (PersonAccess p in access.Form)
            {
                p.FullName = access.SelectPersons.Find(ps => ps.FunctionID == p.FunctionID) == null ? "????" : access.SelectPersons.Find(ps => ps.FunctionID == p.FunctionID).FullName;
            }
            access.SelectPersons.RemoveAll(item => access.Form.Exists(p => p.FunctionID == item.FunctionID));

            if (Action == "Access" & ModelState.IsValid)
            {
                reposetory.SaveAccess(access.Form);
                ViewBag.FormSucces = true;
            }

            return View(access);
        }

        public ActionResult EditNote(Guid Id)
        {
            Association association = reposetory.GetAssociation(Id);

            AssociationNoteEditModel Note = new AssociationNoteEditModel
            {
                Note = association.Comments,
                AssociationID = association.AssociationID
            };

            return View(Note);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNote(AssociationNoteEditModel Note)
        {
            Association association = reposetory.GetAssociation(Note.AssociationID);

            if (ModelState.IsValid)
            {

                association.Comments = "<hr><strong><i class=\"ace-icon fa fa-clock-o bigger-110\"></i> " + DateTime.Now.ToShortDateString() + " " + CurrentProfile.Username + "</strong>: " +
                Note.NewNote + "<hr />" + association.Comments;

                if (reposetory.Save(association))
                {
                    ViewBag.FormSucces = true;
                    ModelState.Clear();
                    Note.Note = association.Comments;
                    Notification not = reposetory.Notify(Note, String.Format(Notifications.AssociationNewNote, association.Name));
                    reposetory.NotifyAddAdministration(not);
                    reposetory.NotifySave(not);
                }
                else
                {
                    ViewBag.HandledException = true;
                }

            }

            return View(Note);
        }

        public ActionResult Logos(Guid Id)
        {
            Association association = reposetory.GetAssociation(Id);
            if (association == null) return RedirectToAction("Index");
            string LogoDirSetting = Url.Content(ConfigurationManager.AppSettings["Logos"]);

            if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

            ViewBag.LogoPath = LogoDirSetting + "/" + association.AssociationID.ToString();

            return View(association);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files, Guid AssociationID)
        {
            string errorMessage = "";

            if (files != null && files.Count() > 0)
            {
                Association association = reposetory.GetAssociation(AssociationID);
                if (association == null) return Json(new { success = false, errorMessage = "" });

                string LogoDirSetting = Url.Content(ConfigurationManager.AppSettings["Logos"]);

                if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

                foreach (var file in files)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (file.ContentLength > 0 && (extension == ".jpg" | extension == ".pdf" | extension == ".emf"))
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath(LogoDirSetting), association.AssociationID.ToString() + extension);
                        file.SaveAs(path);
                       
                    }
                }

                return Json(new { success = true });
            }
            errorMessage = General.FileUploadNoUploaded; //failure

            return Json(new { success = false, errorMessage = errorMessage });

        }

        public ActionResult _ResetLocalPage(Guid Id)
        {
            Association association = reposetory.GetAssociation(Id);
            if (association.PageAbout == null) association.PageAbout = new Content();
            association.PageAbout.Title = DefaultForening.PageContentAboutTitle;
            association.PageAbout.Body = DefaultForening.PageContentAbout;

            if (association.PagePress == null) association.PagePress = new Content();
            association.PagePress.Title = DefaultForening.PageContentPressTitle;
            association.PagePress.Body = DefaultForening.PageContentPress;

            if (association.PageSponsor == null) association.PageSponsor = new Content();
            association.PageSponsor.Title = DefaultForening.PageContentSponsorTitle;
            association.PageSponsor.Body = DefaultForening.PageContentSponsor;

            if (association.PageLink == null) association.PageLink = new Content();
            association.PageLink.Title = DefaultForening.PageContentLinkTitle;
            association.PageLink.Body = DefaultForening.PageContentLink;


            association.UsePressPage = false;
            association.UseSponsorPage = false;
            association.UseLinksPage = false;
            reposetory.SaveAssociation(association);
            return RedirectToAction("Index");
        }


        public ActionResult _DefaultContent(Guid Id)
        {
            Association association = reposetory.GetAssociation(Id);
            if (association.PageAbout == null) association.PageAbout = new Content();
            association.PageAbout.Title = DefaultForening.PageContentAboutTitle;
            association.PageAbout.Body = DefaultForening.PageContentAbout;
            reposetory.SaveAssociation(association);
            return null;
        }


        public ActionResult BirthdayListAssociation()
        {
            Response.ClearContent();
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");


            List<Association> associations = reposetory.GetAssociations().Where(A => A.Status != AssociationStatus.Closed & A.Established != null).OrderBy(A => ((DateTime)A.Established).Month).ThenBy(A => ((DateTime)A.Established).Day).ToList();

            var now = DateTime.Now;
            var afterNow = associations.SkipWhile(dt => IsBeforeNow(now, dt.Established));
            var beforeNow = associations.TakeWhile(dt => IsBeforeNow(now, dt.Established));

            var birthdays = Enumerable.Concat(afterNow, beforeNow);


            Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.AssociationBirthdayList));
            return View(birthdays.ToList());
        }

        private static bool IsBeforeNow(DateTime now, DateTime? dateTime)
        {
            if (dateTime == null) return true;
            DateTime dt = (DateTime)dateTime;
            return dt.Month < now.Month
                || (dt.Month == now.Month && dt.Day < now.Day);
        }

    }
}