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
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;

namespace NR.Models
{
    public class Network
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NetworkID { get; set; }

        [Display(Name = "NetworkNumber", ResourceType = typeof(DomainStrings))]
        [Required]
        public int NetworkNumber { get; set; }

        [Display(Name = "NetworkName", ResourceType = typeof(DomainStrings))]
        [Required]
        public string NetworkName { get; set; }

        [Display(Name = "NetworkNotToShow", ResourceType = typeof(DomainStrings))]
        [Required]
        public bool NetworkNotToShow { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        #endregion

        #region Navigation Properties

        public ICollection<Association> Associations { get; set; }

        #endregion
    }
}
