/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using DTA;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class PersonWithMembership
    {

        //#region Primitive Properties Person 

        public virtual Guid PersonID { get; set; }

        
        [MaxLength(10)]
        public virtual string UserName { get; set; }

        [DTAEmailAddressAttribute(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        public virtual string Email { get; set; }

        //[MaxLength(40)]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        //[StringLength(40, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        //[Display(Name = "FirstName", ResourceType = typeof(DomainStrings))]
        //public virtual string FirstName { get; set; }

        //[MaxLength(50)]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        //[StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        //[Display(Name = "FamilyName", ResourceType = typeof(DomainStrings))]
        //public virtual string FamilyName { get; set; }

        //[NotMapped]
        //[Display(Name = "FullName", ResourceType = typeof(DomainStrings))]
        //public virtual string FullName
        //{
        //    get
        //    {
        //        return FirstName + " " + FamilyName;
        //    }
        //}

        //[Display(Name = "Birthdate", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        //public virtual DateTime? BirthDate { get; set; }

        //[NotMapped]
        //[Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        //public virtual string FullAddress
        //{
        //    get
        //    {
        //        return Address + ", " + Zip + " " + City;
        //    }
        //}

        //[MaxLength(70)]
        //[StringLength(70, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        //[Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        //public virtual string Address { get; set; }

        //[MaxLength(15)]
        //[StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        //[Display(Name = "Zip", ResourceType = typeof(DomainStrings))]
        //public virtual string Zip { get; set; }

        //[MaxLength(50)]
        //[StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        //[Display(Name = "City", ResourceType = typeof(DomainStrings))]
        //public virtual string City { get; set; }

        //[Display(Name = "Country", ResourceType = typeof(DomainStrings))]
        //public virtual Country Country { get; set; }

        //[MaxLength(15)]
        //[StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        //[Display(Name = "Mobile", ResourceType = typeof(DomainStrings))]
        //public virtual string Mobile { get; set; }

        //[MaxLength(15)]
        //[StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        //[Display(Name = "Phone", ResourceType = typeof(DomainStrings))]
        //public virtual string Phone { get; set; }

        //[Display(Name = "Gender", ResourceType = typeof(DomainStrings))]
        //public virtual Gender Gender { get; set; }

        //[MaxLength(1000)]
        //[Display(Name = "ProfileInfo", ResourceType = typeof(DomainStrings))]
        //public virtual string ProfileInfo { get; set; }

        //[Display(Name = "IDWEB", ResourceType = typeof(DomainStrings))]
        //public virtual Nullable<int> IDWEB { get; set; }


        //[Display(Name = "SetupPrintNewslettet", ResourceType = typeof(DomainStrings))]
        //public virtual bool PrintNewslettet { get; set; }

        //[Display(Name = "SetupEmailNewsLetter", ResourceType = typeof(DomainStrings))]
        //public virtual bool EmailNewsLetter { get; set; }

        //[Display(Name = "SetupSeniorInstructor", ResourceType = typeof(DomainStrings))]
        //public virtual bool SeniorInstructor { get; set; }

        //[NotMapped]
        //[Display(Name = "BasicTraining", ResourceType = typeof(DomainStrings))]
        //public virtual bool BasicTraining { get { return BasicTrainingDate != null; } }

        //[Display(Name = "BasicTrainingDate", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        //public virtual Nullable<DateTime> BasicTrainingDate { get; set; }

        //public virtual bool MailUndeliverable { get; set; }

        //public virtual Nullable<DateTime> MailUndeliverableDate { get; set; }

        //[Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public virtual System.DateTime Lastchanged { get; set; }

        //[Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public virtual System.DateTime Created { get; set; }

        //#endregion

        //#region Primitive Properties Memebership

        public virtual Guid FunctionID
        {
            get;
            set;
        }

        //public virtual BoardFunctionType BoardFunction
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "SetupTeamleader", ResourceType = typeof(DomainStrings))]
        //public virtual bool Teamleader
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "SetupPlanner", ResourceType = typeof(DomainStrings))]
        //public virtual bool Planner
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "SetupSecretary", ResourceType = typeof(DomainStrings))]
        //public virtual bool Secretary
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "SetupEditor", ResourceType = typeof(DomainStrings))]
        //public virtual bool Editor
        //{
        //    get;
        //    set;
        //}


        //[Display(Name = "SignupDate", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        //public virtual Nullable<DateTime> SignupDate
        //{
        //    get;
        //    set;
        //}


        //[NotMapped]
        //[Display(Name = "Absent", ResourceType = typeof(DomainStrings))]
        //public virtual bool Absent
        //{
        //    get { return AbsentDate != null; }

        //}

        //[Display(Name = "AbsentDate", ResourceType = typeof(DomainStrings))]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        //public virtual Nullable<DateTime> AbsentDate
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "PersonType", ResourceType = typeof(DomainStrings))]
        //public virtual PersonType Type
        //{
        //    get;
        //    set;
        //}

        //[Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        //public virtual string Note
        //{
        //    get;
        //    set;
        //}


        //public virtual Association Association
        //{
        //    get;
        //    set;
        //}

        //#endregion

        //#region Navigation Properties

        //public virtual IList<Membership> Memberships { get; set; }

        //public virtual IList<CausePartisipant> Causes { get; set; }

        //public virtual IList<MessageReciver> Messages { get; set; }

        //public virtual IList<NotificationReciver> Notifications { get; set; }

        //public virtual IList<FolderUserAnswer> FolderAnswers { get; set; }

        //public virtual IList<List> OnLists { get; set; }

        //public virtual IList<Team> Teams { get; set; }

        //#endregion


    }

    public class PersonWithMembershipConfiguration : EntityTypeConfiguration<PersonWithMembership>
    {

        public PersonWithMembershipConfiguration()
        {
            //Property(d => d.PersonID).HasColumnName("PersonID");
            //Property(d => d.Email).HasColumnName("Email");

            Map(m =>
                {
                    m.Properties(d => new { d.PersonID, d.Email });
                    m.ToTable("People");

                });
            Map(m =>
            {
                m.Properties(d => new { d.FunctionID });
                m.ToTable("Memberships");

            });
        }
    }

}