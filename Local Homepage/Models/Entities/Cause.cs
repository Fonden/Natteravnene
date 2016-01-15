using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class Cause
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CauseID { get; set; }

        [MaxLength(40)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(40, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "CauseName", ResourceType = typeof(DomainStrings))]
        public string Name { get; set; }

        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "CauseDescription", ResourceType = typeof(DomainStrings))]
        public string Description { get; set; }

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

        public ICollection<CausePartisipant> Partisipants
        {
            get;
            set;
        }


        #endregion
    }
}