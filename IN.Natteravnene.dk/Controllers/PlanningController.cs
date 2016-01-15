/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using DTA;
using NR.Abstract;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class PlanningController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public PlanningController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        public ActionResult Index()
        {
            if (CurrentProfile.AssociationID == Guid.Empty) return RedirectToAction("Index", "Home", new { Area = "" });
            ThePlanModel Plan = new ThePlanModel
            {
                Plan = reposetory.GetAssociationPlan(CurrentProfile.AssociationID),
                association = reposetory.GetAssociation(CurrentProfile.AssociationID)
            };

            return View(Plan);
        }

        public ActionResult Folders()
        {

            List<Folder> Folders = reposetory.GetAssociationFolders(CurrentProfile.AssociationID).ToList();

            return View(Folders);
        }

        public ActionResult CreateFolder()
        {
            Folder Folder = new Folder()
            {
                Start = DateTime.Now.AddMonths(1).Date,
                Finish = DateTime.Now.AddMonths(3).Date.AddDays(1).AddSeconds(-1)


            };

            return View(Folder);

        }

        [HttpPost]
        public ActionResult CreateFolder(Folder Folder)
        {
            Folder.Start = Folder.Start.Date;
            Folder.Finish = Folder.Finish.Date.AddDays(1).AddSeconds(-1);
            Folder.Association = reposetory.GetAssociation(CurrentProfile.AssociationID);

            if (Folder.Start < DateTime.Now) ModelState.AddModelError("Start", General.ErrorStartGreaterToday);
            if (Folder.Start > Folder.Finish) ModelState.AddModelError("Start", General.ErrorStartGreaterFinish);

            if (ModelState.IsValid)
            {
                if (reposetory.SaveFolder(Folder))
                {

                    return RedirectToAction("Folder", new { ID = Folder.FolderID });
                }
            }

            return View(Folder);
        }

        public ActionResult FolderEdit(Guid Id)
        {
            Folder Folder = reposetory.GetFolder(Id);
            if (Folder == null) return HttpNotFound();
            var statuses = from FolderStatus s in Enum.GetValues(typeof(FolderStatus))
                           where (int)s >= (int)Folder.Status
                           select new { ID = (int)s, Name = s.DisplayName() };
            ViewData["Status"] = new SelectList(statuses, "ID", "Name", Folder.Status);

            return View(Folder);
        }

        [HttpPost]
        public ActionResult FolderEdit(Folder Folder)
        {
            Folder dbFolder = reposetory.GetFolder(Folder.FolderID);
            if (dbFolder == null) return HttpNotFound();



            if (dbFolder.Status == FolderStatus.New && Folder.Start < DateTime.Now) ModelState.AddModelError("Start", General.ErrorStartGreaterToday);
            if (dbFolder.Status == FolderStatus.New && Folder.Start > Folder.Finish) ModelState.AddModelError("Start", General.ErrorStartGreaterFinish);

            if (ModelState.IsValid)
            {
                if (dbFolder.Status == FolderStatus.New)
                {
                    dbFolder.Start = Folder.Start.Date;
                    dbFolder.Finish = Folder.Finish.Date.AddDays(1).AddSeconds(-1);
                }
                if ((int)dbFolder.Status < (int)Folder.Status) dbFolder.Status = Folder.Status;
                if (reposetory.SaveFolder(dbFolder))
                {
                    ViewBag.FormSucces = true;
                    //return RedirectToAction("Folder", new { ID = Folder.FolderID });
                }
            }

            var statuses = from FolderStatus s in Enum.GetValues(typeof(FolderStatus))
                           where (int)s >= (int)Folder.Status
                           select new { ID = (int)s, Name = s.DisplayName() };
            ViewData["Status"] = new SelectList(statuses, "ID", "Name", dbFolder.Status);

            return View(Folder);
        }

        public ActionResult FolderDelete(Guid Id)
        {
            if (reposetory.DeleteFolder(Id))
            {
                if (!Request.IsAjaxRequest())
                {
                    return RedirectToAction("Folders");
                }
            }

            return RedirectToAction("Folders");
        }

        public ActionResult Folder(Guid Id)
        {
            Folder Folder = reposetory.GetFolder(Id);
            if (Folder == null) return HttpNotFound();
            return View(Folder);
        }

        public ActionResult FolderPlanning(Guid Id)
        {
            Folder Folder = reposetory.GetFolder(Id);
            if (Folder == null) return HttpNotFound();
            Folder.Teams =  Folder.Teams != null ? Folder.Teams = Folder.Teams.OrderBy(m => m.Start).ToList() : Folder.Teams = new List<Team>();
            return View(Folder);
        }

        public PartialViewResult _FolderPlanning(Guid Id)
        {
            Folder Folder = reposetory.GetFolder(Id);
            if (Folder == null) throw new NullReferenceException("Folder is null");
            Folder.Teams = Folder.Teams != null ? Folder.Teams = Folder.Teams.OrderBy(m => m.Start).ToList() : Folder.Teams = new List<Team>();
            FolderPlanningModel Plannig = new FolderPlanningModel
            {
                Folder = Folder,
                Active = reposetory.GetAssociationActivePersons(Folder.Association.AssociationID),
                FeedBack = reposetory.GetFolderUserAnswers(Folder.FolderID),
                minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]),
                maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"])
            };
            return PartialView(Plannig);
        }

        [HttpPost]
        public PartialViewResult _FPAj(Guid Team, Guid Person)
        {
            if (Team == null || Team == Guid.Empty) return null;
            if (Person == null || Person == Guid.Empty) return null;
            Team theTeam = reposetory.GetTeam(Team);

            Folder Folder = reposetory.GetFolder(theTeam.Folder.FolderID);
            FolderPlanningModel Plannig = new FolderPlanningModel
            {
                Folder = Folder,
                Active = reposetory.GetAssociationActivePersons(Folder.Association.AssociationID),
                FeedBack = reposetory.GetFolderUserAnswers(Folder.FolderID),
                minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]),
                maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"])

            };

            var thePerson = Plannig.Active.Where(A => A.Person.PersonID == Person).FirstOrDefault();
            if (thePerson == null) return null;


            if (theTeam.Teammembers == null) theTeam.Teammembers = new List<Person>();
            if (theTeam.Teammembers.Remove(thePerson.Person))
            {
                //Zero TeamLeader if the removed person is Teamleader 
                if (theTeam.TeamLeaderId == thePerson.Person.PersonID)
                {
                    theTeam.TeamLeaderId = Guid.Empty;
                    //Try to replace teamleader from remaining teamleaders
                    foreach (Person P in theTeam.Teammembers)
                    {
                        var Mem = P.Memberships.Where(M => M.Association.AssociationID == Folder.Association.AssociationID).FirstOrDefault();
                        if (Mem != null && Mem.Teamleader)
                        {
                            theTeam.TeamLeaderId = P.PersonID;
                            break;
                        }
                    }

                }
                reposetory.SaveTeam(theTeam);
            }
            else if (!theTeam.Teammembers.Contains(thePerson.Person) && theTeam.Teammembers.Count() < Plannig.maxTeammembers)
            {
                if (theTeam.TeamLeaderId == Guid.Empty)
                {
                    if (thePerson.Teamleader)
                    {
                        theTeam.TeamLeaderId = thePerson.Person.PersonID;
                        theTeam.Teammembers.Add(thePerson.Person);
                        reposetory.SaveTeam(theTeam);
                    }
                    else if (theTeam.Teammembers.Count() < Plannig.maxTeammembers - 1)
                    {
                        theTeam.Teammembers.Add(thePerson.Person);
                        reposetory.SaveTeam(theTeam);
                    }
                }
                else
                {
                    theTeam.Teammembers.Add(thePerson.Person);
                    reposetory.SaveTeam(theTeam);
                }

            }




            return PartialView("_FolderPlanning", Plannig);

        }

        public PartialViewResult _TTP(Guid Id)
        {
            if (!CurrentProfile.useTakeTeamSpot) return null;
            if (Id == null || Id == Guid.Empty) return null;
            Team theTeam = reposetory.GetTeam(Id);
            if (theTeam.Association == null) return null;
            if (theTeam.Status != TeamStatus.Planned) return null;
            int minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
            int maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"]);
            Person CP = reposetory.GetPerson(CurrentProfile.PersonID);
            if (CP == null) return null;

            if (!theTeam.Teammembers.Contains(CP) && theTeam.Teammembers.Count() < maxTeammembers)
            {
                if (theTeam.TeamLeaderId == Guid.Empty)
                {
                    if (CurrentProfile.isTeamleader)
                    {
                        theTeam.TeamLeaderId = CurrentProfile.PersonID;
                        theTeam.Teammembers.Add(CP);
                        reposetory.SaveTeam(theTeam);
                        Notification not = reposetory.Notify(theTeam, String.Format(Notifications.TakeTeam, CP.FullName));
                        reposetory.NotifyAddPlanner(not, theTeam.Association.AssociationID);
                        reposetory.NotifyAddTeam(not, theTeam);
                        reposetory.NotifySave(not);
                    }
                    else if (theTeam.Teammembers.Count() < maxTeammembers - 1)
                    {
                        theTeam.Teammembers.Add(CP);
                        reposetory.SaveTeam(theTeam);
                        Notification not = reposetory.Notify(theTeam, String.Format(Notifications.TakeTeam, CP.FullName));
                        reposetory.NotifyAddPlanner(not, theTeam.Association.AssociationID);
                        reposetory.NotifyAddTeam(not, theTeam);
                        reposetory.NotifySave(not);
                    }
                }
                else
                {
                    theTeam.Teammembers.Add(CP);
                    reposetory.SaveTeam(theTeam);
                    Notification not = reposetory.Notify(theTeam, String.Format(Notifications.TakeTeam, CP.FullName));
                    reposetory.NotifyAddPlanner(not, theTeam.Association.AssociationID);
                    reposetory.NotifyAddTeam(not, theTeam);
                    reposetory.NotifySave(not);
                }

            }

            return PartialView("_TeamListingRow", theTeam);

        }

        public PartialViewResult _TSU(Guid Id, bool OK)
        {
            if (Id == null || Id == Guid.Empty) return null;
            //Team theTeam = reposetory.GetTeam(Id);
            //if (theTeam.Association == null) return null;
            ////if (theTeam.Status != TeamStatus.Planned) return null;
            ////int minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
            ////int maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"]);
            //theTeam.Status = OK == true ? TeamStatus.OK   : TeamStatus.Cancelled ;

            //reposetory.SaveTeam(theTeam);
            Team theTeam = reposetory.UpdateStatus(Id, OK);
            //if (theTeam != null) return PartialView("_TeamListingRow", theTeam);

            return theTeam == null ? null : PartialView("_TeamListingRow", theTeam);

        }


        public ActionResult Edit(Guid Id)
        {
            if (Request.IsLocal) TempData["ReturnUrl"] = Request.UrlReferrer;
            Team Team = reposetory.GetTeam(Id);
            if (Team == null) return HttpNotFound();
            int minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
            int maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"]);

            var Active = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            Active.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownChoose });
            TeamEditModel Model = new TeamEditModel
            {
                Team = Team,
                Active = Active,
                TeamLeader = Team.TeamLeaderId,
                Teammembers = new List<Guid>(),
                minTeammembers = minTeammembers,
                maxTeammembers = maxTeammembers
            };
            foreach (Person P in Team.Teammembers ?? new List<Person>())
            {
                if (P.PersonID != Team.TeamLeaderId) Model.Teammembers.Add(P.PersonID);
            }
            int EmptyAdd = maxTeammembers - Model.Teammembers.Count - 1; // (Team.Trial ? 2 : 1);
            for (var i = 0; i < EmptyAdd; i++)
            {
                Model.Teammembers.Add(Guid.Empty);
            }


            return View(Model);
        }

        [HttpPost]
        public ActionResult Edit(TeamEditModel response)
        {
            if (response.Team == null || response.Team.TeamID == Guid.Empty) return HttpNotFound();

            Team Team = reposetory.GetTeam(response.Team.TeamID);
            if (Team == null) return HttpNotFound();
            int minTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
            int maxTeammembers = int.Parse(ConfigurationManager.AppSettings["TeamMax"]);
            if (response.Start < Team.Folder.Start | response.Start > Team.Folder.Finish)
            {
                ModelState.AddModelError("Team.Start", General.ErrorStartNotWithinFolderDate);
                ModelState.SetModelValue("Team.Start", new ValueProviderResult(Team.Start, Team.Start.ToString(), CultureInfo.InvariantCulture));
            }
            if (response.Team.Trial && response.Teammembers != null && response.Teammembers.Where(x => x != Guid.Empty).Count() >= maxTeammembers - 1) ModelState.AddModelError("", General.ErrorTrialToManyTeammembers);

            if (ModelState.IsValid)
            {
                Team.Start = response.Start;
                //Team.Duration = response.Team.Duration;
                //Team.Location = response.Team.Location;
                Team.Note = response.Team.Note;
                Team.Star = response.Team.Star;
                Team.Status = response.Team.Status;
                Team.Trial = response.Team.Trial;
                Team.TrialInfo = response.Team.TrialInfo;
                Team.TeamLeaderId = response.TeamLeader;

                Team.Teammembers = new List<Person>();
                if (response.Teammembers != null)
                {
                    foreach (Guid p in response.Teammembers.Where(x => x != Guid.Empty).Distinct())
                    {
                        Person dbP = reposetory.GetPerson(p);
                        if (dbP != null && Team.Teammembers.Count() < maxTeammembers) Team.Teammembers.Add(dbP);
                    }
                }
                Person TeamLeader = reposetory.GetPerson(Team.TeamLeaderId);
                if (TeamLeader != null) Team.Teammembers.Add(TeamLeader);
                if (reposetory.SaveTeam(Team))
                {
                    ViewBag.FormSucces = true;
                    ModelState.Clear();
                    Notification not = reposetory.Notify(Team, String.Format(Notifications.TeamChanged));
                    reposetory.NotifyAddPlanner(not, Team.Association.AssociationID);
                    reposetory.NotifyAddTeam(not, Team);
                    reposetory.NotifySave(not);
                    //if (TempData["ReturnUrl"] != null) return Redirect(((Uri)TempData["ReturnUrl"]).ToString());
                }
            }

            var Active = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            Active.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownChoose });
            response.Teammembers = response.Teammembers == null ? new List<Guid>() : response.Teammembers = response.Teammembers.Where(x => x != Guid.Empty).Distinct().ToList();
            int EmptyAdd = maxTeammembers - response.Teammembers.Count - 1; // (response.Team.Trial ? 2 : 1);
            for (var i = 0; i < EmptyAdd; i++)
            {
                response.Teammembers.Add(Guid.Empty);
            }
            response.Team = Team;
            response.Active = Active;
            response.minTeammembers = minTeammembers;
            response.maxTeammembers = maxTeammembers;

            return View(response);
        }

        public ActionResult DeleteTeam(Guid id)
        {
            Team Team = reposetory.GetTeam(id);
            Guid FolderId = Team.Folder.FolderID;
            if (Team.Folder.Status == FolderStatus.New)
            {
                reposetory.Delete(Team);
            }
            return RedirectToAction("Folder", "Planning", new { id = FolderId });
        }

        public PartialViewResult _EditTeam(Guid Id)
        {
            Team Team = reposetory.GetTeam(Id);


            return PartialView(Team);
        }

        public ActionResult View(Guid Id)
        {
            Team team = reposetory.GetTeam(Id);

            if (team == null) return HttpNotFound();
            List<Team> teams = new List<Team>();
            teams.Add(team);
            return View(teams);
        }

        public ActionResult AddTeams(Guid Id)
        {
            AddTeamsModel result = new AddTeamsModel
            {
                Folder = reposetory.GetFolder(Id),
                LocationsSelectList = reposetory.GetAssociation(CurrentProfile.AssociationID).Locations.Select(d => new SelectListItem { Value = d.LocationID.ToString(), Text = d.Name }).ToList()
            };
            result.StartDate = result.StartTime = result.Folder.Start.Date.AddHours(23).AddMinutes(30);
            result.Duration = new TimeSpan(4, 0, 0);


            return View(result);
        }

        [HttpPost]
        public ActionResult AddTeams(AddTeamsModel result)
        {
            Folder Folder = reposetory.GetFolder(result.Folder.FolderID);
            Association assocoation = reposetory.GetAssociation(CurrentProfile.AssociationID);
            result.Folder = Folder;
            result.LocationsSelectList = reposetory.GetAssociation(CurrentProfile.AssociationID).Locations.Select(d => new SelectListItem { Value = d.LocationID.ToString(), Text = d.Name }).ToList();
            DateTime start = result.StartDate.Date.Add(result.StartTime.TimeOfDay);
            if (ModelState.ContainsKey("Folder.FoldereName")) ModelState["Folder.FoldereName"].Errors.Clear();
            if (start < Folder.Start | start > Folder.Finish) ModelState.AddModelError("", General.ErrorStartNotWithinFolderDate);
            if (result.Duration == new TimeSpan(0, 0, 0)) ModelState.AddModelError("", General.ErrorDurationZero);

            if (ModelState.IsValid)
            {
                Location Loc = reposetory.GetLocation(result.Location);
                for (int i = 0; (int)result.Repetition <= 0 ? i < 1 : start.AddDays(i) < Folder.Finish; i += (int)result.Repetition <= 0 ? 1 : (int)result.Repetition)
                {
                    Team team = new Team
                    {
                        Start = start.AddDays(i),
                        Duration = result.Duration,
                        Location = Loc,
                        Status = TeamStatus.Planned,
                        Association = assocoation
                    };
                    Folder.Teams.Add(team);
                }

                if (reposetory.SaveFolder(Folder)) return RedirectToAction("Folder", null, new { ID = Folder.FolderID });
            }
            return View(result);
        }

        public ActionResult ChangeFolderStatus(Guid Id)
        {
            Folder Folder = reposetory.GetFolder(Id);
            if (Folder == null) return HttpNotFound();

            if (Folder.Status < FolderStatus.Published)
            {
                Folder.Status = Folder.Status.Next();
                reposetory.ChangeStatus(Folder);
            }

            return View("Folder", Folder);
        }

        public ActionResult Communicate(Guid Id)
        {
            List<SelectListItem> AssItems = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            AssItems.Insert(0, new SelectListItem { Text = General.MissingFeedback, Value = MistConversations.Int2Guid(2).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Teamleaders, Value = MistConversations.Int2Guid(1).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.All, Value = MistConversations.Int2Guid(0).ToString() });


            MessageFolderModel Messages = new MessageFolderModel()
            {
                Folder = reposetory.GetFolder(Id),
                List = AssItems,
                NewMessageTo = new List<Guid>()
            };

            Messages.LinkActive = Messages.Folder.Status != FolderStatus.Input;

            return View(Messages);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Communicate(MessageFolderModel MessageContext)
        {
            List<NR.Models.NRMembership> Active = new List<NR.Models.NRMembership>();
            if (MessageContext.NewMessageTo == null || !MessageContext.NewMessageTo.Any())
            {
                ModelState.AddModelError("NewMessageTo", General.ErrorNoRecipients);

            }

            if (MessageContext.Short)
            {
                if (String.IsNullOrWhiteSpace(MessageContext.BodyShort)) ModelState.AddModelError("BodyShort", General.ErrorNoBody);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(MessageContext.Head) & String.IsNullOrWhiteSpace(MessageContext.Body)) ModelState.AddModelError("", General.ErrorNoHeadOrBody);
            }

            if (ModelState.ContainsKey("Folder.FoldereName"))
                ModelState["Folder.FoldereName"].Errors.Clear();

            MessageContext.Folder = reposetory.GetFolder(MessageContext.Folder.FolderID);
            MessageContext.LinkActive = MessageContext.Folder.Status != FolderStatus.Input;

            if (ModelState.IsValid)
            {

                List<Guid> Recivers = new List<Guid>();

                Active = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID);
                Person Sender = reposetory.GetPerson(CurrentProfile.PersonID);


                foreach (Guid PersonID in MessageContext.NewMessageTo)
                {
                    if (PersonID == MistConversations.Int2Guid(0))
                    {
                        Recivers = Active.Select(m => m.Person.PersonID).ToList();
                    }
                    else if (PersonID == MistConversations.Int2Guid(1))
                    {
                        Recivers.AddRange(reposetory.GetAssociationTeamLeadersPersons(CurrentProfile.AssociationID).Select(m => m.PersonID).ToList());
                    }
                    else if (PersonID == MistConversations.Int2Guid(2))
                    {
                        Recivers.AddRange(reposetory.GetAssociationPersonsNoFeedback(CurrentProfile.AssociationID, MessageContext.Folder.FolderID).Select(m => m.PersonID).ToList());
                    }
                    else
                    {
                        Recivers.Add(PersonID);
                    }
                }

                foreach (Guid PID in Recivers)
                {
                    Person P = reposetory.GetPerson(PID);
                    if (!string.IsNullOrWhiteSpace(P.Email))
                    {
                        FeedbackFolder email = new FeedbackFolder();
                        //// set up the email ...
                        email.To = P.Email;
                        email.ReplyTo = CurrentProfile.Email;
                        email.folder = MessageContext.Folder;
                        email.Person = P;
                        email.message = MessageContext.Body;
                        email.Link = MessageContext.Link;
                        try
                        {
                            email.Send();
                        }
                        catch (Exception e)
                        {
                            LogFile.Write(e, "Email:" + P.Email);
                        }
                    }
                    if (MessageContext.Short)
                    {
#if DUMMYTEXT
                        ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", CurrentProfile.TextServiceProviderUserName, CurrentProfile.TextServiceProviderPassword);
#else
                        ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(CurrentProfile.TextServiceProvider, CurrentProfile.TextServiceProviderUserName, CurrentProfile.TextServiceProviderPassword);
#endif

                        if (CurrentProfile.PersonID == Guid.Empty)
                        {
                            SMSGateway.FromText = General.SystemTextMessagesFrom;
                        }
                        else
                        {
                            SMSGateway.From = Sender;
                        }
                        
                        //SMSGateway.FromText = CurrentProfile.PersonID == Guid.Empty ? General.SystemTextMessagesFrom : string.IsNullOrWhiteSpace(CurrentProfile.Mobile) ? General.SystemTextMessagesFrom : CurrentProfile.Mobile;
                        SMSGateway.Message = MessageContext.BodyShort.ReplaceTagPerson(P);

                        //if (MessageContext.Link) SMSGateway.Message += "\r\n" + string.Format(General.FeedbackTextLink, Url.Action("FeedBack", "Planning", new { P = PID, F = MessageContext.Folder.FolderID }, "http"));
                        SMSGateway.Recipient = new List<Person> { P };
                        if (DateTime.Now.AddMinutes(1).Hour < 11) SMSGateway.DeliveryTime = DateTime.Now.AddMinutes(1).Date.AddHours(11);
                        reposetory.NewTextMessage(SMSGateway, CurrentProfile.AssociationID);

                        SMSGateway.HandShakeUrl = Url.Action("TextXStatus", "Account", new { ID = SMSGateway.TextId }, "http");

                        if (SMSGateway.Send())
                        {

                        };
                    }

                }

                ViewBag.FormSucces = true;

            }


            List<SelectListItem> AssItems = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            AssItems.Insert(0, new SelectListItem { Text = General.MissingFeedback, Value = MistConversations.Int2Guid(2).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Teamleaders, Value = MistConversations.Int2Guid(1).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.All, Value = MistConversations.Int2Guid(0).ToString() });

            MessageContext.List = AssItems;

            return View(MessageContext);
        }

        [ChildActionOnly]
        public ActionResult UserHold()
        {


            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult FeedBack(Guid? P, Guid? F)
        {
            if (WebSecurity.IsAuthenticated) ViewBag.IsAuthenticated = true;
            if (P == null || (Guid)P == Guid.Empty) return View("FeedbackWrong");
            if (F == null || (Guid)F == Guid.Empty) return View("FeedbackWrong");

            Folder Folder = reposetory.GetFolder((Guid)F);
            Folder.Teams = Folder.Teams.OrderBy(T => T.Start).ToList();
            Person Person = reposetory.GetPersonComplete((Guid)P);
            //Association Association = Folder.Association;
            if (Folder == null | Person == null) return View("FeedbackWrong");

            //Check if person has membership with the association
            if (Person.Memberships.Where(m => m.Association.AssociationID == Folder.Association.AssociationID && !m.Absent && m.Type == PersonType.Active).Count() < 1) return HttpNotFound();

            FolderUserAnswer Answer = reposetory.GetFolderUserAnswer((Guid)P, (Guid)F);

            if (Answer == null)
            {

                Answer = new FolderUserAnswer
                {
                    Folder = Folder,
                    FolderID = Folder.FolderID,
                    Answers = new List<TeamUserAnswer>(),
                    Person = Person,
                    PersonID = Person.PersonID,
                    Pass = false

                };
            }

            foreach (Team T in Folder.Teams)
            {
                TeamUserAnswer TA = Answer.Answers.Where(a => a.TeamID == T.TeamID).FirstOrDefault();

                if (TA == null)
                {
                    Answer.Answers.Add(new TeamUserAnswer
                        {
                            OK = true,
                            Team = T,
                            TeamID = T.TeamID
                        });
                }
                else
                {
                    TA.Team = T;
                    TA.TeamID = T.TeamID;
                }
            }

            if (WebSecurity.IsAuthenticated) ViewBag.IsAuthenticated = true;

            return View(Answer);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult FeedBack(FolderUserAnswer Answer)
        {
            Folder Folder = reposetory.GetFolder(Answer.FolderID);
            Folder.Teams = Folder.Teams.OrderBy(T => T.Start).ToList();
            Person Person = reposetory.GetPersonComplete(Answer.PersonID);
            if (Folder == null | Person == null) return HttpNotFound();

            //Check if person has membership with the association
            if (Person.Memberships.Where(m => m.Association.AssociationID == Folder.Association.AssociationID && !m.Absent && m.Type == PersonType.Active).Count() < 1) return HttpNotFound();

            FolderUserAnswer dbAnswer = reposetory.GetFolderUserAnswer(Person.PersonID, Folder.FolderID);

            if (ModelState.IsValid)
            {
                if (dbAnswer == null)
                {
                    dbAnswer = new FolderUserAnswer
                    {
                        //Folder = Folder,
                        FolderID = Folder.FolderID,
                        Answers = new List<TeamUserAnswer>(),
                        //Person = Person,
                        PersonID = Person.PersonID
                    };
                }

                dbAnswer.Pass = Answer.Pass == null ? false : Answer.Pass;
                dbAnswer.MaxTeams = Answer.MaxTeams;
                if (Answer.Answers != null && Answer.Answers.Any())
                { 
                foreach (TeamUserAnswer T in Answer.Answers)
                {
                    TeamUserAnswer dbT = T.AnswerID == Guid.Empty ? null : dbAnswer.Answers.Where(A => A.AnswerID == T.AnswerID).FirstOrDefault();
                    if (dbT == null)
                    {
                        dbAnswer.Answers.Add(new TeamUserAnswer
                        {
                            OK = T.OK,
                            //PersonID = Person.PersonID,
                            TeamID = T.TeamID
                            //,
                            //FolderUserAnswer = dbAnswer
                        });
                    }
                    else
                    {
                        dbT.OK = T.OK;
                    }
                }
                }
                else
                { 
                }
                if (reposetory.SaveFolderUserAnswer(dbAnswer))
                {
                    ViewBag.FormSucces = true;
                    Notification not = reposetory.Notify(Folder, String.Format(Notifications.FolderFedbackCompleated, Person.FullName, Folder.FoldereName));
                    reposetory.NotifyAddPlanner(not, Folder.Association.AssociationID);
                    reposetory.NotifySave(not);
                }
                Answer = dbAnswer;

            }



            Answer.Folder = Folder;
            Answer.Person = Person;
            foreach (Team T in Folder.Teams)
            {
                TeamUserAnswer TA = Answer.Answers.Where(a => a.TeamID == T.TeamID).FirstOrDefault();

                if (TA == null)
                {
                    Answer.Answers.Add(new TeamUserAnswer
                    {
                        OK = true,
                        Team = T,
                        TeamID = T.TeamID
                    });
                }
                else
                {
                    TA.Team = T;
                    TA.TeamID = T.TeamID;
                }
            }

            if (WebSecurity.IsAuthenticated) ViewBag.IsAuthenticated = true;

            return View(Answer);
        }


    }
}