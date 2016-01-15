/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using Local_Homepage.Models;
using NR.Entity;
using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using DTA;
using System.Globalization;
using NR.Localication;
using System.IO;

namespace Local_Homepage.Controllers
{
    public class BaseController : Controller
    {

        public AssociationViewModel Basedata = new AssociationViewModel();

        public void Initiate()
        {
            using (var db = new NRDbContext())
            {
                IdnMapping idn = new IdnMapping();
                Guid associationId = (Guid)this.RouteData.Values["associationId"];

                Association association = db.Associations.Include(l => l.PageAbout).Include(l => l.PageLink).Include(l => l.PagePress).Include(l => l.PageSponsor).Include(l => l.Sponsors).Where(a => a.AssociationID == associationId).FirstOrDefault(); //Find(associationId);
                //db.Entry(association).Collection("PageAbout").Load();
                if (association == null) throw new NullReferenceException("Association");

                Basedata.AssociationID = association.AssociationID;

                GetBoard();

                Basedata.SEO = this.RouteData.Values["SEO"] == null ? false : (bool)this.RouteData.Values["SEO"];
               
                Basedata.NetworkID = association.NetworkID;
                Basedata.AssociationName = string.Format(General.BrandTitleAssociation,association.Name);
                Basedata.Host = string.Format("http://{0}.natteravnene.dk", association.Name.ValidDKDomainName());
                Basedata.MainUrl = Request.Url.Host == idn.GetAscii(string.Format("{0}.natteravnene.dk", association.Name.ValidDKDomainName()));
                Basedata.urlCanonical = urlCanonical(association.Name);
                Basedata.Established = (DateTime)association.Established;
                
                Basedata.CVRNR = association.CVRNR;
                if (Basedata.Chairmann != null)
                {
                    Basedata.AssociationEmail = string.IsNullOrWhiteSpace(association.AssociationEmail) ? Basedata.Chairmann.Email : association.AssociationEmail;
                    Basedata.ContactPhone = string.IsNullOrWhiteSpace(association.ContactPhone) ? Basedata.Chairmann.Mobile : association.ContactPhone;
                }
                else
                {
                    Basedata.AssociationEmail = association.AssociationEmail;
                    Basedata.ContactPhone = association.ContactPhone;
                }

               


                
                Basedata.PageAbout = tagReplace(association.PageAbout);
                Basedata.UseLinksPage = association.UseLinksPage;
                Basedata.PageLink = tagReplace(association.PageLink);
                Basedata.UsePressPage = association.UsePressPage;
                Basedata.PagePress = tagReplace(association.PagePress);
                Basedata.PageSponsor = tagReplace(association.PageSponsor);
                Basedata.Sponsors = association.Sponsors.Where(S => S.Finish == null || (DateTime)S.Finish > DateTime.Now).OrderBy(S => S.Sequence).ToList();
                Basedata.UseSponsorPage = association.UseSponsorPage & Basedata.Sponsors.Any();

               

                string LogoDirSetting = ConfigurationManager.AppSettings["Logos"];

                if (string.IsNullOrWhiteSpace(LogoDirSetting)) { throw new ArgumentNullException(); }

                ViewBag.LocalLogoPath = Path.Combine(LogoDirSetting, Basedata.AssociationID + ".jpg");

                if (!System.IO.File.Exists(Server.MapPath(ViewBag.LocalLogoPath)))
                {
                    ViewBag.LocalLogoPath = Path.Combine(LogoDirSetting, "Natteravnene_logo.jpg"); ;
                }

                ViewBag.UseSponsorerPage = Basedata.UseSponsorPage;
                ViewBag.UseLinksPage = Basedata.UseLinksPage;
                ViewBag.UsePressPage = Basedata.UsePressPage;
                ViewBag.Canonical = Basedata.urlCanonical;
                ViewBag.Association = Basedata.AssociationName;
                ViewBag.Host = Basedata.Host;
                ViewBag.AssociationContactPhone = Basedata.ContactPhone;
                ViewBag.AssociationContactEmail = Basedata.AssociationEmail;
                ViewBag.CVR = association.CVRNR;
                ViewBag.AssociationZIP = association.Zip;
                ViewBag.AssociationCity = association.City;
                ViewBag.AssociationCountry = (int)association.Country == 0 ? null : association.Country.DisplayName();
                ViewBag.Founded = association.Established == null ? null : ((DateTime)association.Established).ToString("yyyy-MM-dd");
            }

        }

        public void News(int? id)
        {
            using (var db = new NRDbContext())
            {

                Basedata.News = db.News
                    .Where(e => (!e.Internal  & (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == Basedata.NetworkID) |
                    (e.Distribution == LevelType.Local & e.DistributionLink == Basedata.AssociationID))) & (e.Depublish == null | e.Depublish > DateTime.Now) & (e.Publish < DateTime.Now))
                    .OrderByDescending(e => e.Publish)
                    .Skip(10 * id ?? 0).Take(10)
                    .ToList();
            }
        }
        public void Events(int? id)
        {
            using (var db = new NRDbContext())
            {

                Basedata.Events = db.Events
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == Basedata.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == Basedata.AssociationID)) & e.Finish > DateTime.Now).OrderBy(e => e.Start)
                .ToList();
            }
        }


        public void AllNewsActivity()
        {
            using (var db = new NRDbContext())
            {

                Basedata.News = db.News
                    .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == Basedata.NetworkID) |
                    (e.Distribution == LevelType.Local & e.DistributionLink == Basedata.AssociationID)) & (e.Depublish == null | e.Depublish > DateTime.Now) & (e.Publish < DateTime.Now))
                    .OrderByDescending(e => e.Publish)
                    .ToList();


                Basedata.Events = db.Events
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == Basedata.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == Basedata.AssociationID)) & e.Finish > DateTime.Now).OrderBy(e => e.Start)
                .ToList();
            }
        }

        public void GetEvent(Guid ID)
        {
            using (var db = new NRDbContext())
            {
                Basedata.Event =  db.Events.Find(ID);
            }
        }

        public void GetNews(Guid ID)
        {
            using (var db = new NRDbContext())
            {
                Basedata.theNews = db.News.Find(ID);
            }
        }

        public void GetBoard()
        {
            using (var db = new NRDbContext())
            {
                Basedata.Chairmann = db.Memberships
                .Where(M => M.AssociationID == Basedata.AssociationID & M.BoardFunction == BoardFunctionType.Chairman)
                .Select(M => M.Person).FirstOrDefault();

                Basedata.Accountant = db.Memberships
                .Where(M => M.AssociationID == Basedata.AssociationID & M.BoardFunction == BoardFunctionType.Accountant)
                .Select(M => M.Person).FirstOrDefault();

                Basedata.BoardMembers = db.Memberships
                .Where(M => M.AssociationID == Basedata.AssociationID & M.BoardFunction == BoardFunctionType.BoardMember)
                .Select(M => M.Person)
                .OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
            }

        }


        public string urlCanonical(string Value)
        {
            if (string.IsNullOrWhiteSpace(Value)) throw new ArgumentNullException("Association Name");
            return string.Format("http://{0}.natteravnene.dk{1}", Value.ValidDKDomainName(), Request.Url.AbsolutePath);
        }

        public Content tagReplace(Content Value)
        {

            if (Value == null) return new Content();
            Value.Body = Value.Body.Replace("##ForeningsNavn##", Basedata.AssociationName);


             return Value;
        }


    }  
}