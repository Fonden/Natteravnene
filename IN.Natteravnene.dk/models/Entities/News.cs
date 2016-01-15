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
using System.ComponentModel.DataAnnotations.Schema;

namespace NR.Models
{
    public class News
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NewsId { get; set; }

        [Display(Name = "DistributionLevel", ResourceType = typeof(DomainStrings))]
        public LevelType Distribution { get; set; }

        public Guid DistributionLink { get; set; } //Link to either Association or Network dependent on LevelType

        [Display(Name = "SourceLevel", ResourceType = typeof(DomainStrings))]
        public LevelType Source { get; set; }

        public Guid SourceLink { get; set; } //Link to either Association or Network dependent on LevelType

        [Display(Name = "Internal", ResourceType = typeof(DomainStrings))]
        public bool Internal { get; set; }

        [Display(Name = "Publish", ResourceType = typeof(DomainStrings))]
        public DateTime Publish { get; set; }

        [Display(Name = "Depublish", ResourceType = typeof(DomainStrings))]
        public DateTime? Depublish { get; set; }

        [Display(Name = "Headline", ResourceType = typeof(DomainStrings))]
        [MaxLength(50)]
        public string Headline {get; set;}

        [Display(Name = "Teaser", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.MultilineText)]
        [MaxLength(500)]
        public string Teaser { get; set; }

        [Display(Name = "Content", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Html)]
        public string Content {get; set;}

        public Guid AuthorID { get; set; }

        [Display(Name = "Views", ResourceType = typeof(DomainStrings))]
        public int Views { get; set; }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }


        #region Navigation Properties

        [ForeignKey("AuthorID")]
        public Person Author { get; set; }

        #endregion

        
    }
}