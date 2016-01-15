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
    [Table("Folders")]
    public class Folder
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FolderID { get; set; }

        [Display(Name = "Foldername", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(50)]
        public string FoldereName { get; set; }

        [Required]
        [Display(Name = "Start", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Start { get; set; }

        [Required]
        [Display(Name = "Finish", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Finish { get; set; }

        [Display(Name = "Status", ResourceType = typeof(DomainStrings))]
        public virtual FolderStatus Status { get; set; }

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

        public Association Association
        {
            get;
            set;
        }

        public IList<Team> Teams
        {
            get;
            set;
        }

        public IList<FolderUserAnswer> UserAnswer
        {
            get;
            set;
        }

        #endregion
    }
}
