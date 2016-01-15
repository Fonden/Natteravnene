/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using DTA;
using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NR.Models
{
    public class Person
    {
        public Person()
        {
            EmailNewsLetter = true;
            PrintNewslettet = true;
            /*User = new User();
            Forening = new Forening();
            SvarTurmapper = new List<TurMappeBrugerSvar>();
            PaaLister = new List<Liste>();*/
            //PersonID = Guid.NewGuid();
        }


        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonID { get; set;}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MaxLength(10)]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        public string Password { get; set; }

        [DTAEmailAddressAttribute(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        public string Email { get; set; }

        [MaxLength(40)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(40, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "FirstName", ResourceType = typeof(DomainStrings))]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "FamilyName", ResourceType = typeof(DomainStrings))]
        public string FamilyName { get; set; }

        [NotMapped]
        [Display(Name = "FullName", ResourceType = typeof(DomainStrings))]
        public string FullName
        {
            get
            {
                return FirstName + " " + FamilyName;
            }
        }

        [Display(Name = "Birthdate", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [NotMapped]
        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string FullAddress
        {
            get
            {
                return Address + ", " + Zip + " " + City;
            }
        }

        [MaxLength(70)]
        [StringLength(70, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string Address { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "Zip", ResourceType = typeof(DomainStrings))]
        public string Zip { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "City", ResourceType = typeof(DomainStrings))]
        public string City { get; set; }

        [Display(Name = "Country", ResourceType = typeof(DomainStrings))]
        public Country Country { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Mobile", ResourceType = typeof(DomainStrings))]
        public string Mobile { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Phone", ResourceType = typeof(DomainStrings))]
        public string Phone { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(DomainStrings))]
        public Gender Gender { get; set; }

        [MaxLength(1000)]
        [Display(Name = "ProfileInfo", ResourceType = typeof(DomainStrings))]
        public string ProfileInfo { get; set; }

        [Display(Name = "IDWEB", ResourceType = typeof(DomainStrings))]
        public Nullable<int> IDWEB { get; set; }

        
        [Display(Name = "SetupPrintNewslettet", ResourceType = typeof(DomainStrings))]
        public bool PrintNewslettet { get; set; }

        [Display(Name = "SetupEmailNewsLetter", ResourceType = typeof(DomainStrings))]
        public bool EmailNewsLetter { get; set; }

        [Display(Name = "SetupSeniorInstructor", ResourceType = typeof(DomainStrings))]
        public bool SeniorInstructor { get; set; }

        [NotMapped]
        [Display(Name = "BasicTraining", ResourceType = typeof(DomainStrings))]
        public bool BasicTraining { get { return BasicTrainingDate != null; } }

        [Display(Name = "BasicTrainingDate", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public virtual Nullable<DateTime> BasicTrainingDate { get; set; }

        [Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        public string Note { get; set; }

        public bool MailUndeliverable { get; set; }

        public Nullable<DateTime> MailUndeliverableDate { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        #endregion


        #region Delta Properties

        public int DeltaActivity { get; set; }

        #endregion

        #region UI Properties

        [Display(Name = "ListLines", ResourceType = typeof(DomainStrings))]
        public int ListLines { get; set; }

        [Display(Name = "CurrentAssociation", ResourceType = typeof(DomainStrings))]
        public Guid CurrentAssociation { get; set; }

        #endregion

        #region Navigation Properties

        public IList<NRMembership> Memberships { get; set; }

        public IList<CausePartisipant> Causes { get; set; }

        public IList<MessageReciver> Messages { get; set; }

        public IList<NotificationReciver> Notifications { get; set; }

        /*public virtual ICollection<Hold> Holdene
        {
            get;
            set;
        }*/

        public IList<FolderUserAnswer> FolderAnswers { get; set; }

        public IList<List> OnLists { get; set; }

        public IList<Team> Teams { get; set; }

        [InverseProperty("SeniorInstruktoer")]
        public IList<Association> SiAssociation { get; set; }

        #endregion

       
    }


    
}
