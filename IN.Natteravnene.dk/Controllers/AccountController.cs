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
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using System.Transactions;
using System.Net;

//Controllers

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class AccountController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public AccountController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (WebSecurity.IsAuthenticated) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                LogFile.Write(model.UserName.ToUpper() + " loged in < < <");
                CurrentProfile.Clear();
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            LogFile.Write(model.UserName.ToUpper() + " failed login < < <");
            ModelState.AddModelError("", General.LoginError);
            return View(model);
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            CurrentProfile.Clear();
            return RedirectToLocal("~/");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RetriveLogin(string email, string mobile, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            List<Person> Persons = reposetory.GetPeople(email, mobile);
            LogFile.Write(string.Format("Retrive login {0}/{1}",email, mobile));

            if (Persons != null)
            {
                foreach (Person P in Persons)
                {
                    if (!WebSecurity.Login(P.UserName, P.Password, persistCookie: false))
                    {
                        if (WebSecurity.ResetPassword(WebSecurity.GeneratePasswordResetToken(P.UserName), P.Password))
                        {

                        }
                    }
                    else
                    {
                        WebSecurity.Logout();
                    }

                    if (!String.IsNullOrWhiteSpace(P.Email))
                    {
                        var mail = new ForgotLoginEmail
                        {
                            To = P.Email,
                            UserName = P.UserName,
                            Password = P.Password
                        };
                        try
                        {
                            mail.Send();
                        }
                        catch (Exception Ex)
                        {
                            LogFile.Write(Ex, "Send Mail Retrive Login");
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(P.Mobile) && P.CurrentAssociation != Guid.Empty)
                    {
                        Association association = reposetory.GetAssociation(P.CurrentAssociation);

#if DUMMYTEXT
                        ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", association == null ? null : association.TextServiceProviderUserName, association == null ? null : association.TextServiceProviderPassword);
#else
                        ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(association.TextServiceProvider, association == null ? null : association.TextServiceProviderUserName, association == null ? null : association.TextServiceProviderPassword);
#endif

                        SMSGateway.FromText = General.SystemTextMessagesFrom;
                        SMSGateway.Message = String.Format(General.SystemTextMessagesForgetLogin, P.UserName, P.Password);
                        SMSGateway.Recipient = new List<Person> { P };
                        reposetory.NewTextMessage(SMSGateway, association.AssociationID);
                        SMSGateway.HandShakeUrl = Url.Action("TextXStatus", "Account", new { ID = SMSGateway.TextId }, "http");

                        if (SMSGateway.Send())
                        {
                            
                        };
                    }

                }
            }

            ViewBag.LoginRetrived = true;

            return View("Login");
        }

        public ActionResult ResetPassword(Guid Id)
        {

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string dere)
        {

            return View();
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }



        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult UserStatusBar()
        {


            return PartialView("UserStatusBar", CurrentProfile.userbar);
        }

        public ActionResult NotificationLink(Guid Id)
        {
            Notification Not = reposetory.NotificationRead(Id, CurrentProfile.PersonID);
            if (Not != null)
            {
                CurrentProfile.Clear();
                switch (Not.Type)
                {
                    case NotificationType.Folder:
                        return RedirectToAction("FolderPlanning", "Planning", new { id = Not.Link, Area = "" });
                        break;

                    case NotificationType.Team:
                        return RedirectToAction("View", "Planning", new { id = Not.Link, Area = "" });
                        break;

                    case NotificationType.Person:
                        return RedirectToAction("View", "People", new { id = Not.Link, Area = "" });
                        break;

                    case NotificationType.NewVersion:
                        return RedirectToAction("About", "Home", new { Area = "" });
                        break;

                    case NotificationType.Association:
                        return RedirectToAction("Index", "Association", new { id = Not.Link, Area = "" });
                        break;

                    case NotificationType.Content:
                        return RedirectToAction("Content", "Home", new { id = Not.Link, Area = "" });
                        break;

                    case NotificationType.AssociationNote:
                        return RedirectToAction("View", "Association", new { id = Not.Link, Area = "Admin" });
                        break;

                }
            }
            return RedirectToAction("MyNotifications", "Notification", new { id = Not.NotificationID, Area = "" });
        }

        [AllowAnonymous]
        public ActionResult TextXStatus(string ID, string status, string receiver)
        {
            int TextID = 0;
            if (string.IsNullOrWhiteSpace(ID) || !int.TryParse(ID, out TextID)) new HttpStatusCodeResult(HttpStatusCode.OK);
            int SendStatus = 0;
            if (string.IsNullOrWhiteSpace(status) || !int.TryParse(status, out SendStatus)) new HttpStatusCodeResult(HttpStatusCode.OK);
            SMSStatus ST = (SMSStatus)SendStatus;
            reposetory.UpdateStatusMessage(TextID, ST);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Settings()
        {
            PersonSettings Settings = new PersonSettings
            {
                ListLines = CurrentProfile.ListLines,
                EmailNewsLetter = CurrentProfile.EmailNewsLetter,
                PrintNewslettet = CurrentProfile.PrintNewslettet,
                //Associations = CurrentProfile.Person.Associations == null ? null : CurrentProfile.Person.Associations.ToList()
            };

            return View(Settings);
        }

        [HttpPost]
        public ActionResult Settings(PersonSettings Settings)
        {
            if (Settings.ListLines > 100) Settings.ListLines = 100;
            if (Settings.ListLines < 10) Settings.ListLines = 10;

            Person CU = reposetory.GetPerson(Settings.PersonID);
            CU.ListLines = Settings.ListLines;
            CU.CurrentAssociation = Settings.CurrentAssociation;
            CU.EmailNewsLetter = Settings.EmailNewsLetter;
            CU.PrintNewslettet = Settings.PrintNewslettet;

            if (!reposetory.SavePerson(CU)) ModelState.AddModelError("", General.ErrorSave);

            return View(Settings);
        }

        public ActionResult EditProfile(string username)
        {
            Person CU;
            if (username == null)
            { CU = reposetory.CurrentUser(); }
            else
            {
                CU = reposetory.GetPerson(username);
            }

            NR.Models.PersonProfile VM = new NR.Models.PersonProfile();
            VM.PersonID = CU.PersonID;
            VM.Username = CU.UserName;
            VM.FirstName = CU.FirstName;
            VM.FamilyName = CU.FamilyName;
            VM.Email = CU.Email;
            VM.Mobil = CU.Mobile;
            VM.Phone = CU.Phone;
            VM.Gender = CU.Gender;
            VM.BirthDate = CU.BirthDate;
            VM.Address = CU.Address;
            VM.Zip = CU.Zip;
            VM.City = CU.City;
            VM.Country = CU.Country;

            return View(VM);
        }

        [HttpPost]
        public ActionResult EditProfile(NR.Models.PersonProfile profile, string RadioGender, HttpPostedFileBase file, double? x, double? y, double? w, double? h)
        {
            Person CU = null;

            profile.Username = profile.Username.ToUpper();
            if (RadioGender == "M") profile.Gender = Gender.Man;
            if (RadioGender == "F") profile.Gender = Gender.Woman;
            if (String.IsNullOrWhiteSpace(profile.Email) & string.IsNullOrWhiteSpace(profile.Mobil) & string.IsNullOrWhiteSpace(profile.Phone)) ModelState.AddModelError("", General.ErrorNoContactInfo);

            if (ModelState.IsValid)
            {
                CU = reposetory.GetPerson(profile.PersonID);
                if (profile.PersonID != CU.PersonID) return RedirectToAction("EditProfile");//TODO: Null check
                if (profile.Username != CU.UserName && !reposetory.IsUserNameUniqe(profile.Username.ToUpper())) ModelState.AddModelError("Username", General.ErrorUsernameNotUnique);

                if (!reposetory.SavePerson(CU)) ModelState.AddModelError("Username", General.ErrorSave);
                ViewBag.FormSucces = true;
            }


            if (file != null && file.ContentLength > 0)
            {
                if (CU == null) CU = reposetory.GetPerson(profile.PersonID);
                if (profile.PersonID != CU.PersonID) return RedirectToAction("EditProfile");

                if (CU.PersonID != Guid.Empty)
                {
                    Image image = Image.FromStream(file.InputStream);
                    if (image.Width > 428 & image.Height > 550)
                    {
                        var fileExt = Path.GetExtension(file.FileName);
                        string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];
                        if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }
                        var path = Server.MapPath(String.Format(ProfilDirSetting, CU.PersonID.ToString()));
                        file.SaveAs(path);
                        ViewBag.CropImage = true;
                        ViewBag.ImageWidth = image.Width;
                        ViewBag.ImageHeigh = image.Height;
                    }
                    else
                    {
                        ModelState.AddModelError("", General.ProfileImageToSmall);
                    }

                }
            }

            if (x != null && y != null && h != null && w != null)
            {
                if (CU == null) CU = reposetory.GetPerson(profile.PersonID);
                if (profile.PersonID != CU.PersonID) return RedirectToAction("EditProfile");

                if (CU.PersonID != Guid.Empty)
                {
                    if (!ModifyImage(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(w), Convert.ToInt32(h), CU)) ModelState.AddModelError("", General.ProfileImageCropError);
                }

            }


            return View(profile);
        }

        /// <summary>
        /// Modifies an Profile image.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <param name="PersonID">Profile Image to work on</param>
        /// <returns>New Image Id</returns>
        private bool ModifyImage(int x, int y, int w, int h, Person person)
        {
            string ProfilDirSetting = ConfigurationManager.AppSettings["ProfileImage"];

            if (string.IsNullOrWhiteSpace(ProfilDirSetting)) { throw new ArgumentNullException(); }

            var ProfilBilled = string.Format(ProfilDirSetting, person.PersonID);

            if (System.IO.File.Exists(Server.MapPath(ProfilBilled)))
            {
                Image img = Image.FromFile(Server.MapPath(ProfilBilled));



                using (System.Drawing.Bitmap _bitmap = new System.Drawing.Bitmap(428, 550))
                {
                    _bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                    using (Graphics _graphic = Graphics.FromImage(_bitmap))
                    {
                        _graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        _graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        _graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        _graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        _graphic.Clear(Color.White);
                        //_graphic.DrawImage(img, 0, 0, w, h);
                        _graphic.DrawImage(img, new Rectangle(0, 0, 428, 550), x, y, w, h, GraphicsUnit.Pixel);
                        //_graphic.DrawImage(img, new Rectangle(0, 0, w, h), x, y, w, h, GraphicsUnit.Pixel);
                        //_graphic.DrawImage(img, 0, 0, img.Width, img.Height);
                        //_graphic.DrawImage(img, new Rectangle(0, 0, 428, 550), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);

                    }
                    System.Drawing.Imaging.PropertyItem prop = img.PropertyItems[0];
                    SetProperty(ref prop, 270, string.Format("Username: {0}, {1}", person.UserName, person.FullName));
                    _bitmap.SetPropertyItem(prop);
                    SetProperty(ref prop, 33432, "Copyright Natteravnene www.natteravnene.dk");
                    _bitmap.SetPropertyItem(prop);
                    //TODO: Set more properties
                    img.Dispose();

                    _bitmap.Save(Server.MapPath(ProfilBilled), System.Drawing.Imaging.ImageFormat.Jpeg);
                }

            }

            return true;
        }

        #region Helpers

        private void SetProperty(ref System.Drawing.Imaging.PropertyItem prop, int iId, string sTxt)
        {
            int iLen = sTxt.Length + 1;
            byte[] bTxt = new Byte[iLen];
            for (int i = 0; i < iLen - 1; i++)
                bTxt[i] = (byte)sTxt[i];
            bTxt[iLen - 1] = 0x00;
            prop.Id = iId;
            prop.Type = 2;
            prop.Value = bTxt;
            prop.Len = iLen;
        }

        public enum ImageMetadataPropertyId
        {
            Title = 270,
            Comment = 40092,
            Author = 40093,
            Keywords = 40094,
            Subject = 40095,
            Artist = 315,
            Copyright = 33432,
            Software = 272
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
