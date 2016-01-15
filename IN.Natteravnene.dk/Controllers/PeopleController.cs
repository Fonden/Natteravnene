/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/
using DataTables.Mvc;
using LINQExtensions;
using NR.Abstract;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using OptimizeResponseStream;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DTA;
using WebMatrix.WebData;
using System.Text.RegularExpressions;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class PeopleController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public PeopleController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        public ActionResult Index()
        {
            ICollection<NRMembership> Ps = reposetory.GetAssociationPersons(CurrentProfile.AssociationID);
            return View(Ps);
        }

        [HttpPost]
        public ActionResult Ajax([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest param)
        {
            if (param.Draw == 0)
            {
                return Json(new
                {
                    draw = param.Draw,
                    error = "ERROR"
                },
               JsonRequestBehavior.AllowGet);
            }

            int recordsTotal = reposetory.People.Count();
            int recordsFiltered = recordsTotal;
            List<PersonAdminList> result = new List<PersonAdminList>();


            if (recordsTotal > 0)
            {

                IOrderedQueryable<Person> FilteredPersons;

                //Filtering
                if (param.Search != null && !string.IsNullOrEmpty(param.Search.Value))
                {
                    FilteredPersons = (IOrderedQueryable<Person>)(from p in reposetory.People.Include(p => p.Memberships.Select(m => m.Association))
                                                                  where p.UserName.Contains(param.Search.Value) | String.Concat(p.FirstName, " ", p.FamilyName).Contains(param.Search.Value) |
                                                                      p.Mobile.Contains(param.Search.Value) | p.Phone.Contains(param.Search.Value) |
                                                                      p.Email.Contains(param.Search.Value)
                                                                  select p);

                    recordsFiltered = (from p in reposetory.People
                                       where p.UserName.Contains(param.Search.Value) | String.Concat(p.FirstName, " ", p.FamilyName).Contains(param.Search.Value) |
                                            p.Mobile.Contains(param.Search.Value) | p.Phone.Contains(param.Search.Value) |
                                            p.Email.Contains(param.Search.Value)
                                       select p).Count();
                }
                else
                {
                    FilteredPersons = (IOrderedQueryable<Person>)reposetory.People;

                }


                //Sorting
                var sortedColumns = param.Columns.GetSortedColumns();
                var isSorted = false;
                string sortdata;
                foreach (var column in sortedColumns)
                {
                    sortdata = column.Data;
                    //Fullname not supportet by Entity ordering
                    if (column.Data == "FullName")
                    {
                        if (!isSorted)
                        {
                            if (column.SortDirection == DataTables.Mvc.Column.OrderDirection.Ascendant)
                            {
                                FilteredPersons = FilteredPersons.OrderBy(p => p.FirstName.Trim()).ThenBy(p => p.FamilyName.Trim());

                            }
                            else
                            {
                                FilteredPersons = FilteredPersons.OrderByDescending(p => p.FirstName.Trim()).ThenByDescending(p => p.FamilyName.Trim());
                            }
                        }
                        else
                        {
                            if (column.SortDirection == DataTables.Mvc.Column.OrderDirection.Ascendant)
                            {
                                FilteredPersons = FilteredPersons.ThenBy(p => p.FirstName).ThenBy(p => p.FamilyName);

                            }
                            else
                            {
                                FilteredPersons = FilteredPersons.ThenByDescending(p => p.FirstName).ThenByDescending(p => p.FamilyName);
                            }
                        }
                        isSorted = true;
                    }

                    else if (!isSorted)
                    {
                        if (column.SortDirection == DataTables.Mvc.Column.OrderDirection.Ascendant)
                        {
                            FilteredPersons = FilteredPersons.OrderBy(column.Data);

                        }
                        else
                        {
                            FilteredPersons = FilteredPersons.OrderByDescending(column.Data);
                        }

                        isSorted = true;
                    }
                    else
                    {
                        if (column.SortDirection == DataTables.Mvc.Column.OrderDirection.Ascendant)
                        {
                            FilteredPersons = FilteredPersons.ThenBy(column.Data);
                        }
                        else
                        {
                            FilteredPersons = FilteredPersons.OrderByDescending(column.Data);
                        }
                    }
                }

                foreach (Person p in FilteredPersons.Skip(param.Start)
                            .Take(param.Length == 0 ? 10 : param.Length).Include("Memberships.Association").Include("Causes.Cause"))
                {
                    result.Add(new PersonAdminList(p));
                }



            }
            return Json(new
            {
                draw = param.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                error = "",
                data = result
            },
            JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(Guid? ID)
        {
            NR.Models.NRMembership CU;
            if (ID == null)
            {
                CU = new NR.Models.NRMembership
                {
                    Association = reposetory.GetAssociation(CurrentProfile.AssociationID),
                    SignupDate = DateTime.Now,
                    Type = PersonType.Active,
                    Person = new Person
                    {
                        Country = Country.DK
                    }
                };
                //CU.Person = new Person();


            }
            else
            {
                CU = reposetory.GetMembership((Guid)ID);
                if (CU == null) return HttpNotFound();
            }


            return View(CU);
        }

        [HttpPost]
        public ActionResult Edit(NRMembership Membership, string RadioGender, string Action)
        {
            NRMembership CU = new NR.Models.NRMembership
             {
                 AssociationID = CurrentProfile.AssociationID,
                 SignupDate = DateTime.Now,
                 Type = PersonType.Active
             };

            CU.Person = new Person();
            if (Membership.MembershipID != Guid.Empty)
            {
                CU = reposetory.GetMembership(Membership.MembershipID);
                if (CU == null) return HttpNotFound();
            }

            //Check for a uniq username against database
            if (Membership.Person.PersonID == Guid.Empty || (Membership.Person.UserName != CU.Person.UserName))
            {
                if (String.IsNullOrWhiteSpace(Membership.Person.UserName) | !reposetory.IsUserNameUniqe(Membership.Person.UserName))
                {
                    string SuggestedUserName = Membership.Person.UserName;
                    if (Membership.Person.PersonID == Guid.Empty)
                    {
                        reposetory.GenerateUniqueUserName(Membership.Person);
                        ModelState.SetModelValue("Person.UserName", new ValueProviderResult(Membership.Person.UserName, Membership.Person.UserName.ToString(), CultureInfo.InvariantCulture));
                        if (!String.IsNullOrWhiteSpace(SuggestedUserName)) ModelState.AddModelError("Person.UserName", string.Format(DomainStrings.UserNameNotUniqueSuggestion, SuggestedUserName));
                    }
                    else
                    {
                        Membership.Person.UserName = CU.Person.UserName;
                        //ModelState.Clear();
                        ModelState.SetModelValue("Person.UserName", new ValueProviderResult(Membership.Person.UserName, Membership.Person.UserName.ToString(), CultureInfo.InvariantCulture));
                        ModelState.AddModelError("Person.UserName", string.Format(DomainStrings.UserNameNotUnique, SuggestedUserName));
                    }
                }
            }

            if (RadioGender == "M") Membership.Person.Gender = Gender.Man;
            if (RadioGender == "F") Membership.Person.Gender = Gender.Woman;
            Membership.Person.Trim();
            if (CU.Person.MailUndeliverable && (
                CU.Person.FirstName != Membership.Person.FirstName |
                CU.Person.FamilyName != Membership.Person.FamilyName |
                CU.Person.Address != Membership.Person.Address |
                CU.Person.City != Membership.Person.City |
                CU.Person.Zip != Membership.Person.Zip
                ))
            {
                CU.Person.MailUndeliverable = false;
                CU.Person.MailUndeliverableDate = null;
            }
            CU.Person.UserName = Membership.Person.UserName;
            CU.Type = Membership.Type;
            CU.Person.FirstName = Membership.Person.FirstName;
            CU.Person.FamilyName = Membership.Person.FamilyName;
            CU.Person.Address = Membership.Person.Address;
            CU.Person.City = Membership.Person.City;
            CU.Person.Zip = Membership.Person.Zip;
            CU.Person.Email = Membership.Person.Email;
            CU.SignupDate = Membership.SignupDate;
            CU.Person.Country = Membership.Person.Country;
            CU.Person.Mobile = Membership.Person.Mobile;
            CU.Person.Phone = Membership.Person.Phone;
            CU.Person.BasicTrainingDate = Membership.Person.BasicTrainingDate;
            CU.Person.BirthDate = Membership.Person.BirthDate;
            CU.Person.EmailNewsLetter = Membership.Person.EmailNewsLetter;
            CU.Person.PrintNewslettet = Membership.Person.PrintNewslettet;
            CU.Person.Gender = Membership.Person.Gender;
            CU.Teamleader = Membership.Teamleader;
            CU.Note = Membership.Note;

            if (String.IsNullOrWhiteSpace(CU.Person.Password)) CU.Person.Password = String.Empty.GeneratePassword();

            if (ModelState.IsValid)
            {
                var refresh = Membership.Person.PersonID == Guid.Empty;
                reposetory.SavePerson(CU);

                //TODO: Remove Websecurity account if status is changed from Active

                if (refresh)
                {
                    //ensure that form i updated if resubmit of form i done
                    //ModelState.SetModelValue("MembershipID", new ValueProviderResult(CU.MembershipID, CU.MembershipID.ToString(), CultureInfo.InvariantCulture));
                    //ModelState.SetModelValue("Person.PersonID", new ValueProviderResult(CU.Person.PersonID, CU.Person.PersonID.ToString(), CultureInfo.InvariantCulture));

                    //Create Websecurity Memebership if account is active
                    if (CU.Type == PersonType.Active)
                    {
                        if (!WebSecurity.IsConfirmed(CU.Person.UserName)) WebSecurity.CreateAccount(CU.Person.UserName, CU.Person.Password);

                        //Send Welcome mail if user has an E-mail
                        if (!String.IsNullOrWhiteSpace(CU.Person.Email))
                        {
                            var mail = new WelcomeMailEmail
                            {
                                To = CU.Person.Email,
                                UserName = CU.Person.UserName,
                                Password = CU.Person.Password
                            };
                            mail.Send();
                        }
                        else if (!String.IsNullOrWhiteSpace(CU.Person.Mobile))
                        {
                            Association association = reposetory.GetAssociation(CU.Person.CurrentAssociation);

#if DUMMYTEXT
                        ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", association == null ? null : association.TextServiceProviderUserName, association == null ? null : association.TextServiceProviderPassword);
#else
                            ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(association.TextServiceProvider, association == null ? null : association.TextServiceProviderUserName, association == null ? null : association.TextServiceProviderPassword);
#endif


                            SMSGateway.FromText = General.SystemTextMessagesFrom;
                            SMSGateway.Message = String.Format(General.SystemTextMessagesWelcome, CU.Person.UserName, CU.Person.Password);
                            SMSGateway.Recipient = new List<Person> { CU.Person };
                            if (association != null)
                            {
                                reposetory.NewTextMessage(SMSGateway, association.AssociationID);
                                SMSGateway.HandShakeUrl = Url.Action("TextXStatus", "Account", new { ID = SMSGateway.TextId }, "http");
                            }

                            if (SMSGateway.Send())
                            {

                            };
                        }

                        Notification not = reposetory.Notify(CU.Person, String.Format(Notifications.NewPerson, CU.Person.FullName));
                        reposetory.NotifyAddAdministration(not);
                        reposetory.NotifyAddAssociation(not, CU.AssociationID);

                        reposetory.NotifySave(not);

                    }
                }
                ModelState.Clear();
                ViewBag.FormSucces = true;
                return View(CU);
            }
            return View(Membership);
        }

        public ActionResult View(Guid? Id)
        {
            Person CU;
            if (Id == null || Id == Guid.Empty)
            {
                CU = reposetory.GetPersonComplete(CurrentProfile.PersonID);
            }
            else
            {
                CU = reposetory.GetPersonComplete((Guid)Id);
            }
            if (CU == null) return HttpNotFound();

            DisplayProfile Profile = reposetory.GetPersonDisplayProfile(CU.PersonID);
            return View(Profile);
        }

        public ActionResult ViewProfile(String Username)
        {
            Person CU = reposetory.GetPerson(Username);

            if (CU == null) return HttpNotFound();

            DisplayProfile Profile = reposetory.GetPersonDisplayProfile(CU.PersonID);

            return View("View", Profile);
        }

        public ActionResult EditProfile()
        {
            Person CU = reposetory.GetPersonComplete(CurrentProfile.PersonID);
            if (CU == null) return HttpNotFound();

            return View(CU);
        }

        [HttpPost]
        public ActionResult EditProfile(Person person, string RadioGender, string Action)
        {
            Person CU = new Person();
            if (person.PersonID != Guid.Empty)
            {
                CU = reposetory.GetPersonComplete(person.PersonID);
                if (CU == null) return HttpNotFound();
            }

            //Check for a uniq username against database
            if (person.PersonID == Guid.Empty || person.UserName != CU.UserName)
            {
                if (String.IsNullOrWhiteSpace(person.UserName) | !reposetory.IsUserNameUniqe(person.UserName))
                {
                    string SuggestedUserName = person.UserName;
                    if (person.PersonID == Guid.Empty)
                    {
                        reposetory.GenerateUniqueUserName(person);
                        ModelState.SetModelValue("UserName", new ValueProviderResult(null, string.Empty, CultureInfo.InvariantCulture));
                        if (!String.IsNullOrWhiteSpace(SuggestedUserName)) ModelState.AddModelError("UserName", string.Format(DomainStrings.UserNameNotUniqueSuggestion, SuggestedUserName));
                    }
                    else
                    {
                        person.UserName = CU.UserName;
                        //ModelState.Clear();
                        ModelState.SetModelValue("UserName", new ValueProviderResult(null, string.Empty, CultureInfo.InvariantCulture));
                        ModelState.AddModelError("UserName", string.Format(DomainStrings.UserNameNotUnique, SuggestedUserName));
                    }

                }
            }
            if (RadioGender == "M") person.Gender = Gender.Man;
            if (RadioGender == "F") person.Gender = Gender.Woman;
            person.Trim();
            if (CU.MailUndeliverable && (
                CU.FirstName != person.FirstName |
                CU.FamilyName != person.FamilyName |
                CU.Address != person.Address |
                CU.City != person.City |
                CU.Zip != person.Zip
                ))
            {
                CU.MailUndeliverable = false;
                CU.MailUndeliverableDate = null;
            }
            CU.UserName = person.UserName;
            CU.FirstName = person.FirstName;
            CU.FamilyName = person.FamilyName;
            CU.Address = person.Address;
            CU.City = person.City;
            CU.Zip = person.Zip;
            CU.Email = person.Email;
            CU.Country = person.Country;
            CU.Mobile = person.Mobile;
            CU.Phone = person.Phone;
            //CU.BasicTrainingDate = person.BasicTrainingDate;
            CU.BirthDate = person.BirthDate;
            CU.EmailNewsLetter = person.EmailNewsLetter;
            CU.PrintNewslettet = person.PrintNewslettet;
            CU.Gender = person.Gender;
            CU.ProfileInfo = person.ProfileInfo;
            //CU.Note = person.Note;
            //CU.SeniorInstructor = person.SeniorInstructor;



            if (ModelState.IsValid)
            {
                var refresh = person.PersonID == Guid.Empty;
                reposetory.SavePerson(CU);
                ViewBag.FormSucces = true;
                CurrentProfile.Clear();
                //if (refresh) return RedirectToAction("Edit", new { ID = CU.PersonID.ToString() });
            }
            //if (CU.Memberships == null) CU.Memberships = new List<NRMembership>();


            return View(CU);
        }

        public ActionResult SendLogin(Guid Id)
        {
            Person CU = reposetory.GetPersonComplete(Id);

            if (CU != null && !String.IsNullOrWhiteSpace(CU.Email))
            {
                var mail = new ForgotLoginEmail
                {
                    To = CU.Email,
                    UserName = CU.UserName,
                    Password = CU.Password
                };
                mail.Send();
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new { errorMessage = @General.MessageSendLogin });
            }
            return RedirectToAction("Index");
        }

        public ActionResult MakeAbsent(Guid Id)
        {
            NRMembership CU = reposetory.GetMembership(Id);
            if (CU == null) return HttpNotFound();

            if (CU.Person.PersonID != CurrentProfile.PersonID && CU.BoardFunction == BoardFunctionType.none)
            {
                CU.AbsentDate = DateTime.Now;
                CU.Editor = false;
                CU.Secretary = false;
                CU.Planner = false;
                reposetory.SavePerson(CU);
            }

            return RedirectToAction("Index");

        }

        public ActionResult ChangePassword()
        {
            return View(new LocalPasswordModel());
        }

        public ActionResult ListPeople()
        {
            List<NRMembership> people = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID);


            Response.ClearContent();
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            Response.AddHeader("content-disposition", @"attachment;filename=""Natteravne Frivillige.xls""");

            return View(people);
        }

        [HttpPost]
        public ActionResult ChangePassword(LocalPasswordModel result)
        {
            Person person = reposetory.GetPerson(CurrentProfile.PersonID);

            if (Regex.IsMatch(result.NewPassword, @"^[a-zA-Z0-9æøåÆØÅ*!#%&]{6,10}$") & ModelState.IsValid)
            {
                //bool changePasswordSucceeded;
                try
                {
                    string userName = User.Identity.Name;
                   if (WebSecurity.ChangePassword(userName, result.OldPassword, result.NewPassword))
                    {
                        person.Password = result.NewPassword;
                        reposetory.SavePerson(person);
                        ViewBag.FormSucces = true;
                        ModelState.Clear();
                        result = new LocalPasswordModel();
                    }
                    else
                    {
                        ModelState.AddModelError("", General.ErrorPassword);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", General.ErrorPassword);
                }
            }
            else
            {
                ModelState.AddModelError("",
                  General.ErrorPassword);
            }
            return View(result);
        }

        public ActionResult Setup()
        {
            Person CU = reposetory.GetPersonComplete(CurrentProfile.PersonID);
            if (CU == null) return HttpNotFound();

            var Associations = CU.Memberships.Select(d => new SelectListItem { Value = d.AssociationID.ToString(), Text = d.Association.Name }).ToList();
            if (Associations == null)
            {
                Associations = new List<SelectListItem>();
                Associations.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownEmpty });
            }
            if (Associations != null) 
            {
                ViewBag.Associations = Associations;
            }

            return View(CU);
        }

        [HttpPost]
        public ActionResult Setup(Person CU)
        {
            Person dbPerson = reposetory.GetPersonComplete(CurrentProfile.PersonID);
            if (CU == null | dbPerson.PersonID != CurrentProfile.PersonID) return HttpNotFound();
 
            dbPerson.ListLines = CU.ListLines < 10 ? 10 : CU.ListLines;
            if (dbPerson.CurrentAssociation != CU.CurrentAssociation) dbPerson.CurrentAssociation = CU.CurrentAssociation; 

            reposetory.SavePerson(dbPerson);
            CurrentProfile.Clear();
            ModelState.Clear();
            ViewBag.FormSucces = true;

            var Associations = dbPerson.Memberships.Select(d => new SelectListItem { Value = d.AssociationID.ToString(), Text = d.Association.Name }).ToList();
            if (Associations == null)
            {
                Associations = new List<SelectListItem>();
                Associations.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = General.DropDownEmpty });
            }
            if (Associations != null)
            {
                ViewBag.Associations = Associations;
            }
            return View(CU);
        }

        [AllowAnonymous]
        public ActionResult Newsletter(Guid? Person, int Id)
        {

            return View("~/Views/Emails/Newsletter.Html.cshtml", new NewsletterModel());
        }

    }
}