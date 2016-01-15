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
    public class Wiki
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid WikiID { get; set; }

        [Display(Name = "WikiTitle", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(50)]
        public virtual string Title { get; set; }

        [Display(Name = "WikiBody", ResourceType = typeof(DomainStrings))]
        [Required]
        public virtual string Body { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual System.DateTime Created { get; set; }

        #endregion

        #region Navigation Properties

        public virtual IList<WikiWord> Words { get; set; }

        public virtual IList<AFile> Files { get; set; }

        #endregion
    }

    public class WikiWord
    {

        #region Primitive Properties

        [Key]
        public int WikiWordID { get; set; }

        [Display(Name = "WikiWord", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(25)]
        public string Word { get; set; }


        #endregion

        #region Navigation Properties

        public Wiki Wiki { get; set; }

        #endregion
    }
}