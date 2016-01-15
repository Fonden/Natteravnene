using NR.Localication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NR.Models
{
    public class TeamUserAnswer
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnswerID { get; set; }

        public Guid FolderUserAnswerID { get; set; }

        //public Guid PersonID { get; set; }

        public Guid TeamID { get; set; }

        public Nullable<bool> OK { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("FolderUserAnswerID")]
        public FolderUserAnswer FolderUserAnswer { get; set; }

        //[ForeignKey("PersonID")]
        //public virtual Person Person { get; set; }

        [ForeignKey("TeamID")]
        public Team Team { get; set; }

        #endregion
    }
}
