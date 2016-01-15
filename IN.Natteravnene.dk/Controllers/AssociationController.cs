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
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DTA;
using WebMatrix.WebData;
using System.IO;



namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class AssociationController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public AssociationController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        public ActionResult Index(Guid? id)
        {
            Association association;
            if (id == null)
            {
                association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            }
            else
            {
                association = reposetory.GetAssociation((Guid)id);
            }
            if (association == null) return HttpNotFound();
            ViewBag.JsonLocations = new JavaScriptSerializer().Serialize(
                                from l in association.Locations.Where(L => L.Active == true)
                                where (l.Lat != 0 & l.Lng != 0 & l.Active)
                                select new { Name = l.Name, Lat = l.latstring, Lng = l.lngstring });
            return View(association);
           
        }

        public ActionResult Edit()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association == null) return HttpNotFound();

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

            if (ModelState.ContainsKey("Name")) ModelState["Name"].Errors.Clear(); 

            if (ModelState.IsValid)
            {
                bool StatusChanged = false;
                //CA.Name = association.Name;
                //if (CA.Status != association.Status)
                //{
                //    CA.Status = association.Status; //TODO: Implement notification if Status changes
                //    StatusChanged = true;
                //}
                //CA.Governance = association.Governance;
                CA.AssociationEmail = association.AssociationEmail;
                //CA.Established = association.Established;
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
                CA.DeadlineHoursTeamChange = association.DeadlineHoursTeamChange;
                CA.UsePressPage = association.UsePressPage;
                CA.UseLinksPage = association.UseLinksPage;
                CA.UseSponsorPage = association.UseSponsorPage;





                if (reposetory.SaveAssociation(CA))
                {
                    ViewBag.FormSucces = true;
                    if (StatusChanged)
                    {
                        Notification not = reposetory.Notify(CA, String.Format(Notifications.AssociationStatusChanged, CA.Name, CA.Status.DisplayName()));
                        reposetory.NotifyAddAdministration(not);
                        reposetory.NotifyAddAssociation(not, CA.AssociationID);
                        reposetory.NotifySave(not);
                    }
                }

            }

            return View(association);

        }

        public ActionResult Board(Guid? id)
        {
            BoardModelView Management;
            if (id == null)
            {
                Management = reposetory.GetBoardView(CurrentProfile.AssociationID);
            }
            else
            {
                Management = reposetory.GetBoardView((Guid)id);
            }

            if (Management == null) return HttpNotFound();


            return View(Management);
        }
        
        public ActionResult BoardPartial(Guid id)
        {
            BoardModelView Management;
            if (id == null)
            {
                Management = reposetory.GetBoardView(CurrentProfile.AssociationID);
            }
            else
            {
                Management = reposetory.GetBoardView(id);
            }

            if (Management == null) return HttpNotFound();
            
            
            return PartialView(Management);
        }

        public ActionResult EditBoard()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
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




            return View(Management); //TODO: View does not show ok when changes
        }

        public ActionResult Location()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association == null) return HttpNotFound();
            return View(association);
        }

        public ActionResult LocationCreate()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association == null) return HttpNotFound();
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
                //CL.ShowNationalMap = location.ShowNationalMap;
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

        public ActionResult Access()
        {
            AccessModel access = reposetory.GetAccess(CurrentProfile.AssociationID);
            if (access == null) return HttpNotFound();
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
                p.FullName = access.SelectPersons.Find(ps => ps.FunctionID == p.FunctionID).FullName;
            }
            access.SelectPersons.RemoveAll(item => access.Form.Exists(p => p.FunctionID == item.FunctionID));

            if (Action == "Access" & ModelState.IsValid)
            {
                reposetory.SaveAccess(access.Form);
                ViewBag.FormSucces = true;
                CurrentProfile.Clear();
            }

            return View(access);
        }

        public ActionResult Logo()
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association == null) return HttpNotFound();
            string LogoDirSetting = Url.Content(ConfigurationManager.AppSettings["Logos"]);

            if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

            string Filename = string.Format(General.BrandTitleAssociation, association.Name) + "-logo.";

            ViewBag.LogoPath =   Path.Combine(LogoDirSetting, association.AssociationID + ".jpg");

            if (System.IO.File.Exists(Server.MapPath(Path.Combine(LogoDirSetting, association.AssociationID + ".jpg")))) ViewBag.Logojpg = true;
            if (System.IO.File.Exists(Server.MapPath(Path.Combine(LogoDirSetting, association.AssociationID + ".pdf")))) ViewBag.Logopdf = true;
            if (System.IO.File.Exists(Server.MapPath(Path.Combine(LogoDirSetting, association.AssociationID + ".emf")))) ViewBag.Logoemf = true;

            return View();
        }


        public ActionResult DownloadLogo(string type)
        {
            Association association = reposetory.GetAssociation(CurrentProfile.AssociationID);
            if (association == null) return HttpNotFound();
            string LogoDirSetting = ConfigurationManager.AppSettings["Logos"];

            if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

            string extension = type.ToLower();
            string contentType = "";

            if (type.ToLower() != "jpg" & type.ToLower() != "pdf" & type.ToLower() != "emf") extension = "pdf";

            string Filename = string.Format(General.BrandTitleAssociation, association.Name) + "-logo." + extension;

            switch (extension)
            {
                case "jpg":
                    contentType = "image/jpeg";
                    break;
                case "emf":
                    contentType = "image/x-emf";
                    break;
                default:
                    contentType = "application/pdf";
                    break;

            }

            var LogoFile = Path.Combine(LogoDirSetting, association.AssociationID + "." + extension);

            if (!System.IO.File.Exists(Server.MapPath(LogoFile)))
            {
                LogoFile = Path.Combine(LogoDirSetting, "Natteravnene_logo." + extension);
                Filename = "Natteravnenes-logo." + extension;
            }

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = LogoFile,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            //Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(LogoFile, contentType, Filename);
        }



        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetSelectListAssociations()
        {
            if (!WebSecurity.IsAuthenticated) return null; 
            var list = reposetory.GetAssociations().Where(n => n.Status == AssociationStatus.Active).OrderBy(a => a.Name);

            return Json(new SelectList(list.ToArray(), "AssociationID", "Name"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetSelectListNetworks()
        {
            if (!WebSecurity.IsAuthenticated) return null;
            var list = reposetory.GetAllNetworks().Where(n => n.NetworkNotToShow != true).OrderBy(a => a.NetworkName);

            return Json(new SelectList(list.ToArray(), "NetworkID", "NetworkName"), JsonRequestBehavior.AllowGet);
        }

    }
}