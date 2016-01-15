/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class DisplayProfile
    {

        public DisplayProfile(Person person)
        {
            if (person == null) throw new ArgumentNullException("Person object null"); 
            Person = person;
            Activity = null;
            ActiveSince = null;
            CurrentMembership = Person.Memberships.Where(x => x.Person.CurrentAssociation == x.AssociationID).FirstOrDefault();
            if (person.DeltaActivity != 0 | (person.Teams != null && person.Teams.Any())) Activity = person.Teams.Where(t => t.Status == TeamStatus.OK).Count() + person.DeltaActivity;
            if (person.Memberships != null && person.Memberships.Any()) ActiveSince = (Person.Memberships.Aggregate((curmin, x) => (curmin == null || (x.SignupDate ?? DateTime.MaxValue) < curmin.SignupDate ? x : curmin))).SignupDate;
        }

        public Person Person { get; set; }

        public int? Activity { get; set; }

        public DateTime? ActiveSince { get; set; }

        public NRMembership CurrentMembership { get; set; }

        public DateTime LastLogin { get; set; }

    }
}