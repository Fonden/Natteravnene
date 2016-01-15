/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class BoardModelView 
    {
       
        [Display(Name = "BoardFunctionTypeChairman", ResourceType = typeof(DomainStrings))]
        public Person Chairmann {get; set;}
       
        [Display(Name = "BoardFunctionTypeAccountant", ResourceType = typeof(DomainStrings))]
        public Person Accountant { get; set; }

        [Display(Name = "BoardFunctionTypeAuditor", ResourceType = typeof(DomainStrings))]
        public Person Auditor { get; set; }

        [Display(Name = "BoardFunctionTypeAuditorAlternate", ResourceType = typeof(DomainStrings))]
        public Person AuditorAlternate { get; set; }

        [Display(Name = "BoardFunctionTypeBoardMember", ResourceType = typeof(DomainStrings))]
        public List<Person> BoardMembers { get; set; }

        [Display(Name = "BoardFunctionTypeAlternate", ResourceType = typeof(DomainStrings))]
        public List<Person> Alternate { get; set; }

        public Association Association { get; set; }
        
        
    }
}