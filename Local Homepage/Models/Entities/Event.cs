using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;


namespace NR.Models
{
    

    [Table("Events")]
    public class Event
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventID { get; set; }

        [Display(Name = "DistributionLevel", ResourceType = typeof(DomainStrings))]
        public LevelType Distribution { get; set; }

        public Guid DistributionLink { get; set; } //Link to either Association or Network dependent on LevelType

        [Display(Name = "SourceLevel", ResourceType = typeof(DomainStrings))]
        public LevelType Source { get; set; }

        public Guid SourceLink { get; set; } //Link to either Association or Network dependent on LevelType

        [Display(Name = "Headline", ResourceType = typeof(DomainStrings))]
        [MaxLength(50)]
        public string Headline { get; set; }

        [Display(Name = "Location", ResourceType = typeof(DomainStrings))]
        [MaxLength(50)]
        public string Location { get; set; }

        [Display(Name = "IgnorDistribution", ResourceType = typeof(DomainStrings))]
        public string IgnorDistribution { get; set; }

        [Display(Name = "Description", ResourceType = typeof(DomainStrings))]
        [StringLength(5000, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 10)]
        [MaxLength(5000)]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        [Display(Name = "Start", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DataType(DataType.Text)]
        public System.DateTime Start { get; set; }

        [Display(Name = "Finish", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DataType(DataType.Text)]
        public System.DateTime Finish { get; set; }

        public Guid AuthorID { get; set; } 

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

        [ForeignKey("AuthorID")]
        public Person Author { get; set; }
        
        public List Signup { get; set; }

        #endregion
    }
}
