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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class Sponsor
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SponsorID { get; set; }

        [Display(Name = "Name", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "Body", ResourceType = typeof(DomainStrings))]
        public string Body { get; set; }

        [Display(Name = "URL", ResourceType = typeof(DomainStrings))]
        public string URL { get; set; }

        [Display(Name = "Finish", ResourceType = typeof(DomainStrings))]
        public Nullable<DateTime> Finish { get; set; }

        [Display(Name = "Sequence", ResourceType = typeof(DomainStrings))]
        [Range(0, 100,  ErrorMessageResourceName="SequenceRange",ErrorMessageResourceType=typeof(DomainStrings))]
        public int Sequence { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        public Guid AssociationID { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("AssociationID")]
        public Association Association { get; set; }

        #endregion


    }
}