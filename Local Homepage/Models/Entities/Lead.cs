﻿using DTA;
using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class Lead
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LeadID { get; set; }

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

        [DTAEmailAddressAttribute(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        public string Email { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Mobile", ResourceType = typeof(DomainStrings))]
        public string Mobile { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Phone", ResourceType = typeof(DomainStrings))]
        public string Phone { get; set; }

        [Display(Name = "Status", ResourceType = typeof(DomainStrings))]
        public LeadStatus Status { get; set; }

        public Guid? AssociationID { get; set; }

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

        [ForeignKey("AssociationID")]
        public Association Association { get; set; }


        #endregion
    }
}