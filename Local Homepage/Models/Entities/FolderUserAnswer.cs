using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;

namespace NR.Models
{
    public class FolderUserAnswer
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FolderUserAnswerID { get; set; }

        public Guid FolderID { get; set; }

        public Guid PersonID { get; set; }

        [Display(Name = "MaxTeams", ResourceType = typeof(DomainStrings))]
        public Nullable<int> MaxTeams { get; set; }

        [Display(Name = "Pass", ResourceType = typeof(DomainStrings))]
        public Nullable<bool> Pass { get; set; }

        [Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        public string Note { get; set; }

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

        [ForeignKey("FolderID")]
        public Folder Folder { get; set; }

        [ForeignKey("PersonID")]
        public Person Person  { get; set; }

        public IList<TeamUserAnswer> Answers { get; set; }

        #endregion
    }
}
