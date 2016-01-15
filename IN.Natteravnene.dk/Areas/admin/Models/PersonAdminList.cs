/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Linq;
using System.Web.Mvc.Html;


namespace NR.Models
{
    public class PersonAdminList
    {
        
        public PersonAdminList(Person person)
        {
            PersonID = person.PersonID;
            UserName = person.UserName;
            Email = person.Email;
            FullName = person.FullName;
            Mobile = person.Mobile;
            Phone = person.Phone;
            if (person.Memberships.Any())
            {
                String StrAssociation = "";
                foreach (NRMembership m in person.Memberships)
                {
                    StrAssociation += m.Association.Name + ", ";
                };
                StrAssociation = StrAssociation.Substring(0, StrAssociation.Length - 2);
                Association = StrAssociation;
            }

            Attributes = HtmlExtensions.PersonOnlyAttributesIcons(person);
        }
        
        
        public virtual System.Guid PersonID
        {
            get;
            set;
        }

        public virtual string UserName
        {
            get;
            set;
        }

        public virtual string FullName
        {
            get;
            set;
        }

        public virtual string Mobile
        {
            get;
            set;
        }

        public virtual string Phone
        {
            get;
            set;
        }

        public virtual string Email
        {
            get;
            set;
        }

        public virtual string Attributes
        {
            get;
            set;
        }

        public String Association
        {
            get;
            set;
        }

    }
}