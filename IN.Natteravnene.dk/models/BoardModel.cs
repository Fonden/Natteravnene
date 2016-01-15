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
    public class BoardModel : IValidatableObject
    {
        public BoardModel()
        {
            BoardMembers = new List<Guid>();
            Alternate = new List<Guid>();
            minMember = Int32.TryParse(ConfigurationManager.AppSettings["BoardMemberMin"], out minMember) ? minMember : 0;
            maxMember = Int32.TryParse(ConfigurationManager.AppSettings["BoardMemberMax"], out maxMember) ? maxMember : 99;
            minAlternate = Int32.TryParse(ConfigurationManager.AppSettings["BoardAlternateMin"], out minAlternate) ? minAlternate : 0;
            maxAlternate = Int32.TryParse(ConfigurationManager.AppSettings["BoardAlternateMax"], out maxAlternate) ? maxAlternate : 99;
        }

        public int minMember;
        public int maxMember;
        public int minAlternate;
        public int maxAlternate;

        Association ass = new Association();

        [Display(Name = "BoardFunctionTypeChairman", ResourceType = typeof(DomainStrings))]
        [Required]
        public Guid Chairmann {get; set;}
       

        [Display(Name = "BoardFunctionTypeAccountant", ResourceType = typeof(DomainStrings))]
        [Required]
        public Guid Accountant { get; set; }

        [Display(Name = "BoardFunctionTypeAuditor", ResourceType = typeof(DomainStrings))]
        [Required]
        public Guid Auditor { get; set; }

        [Display(Name = "BoardFunctionTypeAuditorAlternate", ResourceType = typeof(DomainStrings))]
        public Guid AuditorAlternate { get; set; }

        [Display(Name = "BoardFunctionTypeBoardMember", ResourceType = typeof(DomainStrings))]
        public List<Guid> BoardMembers { get; set; }

        [Display(Name = "BoardFunctionTypeAlternate", ResourceType = typeof(DomainStrings))]
        public List<Guid> Alternate { get; set; }


        [Display(Name = "GovernanceType", ResourceType = typeof(DomainStrings))]
        public GovernanceType Governance
        {
            get;
            set;
        }

        public List<Person> Active { get; set; }

        public List<Person> All { get; set; }

        public Guid AssociationID { get; set; }
        public String AssociationName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var Distinct = new List<Guid>();

            Distinct.AddRange(this.BoardMembers.Where(b => b != Guid.Empty));
            Distinct.AddRange(this.Alternate.Where(b => b != Guid.Empty));
            Distinct.Add(Chairmann);
            Distinct.Add(Accountant);
            if (Auditor != Guid.Empty) Distinct.Add(Auditor);
            if (AuditorAlternate != Guid.Empty) Distinct.Add(AuditorAlternate);

            int BoardMembers = this.BoardMembers == null ? 0 : this.BoardMembers.Where(g => g != Guid.Empty).Count();
            int BoardAlternate = this.Alternate == null ? 0 : this.Alternate.Where(g => g != Guid.Empty).Count();

            if (Chairmann == Guid.Empty) results.Add(new ValidationResult(General.BoardCharimannMissing));
            if (Accountant == Guid.Empty) results.Add(new ValidationResult(General.BoardAccountantMissing));
            //if (Auditor == Guid.Empty) results.Add(new ValidationResult(General.BoardAuditorMissing));
            
            if (BoardMembers < minMember)
            {
                results.Add(new ValidationResult(string.Format(General.BoardMemeberMin, minMember)));
            }
            if (BoardMembers > maxMember)
            {
                results.Add(new ValidationResult(string.Format(General.BoardMemeberMax, maxMember)));
            }
            if (BoardAlternate < minAlternate)
            {
                results.Add(new ValidationResult(string.Format(General.BoardAlternateMin, minAlternate)));
            }
            if (BoardAlternate > maxAlternate)
            {
               results.Add(new ValidationResult(string.Format(General.BoardAlternateMax, maxAlternate)));
            }
            if (ConfigurationManager.AppSettings["BoardEvenOdd"].ToLower() == "odd" & BoardMembers % 2 == 0)
            {
                results.Add(new ValidationResult(General.BoardOdd));
            }
            if (ConfigurationManager.AppSettings["BoardEvenOdd"].ToLower() == "even" & BoardMembers % 2 != 0)
            {
                results.Add(new ValidationResult(General.BoardEven));
            }
            if (Distinct.Count() != Distinct.Distinct().Count())
            {
                results.Add(new ValidationResult(General.BoardSamePerson));
            }


            return results;
        }
        

    }
}