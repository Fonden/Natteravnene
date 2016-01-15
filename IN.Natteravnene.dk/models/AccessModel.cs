/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class AccessModel
    {
        public List<PersonAccess> Form { get; set; }
        public List<NR.Models.NRMembership> Users { get; set; }
        public Guid AddPerson { get; set; }


        public List<PersonAccess> SelectPersons { get; set; }

        public Guid AssociationID { get; set; }
        public String AssociationName { get; set; }
    }

    public class PersonAccess
    {

        public virtual Guid FunctionID { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(DomainStrings))]
        public virtual string FullName { get; set; }

        [Display(Name = "SetupPlanner", ResourceType = typeof(DomainStrings))]
        public virtual bool Planner { get; set; }

        [Display(Name = "SetupSecretary", ResourceType = typeof(DomainStrings))]
        public virtual bool Secretary { get; set; }

        [Display(Name = "SetupEditor", ResourceType = typeof(DomainStrings))]
        public virtual bool Editor { get; set; }
    }


}