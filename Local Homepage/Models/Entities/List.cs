﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;

namespace NR.Models
{
    [Table("Lists")]
    public class List
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ListID { get; set; }

        public Guid AssociationID { get; set; }

        public string ListName { get; set; }

        public string Description { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual System.DateTime Created { get; set; }

        #endregion
        #region Navigation Properties

        [ForeignKey("AssociationID")]
        public Association Association { get; set; }

        public ICollection<Person> People { get; set; }

        #endregion
    }
}
