/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;
using System.Configuration;

namespace NR.Models
{
    public class Team
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamID { get; set; }

        [Display(Name = "Start", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        public System.DateTime Start { get; set; }

        [Display(Name = "Duration", ResourceType = typeof(DomainStrings))]
        public Nullable<TimeSpan> Duration { get; set; }

        [Display(Name = "StarTeam", ResourceType = typeof(DomainStrings))]
        public Boolean  Star { get; set; }

        [NotMapped]
        public DateTime Finish {
            get
            {
                if (Duration == null) return Start;
                return Start + (TimeSpan)Duration;
            }
        }

        [Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        public string Note { get; set; }

        [Display(Name = "Status", ResourceType = typeof(DomainStrings))]
        public TeamStatus Status { get; set; }

        [NotMapped]
        public bool OK
        {
            get {
                if (this.Teammembers == null || this.Status == TeamStatus.Cancelled || this.Status == TeamStatus.Droped) return false;
                if (this.Trial) return this.Teammembers.Count() + 1 >= int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
                return this.Teammembers.Count() >= int.Parse(ConfigurationManager.AppSettings["TeamMin"]);
                }
        }

        [Display(Name = "Teamleader", ResourceType = typeof(DomainStrings))]
        public Guid TeamLeaderId { get; set; }

        [NotMapped]
        public Person Teamleader {
            get
            {
                if (Teammembers == null) return null;
                return Teammembers.Where(p => p.PersonID == TeamLeaderId).FirstOrDefault();
            }
        }

        [Display(Name = "Teamlog", ResourceType = typeof(DomainStrings))]
        [StringLength(5000, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [DataType(DataType.MultilineText)]
        [MaxLength(5000)]
        public string Teamlog { get; set; }

        [Display(Name = "Trial", ResourceType = typeof(DomainStrings))]
        public bool Trial { get; set; }

        [Display(Name = "TrialInfo", ResourceType = typeof(DomainStrings))]
        [StringLength(1000, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [DataType(DataType.MultilineText)]
        [MaxLength(1000)]
        public string TrialInfo { get; set; }

        [Display(Name = "ReminderTextSent", ResourceType = typeof(DomainStrings))]
        public bool ReminderTextSent { get; set; }

        [Display(Name = "NoteTextSent", ResourceType = typeof(DomainStrings))]
        public bool NoteTextSent { get; set; }

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

        public Association Association { get; set; }

        public Folder Folder { get; set; }

        [Display(Name = "TeamMember", ResourceType = typeof(DomainStrings))]
        public IList<Person> Teammembers { get; set; }

        public Location Location { get; set; }

        #endregion


        #region Functions
        

        

         #endregion
    }
}
