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
    public class MessageGroup
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageGroupID { get; set; }

        [Display(Name = "Name", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }


        #endregion

        #region Navigation Properties

        public IList<Person> Person { get; set; }

        #endregion
    }
}