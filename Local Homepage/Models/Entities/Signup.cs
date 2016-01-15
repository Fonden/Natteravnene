using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class SignupList
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SignupID { get; set; }

        [Display(Name = "Title", ResourceType = typeof(DomainStrings))]
        [MaxLength(50)]
        public string Title { get; set; }

        public NotificationType Type { get; set; }

        public Guid Link { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        public Guid AssociationID { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("AssociationID")]
        public virtual Association Association { get; set; }

        public virtual ICollection<Signup> Signups
        {
            get;
            set;
        }

        #endregion
    }

    public class Signup
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SignupID { get; set; }

        public Guid PaymentID { get; set; } 
 
        public Guid PersonID { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        public SignupList SignupList { get; set; }

        #endregion
    }

}