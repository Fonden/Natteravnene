/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using System;
using System.ComponentModel.DataAnnotations;

namespace NR.Models
{
    public class PersonProfile
    {
        public virtual Guid PersonID
        {
            get;
            set;
        }

        [Display(Name = "Username", ResourceType = typeof(DomainStrings))]
        [StringLength(10, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 3)]
        public virtual string Username
        {
            get;
            set;
        }

        //[EmailAddress(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        public virtual string Email
        {
            get;
            set;
        }

        [MaxLength(40)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(40, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "FirstName", ResourceType = typeof(DomainStrings))]
        public virtual string FirstName
        {
            get;
            set;
        }

        [MaxLength(50)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "FamilyName", ResourceType = typeof(DomainStrings))]
        public virtual string FamilyName
        {
            get;
            set;
        }

        [Display(Name = "Birthdate", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTime? BirthDate
        {
            get;
            set;
        }

        [MaxLength(50)]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "Addresse", ResourceType = typeof(DomainStrings))]
        public virtual string Address
        {
            get;
            set;
        }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "Zip", ResourceType = typeof(DomainStrings))]
        public virtual string Zip
        {
            get;
            set;
        }

        [MaxLength(50)]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [Display(Name = "City", ResourceType = typeof(DomainStrings))]
        public virtual string City
        {
            get;
            set;
        }

        [Display(Name = "Country", ResourceType = typeof(DomainStrings))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(2, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [MaxLength(2)]
        public virtual Country Country
        {
            get;
            set;
        }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 8)]
        [Display(Name = "Mobil", ResourceType = typeof(DomainStrings))]
        public virtual string Mobil
        {
            get;
            set;
        }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 8)]
        [Display(Name = "Phone", ResourceType = typeof(DomainStrings))]
        public virtual string Phone
        {
            get;
            set;
        }

        [Display(Name = "Gender", ResourceType = typeof(DomainStrings))]
        public virtual Gender Gender
        {
            get;
            set;
        }

        public int ProfileState
        {
            get
            {
                double tmpNum = 0;
                if (Username != null) tmpNum += 1;
                if (FirstName != null) tmpNum += 1;
                if (FamilyName != null) tmpNum += 1;
                if (Mobil != null | Phone != null | Email != null) tmpNum += 3;
                if (Address != null & Zip != null & City != null) tmpNum += 3;
                if (Gender != Gender.NotDefined) tmpNum += 1;

                return Convert.ToInt32(tmpNum / 10 * 100);
            }
        }

    }
}