using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class TextMessage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TextId { get; set; }

        public SMSStatus TextStatus { get; set; }

        public Guid AssociationID { get; set; }

        public Guid Sender { get; set; }

        public Guid Reciver { get; set; }

        public Guid MessageReciver { get; set; }

        public System.DateTime PlanedSending { get; set; }

        public string Message { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }
    }
}