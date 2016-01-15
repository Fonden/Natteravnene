/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Abstract;
using NR.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IN.Natteravnene.dk.Controllers
{
    public class TestController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public TestController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        } 
        
        
        // GET: Test
        public ActionResult Index()
        {
             Association association = reposetory.GetAssociations().First();

             association.Locations.First().Lat = 55.29006455319217;
             association.Locations.First().Lng = 11.42852783203125;

             return View(association);
        }

        public ActionResult Layout()
        {

            return View();
        }

        public ActionResult PreViewEmail(string name)
        {
            dynamic email = new WelcomeMailEmail();
            // set up the email ...
            email.To = "dta@cfsa.eu";
            email.UserName = "DTA";
            email.Password = "TESDER";
            email.Send();
            return new EmailViewResult(email);

        }

        public ActionResult PreViewEmailFolderFeedback(string name)
        {
            FeedbackFolder email = new FeedbackFolder();
            // set up the email ...
            email.To = "dta@cfsa.eu";
            email.folder = reposetory.GetFolder(new Guid("9b93e5c0-7caf-e411-bf26-c485084553de"));
            email.Person = reposetory.GetPerson(new Guid("983504df-3e8b-4af7-a54d-308090a74a02"));
            email.message = "test besked på at det virker atgsepråer";
            //email.ReplyToList.Add("dta@cfsa.eu");
            email.Send();
            return new EmailViewResult(email);

        }

        public ActionResult PreViewEmailMessagNotification(string name)
        {
            Person P = new Person
            {
                FirstName = "Hej",
                FamilyName = "EfetrHej"
            };
            
            
            MessagNotification email = new MessagNotification();
            // set up the email ...
            email.To = "dta@cfsa.eu";
            email.Subject = "Test emne";
            email.ReplyTo = "dan@Taxbol";
            email.Message = "em længere beskede der måske kan indholde <strong>Html</strong>";
            email.Link = "http://eteelrande";
            email.FromPerson = P;
            //email.folder = reposetory.GetFolder(new Guid("9b93e5c0-7caf-e411-bf26-c485084553de"));
            //email.Person = reposetory.GetPerson(new Guid("983504df-3e8b-4af7-a54d-308090a74a02"));
            //email.message = "test besked på at det virker atgsepråer";
            //email.ReplyToList.Add("dta@cfsa.eu");
            email.Send();
            return new EmailViewResult(email);

        }

        public ActionResult Error()
        {
            throw new ArgumentNullException("TEst");
            return null;

        }

       

    }
}