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
using System.Configuration;
using System.Linq;
using System.Web;

namespace NR.Models
{
    [Table("Files")] 
    public class AFile
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FileID { get; set; }

        [Display(Name = "FileTitle", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "Description", ResourceType = typeof(DomainStrings))]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "FilType", ResourceType = typeof(DomainStrings))]
        [MaxLength(5)]
        public string Type { get; set; }

        public string Tag { get; set; }

        [NotMapped]
        public bool Exists { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }


        #endregion

    }
}