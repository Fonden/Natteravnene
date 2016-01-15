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
using System.Web.Helpers;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Controls;
using System.Web.Security;
using NR.Infrastructure;

//Admin Controller

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
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

            ICollection<Person> Ps = null;

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
            Person CU;
            if (ID == null)
            {
                CU = new Person
                    {
                        Country = Country.DK,
                        Memberships = new List<NRMembership>(),
                        Causes = new List<CausePartisipant>()
                    };
            }
            else
            {
                CU = reposetory.GetPersonComplete((Guid)ID);
                if (CU == null) return RedirectToAction("Index");
            }
            List<AssociationListModel> tmp = reposetory.GetAssociationList(); //.RemoveAll(item => CU.Memberships.ToList().Exists(p => p.Association.AssociationID == item.AssociationID));

            ViewBag.Attach = new SelectList(tmp, "AssociationID", "Name");

            return View(CU);
        }

        [HttpPost]
        public ActionResult Edit(Person person, string RadioGender, string Action, Guid Attach)
        {
            Person CU = new Person();
            if (person.PersonID != Guid.Empty)
            {
                CU = reposetory.GetPersonComplete(person.PersonID);
                if (CU == null) return RedirectToAction("Index");
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
            CU.BasicTrainingDate = person.BasicTrainingDate;
            CU.BirthDate = person.BirthDate;
            CU.EmailNewsLetter = person.EmailNewsLetter;
            CU.PrintNewslettet = person.PrintNewslettet;
            CU.Gender = person.Gender;
            CU.Note = person.Note;
            CU.SeniorInstructor = person.SeniorInstructor;

            if (String.IsNullOrWhiteSpace(CU.Password)) CU.Password = String.Empty.GeneratePassword();

            List<AssociationListModel> tmp = reposetory.GetAssociationList(); //.RemoveAll(item => CU.Memberships.ToList().Exists(p => p.Association.AssociationID == item.AssociationID));
            ViewBag.Attach = new SelectList(tmp, "AssociationID", "Name");

            if (Action == "add" && Attach != Guid.Empty)
            {
                Association association = reposetory.GetAssociation(Attach);
                if (association != null && association.Status == AssociationStatus.Active)
                {
                    NR.Models.NRMembership Mem = new NR.Models.NRMembership
                    {
                        Association = association,
                        SignupDate = DateTime.Now,
                        Type = PersonType.Active
                    };
                    CU.Memberships.Add(Mem);
                    reposetory.SavePerson(CU);
                    ModelState.Clear();
                }

            }


            if (Action == "Edit" & ModelState.IsValid)
            {
                var refresh = person.PersonID == Guid.Empty;
                reposetory.SavePerson(CU);
                if (refresh)
                {
                    //WebSecurity.IsConfirmed 
                    if (!WebSecurity.IsConfirmed(CU.UserName)) WebSecurity.CreateAccount(CU.UserName, CU.Password);
                    if (!string.IsNullOrWhiteSpace(CU.Email))
                    {
                        var mail = new WelcomeMailEmail
                        {
                            To = CU.Email,
                            UserName = CU.UserName,
                            Password = CU.Password
                        };
                        mail.Send();
                    }
                }
                ViewBag.FormSucces = true;
                ModelState.Clear();
            }
            if (CU.Memberships == null) CU.Memberships = new List<NR.Models.NRMembership>();

            if (CU.Causes == null) CU.Causes = new List<CausePartisipant>();
            return View(CU);
        }

        public ActionResult View(Guid Id)
        {
            Person CU = reposetory.GetPersonComplete(Id);
            if (CU == null) return RedirectToAction("Index");

            DisplayProfile Profile = reposetory.GetPersonDisplayProfile(CU.PersonID);
            return View(Profile);
        }

        public ActionResult MakeAbsent(Guid Id)
        {
            NRMembership CU = reposetory.GetMembership(Id);
            if (CU == null) return HttpNotFound();

            if (CU.Person.PersonID != CurrentProfile.PersonID)
            {
                CU.AbsentDate = DateTime.Now;
                CU.Editor = false;
                CU.Secretary = false;
                CU.Planner = false;
                reposetory.SavePerson(CU);
            }

            return RedirectToAction("Edit", new { id = Id });

        }

        public ActionResult ListPeople(int Id)
        {
            Response.ClearContent();
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
           
            switch (Id)
            {
                case 1:
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.BtnExportAllPeople));
                    return View("ListPeople", reposetory.GetAllPersons());

                case 2:
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.BtnExportBoards));
                    return View("ListPeopleBoard", reposetory.GetBoardMembers());

                case 3:
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.BtnExportChairmen));
                    return View("ListPeopleBoard", reposetory.GetChairmen());

                case 5:
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.BtnExportSI));
                    return View("ListPeopleSI", reposetory.GetSeniorInstructors());

                case 6:
                    Response.AddHeader("content-disposition", string.Format(@"attachment;filename=""{0}.xls""", General.BtnExportVoukentereEmails));
                    return View("ListPeopleEmails", reposetory.GetVolenteerPersonEmails());

            }

            return null;
        }


        public ActionResult PrintNewsList()
        {
            IEnumerable<PrintMailingModel> All = reposetory.GetPrintNewsPersons().Select(X => new PrintMailingModel(X)).ToList();

            List<PrintMailingModel> result = new List<PrintMailingModel>();

            //var duplicate = All.GroupBy(x => x.SortAddress).Where(x => x.Count() > 1).Select(x => x);
            var duplicate = All.GroupBy(x => x.SortAddress).Select(x => x);
            foreach (var group in duplicate)
            {
                PrintMailingModel tmpMail = null;
                foreach (PrintMailingModel item in group)
                {
                    if (!string.IsNullOrWhiteSpace(item.Address) & !string.IsNullOrWhiteSpace(item.Zip) & !string.IsNullOrWhiteSpace(item.City))
                    {
                        if (tmpMail == null)
                        {
                            tmpMail = item;
                            tmpMail.Names = new List<NameModel>();
                            tmpMail.Names.Add(new NameModel { FirstName = item.FirstName, FamilyName = item.FamilyName });
                        }
                        else
                        {
                            tmpMail.ID = "00000000000000000000000000000000";
                            tmpMail.UserName += " / " + item.UserName;
                            tmpMail.Names.Add(new NameModel { FirstName = item.FirstName, FamilyName = item.FamilyName });
                        }
                    }
                }
                if (tmpMail != null) result.Add(tmpMail);
            }
            Response.ClearContent();
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            Response.AddHeader("content-disposition", @"attachment;filename=""Natteravne Mailingliste.xls""");
            return View(result.OrderBy(r => r.Country).ThenBy(r => r.Zip).ToList());

        }


        public ActionResult ReturnMail()
        {
            return View();
        }

        public ActionResult ReturnMailInfo(string id, bool info)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            Guid PersonID;
            Person P;
            if (!Guid.TryParse(id, out PersonID))
            {
                P = reposetory.GetPerson(id.Trim());
            }
            else
            {
                P = reposetory.GetPerson(PersonID);
            };


           
            if (P == null) return null;

            if (info)
            {
                return PartialView(P);
            }
            else
            {
                P.MailUndeliverable = true;
                P.MailUndeliverableDate = DateTime.Now;
                reposetory.SavePerson(P);

                Notification not = reposetory.Notify(P, String.Format(Notifications.MailBlocked, P.FullName));
                reposetory.NotifyAddAdministration(not);
                //reposetory.NotifyAddPlanner(not);

                reposetory.NotifySave(not);

                if (!String.IsNullOrWhiteSpace(P.Email))
                {
                    var mail = new MailBlockMail
                    {
                        Email = P.Email,
                        UserName = P.UserName,
                        Password = P.Password 
                    };
                    mail.Send();
                }
                return new EmptyResult();
            }
        }


        [HttpPost]
        public ActionResult ReturnMail(Guid Id)
        {
            return View();
        }

        public ActionResult IDCard(Guid Id)
        {
            string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];
            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) return null;
            Person p = reposetory.GetPersonComplete(Id);
            if (p == null) return null;
            string ProfilImageedPath = Server.MapPath(string.Format(ProfilDirSetting, p.PersonID));
            if (!System.IO.File.Exists(ProfilImageedPath)) return null;

            string idCardPath = Server.MapPath(ConfigurationManager.AppSettings["IDCARD"]);
            if (!System.IO.File.Exists(idCardPath)) return null;

            //ProfileImage parameters
            int ProfilImageX = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDProfilX"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDProfilX"]);
            int ProfilImageY = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDProfilY"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDProfilY"]);
            int ProfilImageH = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDProfilH"]) ? 550 : int.Parse(ConfigurationManager.AppSettings["IDCARDProfilH"]);
            int ProfilImageW = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDProfilW"]) ? 428 : int.Parse(ConfigurationManager.AppSettings["IDCARDProfilW"]);

            //Name Box parameters
            int NameX = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDNameX"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDNameX"]);
            int NameY = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDNameY"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDNameY"]);
            int NameH = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDNameH"]) ? 550 : int.Parse(ConfigurationManager.AppSettings["IDCARDNameH"]);
            int NameW = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDNameW"]) ? 428 : int.Parse(ConfigurationManager.AppSettings["IDCARDNameW"]);

            //QR Box parameters
            int QRX = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDQRX"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDQRX"]);
            int QRY = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDQRY"]) ? 200 : int.Parse(ConfigurationManager.AppSettings["IDCARDQRY"]);
            string QRCR = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IDCARDQRCR"]) ? "M" : ConfigurationManager.AppSettings["IDCARDQRCR"];

            Image idCard = Bitmap.FromFile(idCardPath);
            Image ProfilImage = Bitmap.FromFile(ProfilImageedPath);

            Graphics g = Graphics.FromImage(idCard);

            //Add profile image (Crop and Scale)
            RectangleF destinationRect = new RectangleF(
                    ProfilImageX,
                    ProfilImageY,
                    ProfilImageW,
                    ProfilImageH);

            RectangleF sourceRect;

            if (ProfilImage.Width / ProfilImage.Height <= ProfilImageW / ProfilImageH)
            {
                // Height

                sourceRect = new RectangleF(
                       (ProfilImage.Width - ProfilImage.Height * ProfilImage.Width / ProfilImage.Height) / 2,
                       0,
                       ProfilImage.Height * ProfilImage.Width / ProfilImage.Height,
                       ProfilImage.Height
                    );
            }
            else
            {
                // Width

                sourceRect = new RectangleF(
                       0,
                       (ProfilImage.Height - ProfilImage.Width * ProfilImage.Height / ProfilImage.Width) / 2,
                       ProfilImage.Width,
                       ProfilImage.Width * ProfilImage.Height / ProfilImage.Width
                    );
            }

            //RectangleF sourceRect = new RectangleF(

            g.DrawImage(
                  ProfilImage,
                  destinationRect,
                  sourceRect,
                  GraphicsUnit.Pixel
                  );

            //Name
            using (Font font1 = new Font("Verdana", 9, FontStyle.Bold, GraphicsUnit.Point))
            {
                RectangleF NameRect = new RectangleF(NameX, NameY, NameW, NameH);
                Pen blackPen = new Pen(Color.FromArgb(255, 0, 0, 0), 0);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                g.DrawString(p.FullName, font1, Brushes.Black, NameRect, sf);

                //g.DrawRectangle(null, Rectangle.Round(NameRect));
            }


            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(this.Url.Action("Verify", "ID", new { id = p.PersonID, area = "" }, this.Request.Url.Scheme), out qrCode);
            Renderer renderer = new Renderer(5, Brushes.Black, Brushes.Transparent);
            //Image QRCode = new Bitmap(200, 200);
            renderer.Draw(g, qrCode.Matrix, new Point(QRX, QRY));



            g.Dispose();
            MemoryStream m = new MemoryStream();
            idCard.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
            m.Seek(0, SeekOrigin.Begin);

            FileStreamResult result = new FileStreamResult(m, "image/jpeg");
            result.FileDownloadName = "IDKORT.jpeg";
            return result;

        }

        public ActionResult LoginAs(string UserName)
        {
            Person P = reposetory.GetPerson(UserName);

            if (P != null)
            {
                FormsAuthentication.SetAuthCookie(P.UserName, false);

            }

            return RedirectToAction("Index", "Home");
        }

    }
}