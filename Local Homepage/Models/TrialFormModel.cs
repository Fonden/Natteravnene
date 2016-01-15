using DTA;
using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local_Homepage.Models
{
    public class TrialFormModel
    {

        [Required(ErrorMessage = "Udfyld venligst Fornavn")]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Udfyld venligst Efternavn")]
        [Display(Name = "Efternavn")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Udfyld venligst Telefon")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [DTAEmailAddressAttribute(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Postnummer")]
        public string Zip { get; set; }

        public Guid AssociationID { get; set; } 

    }
}