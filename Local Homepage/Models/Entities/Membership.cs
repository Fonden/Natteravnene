using NR.Localication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NR.Models
{
    [Table("Membership")]
    public class NRMembership
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MembershipID { get; set; }

        public Guid AssociationID { get; set; }

        public Guid PersonID { get; set; }

        public BoardFunctionType BoardFunction { get; set; }

        [Display(Name = "SetupTeamleader", ResourceType = typeof(DomainStrings))]
        public bool Teamleader { get; set; }

        [Display(Name = "SetupPlanner", ResourceType = typeof(DomainStrings))]
        public bool Planner { get; set; }

        [Display(Name = "SetupSecretary", ResourceType = typeof(DomainStrings))]
        public bool Secretary { get; set; }

        [Display(Name = "SetupEditor", ResourceType = typeof(DomainStrings))]
        public bool Editor { get; set; }

        [Display(Name = "SignupDate", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<DateTime> SignupDate { get; set; }


        [NotMapped]
        [Display(Name = "Absent", ResourceType = typeof(DomainStrings))]
        public bool Absent
        {
            get { return AbsentDate != null; }

        }

        [Display(Name = "AbsentDate", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<DateTime> AbsentDate { get; set; }

        [Display(Name = "PersonType", ResourceType = typeof(DomainStrings))]
        public PersonType Type { get; set; }

        [Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        public string Note { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("PersonID")]
        public Person Person
        {
            get;
            set;
        }

        [ForeignKey("AssociationID")]
        public Association Association
        {
            get;
            set;
        }

        #endregion

    }
}