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
using System.Globalization;

namespace NR.Models
{
    [Table("Associations")]
    public class Association : Basic
    {
        public Association()
        {
            Created = DateTime.Now;
            Lastchanged = DateTime.Now;
            ShowAddresse = true;
        }
        
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AssociationID { get; set; }

        [Display(Name = "AssociationStatus", ResourceType = typeof(DomainStrings))]
        public AssociationStatus Status { get; set; }

        [Display(Name = "AssociationNumber", ResourceType = typeof(DomainStrings))]
        public int Number { get; set; }

        [Display(Name = "Network", ResourceType = typeof(DomainStrings))]
        public virtual Guid NetworkID { get; set; }

        [Display(Name = "AssociationName", Prompt = "AssociationNamePrompt", Description = "AssociationNamePrompt", ResourceType = typeof(DomainStrings))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DomainStrings))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name = "AssociationEmail", Prompt = "AssociationEmailPrompt", ResourceType = typeof(DomainStrings))]
        [DTAEmailAddressAttribute(ErrorMessageResourceName = "WrongEmailFormat", ErrorMessageResourceType = typeof(DomainStrings))]
        public string AssociationEmail { get; set; }

        [Display(Name = "Country", ResourceType = typeof(DomainStrings))]
        public Country Country { get; set; }

        [Display(Name = "CVRnr", Prompt = "CVRnrPrompt", ResourceType = typeof(DomainStrings))]
        [CVR(ErrorMessageResourceName = "CVRWrongformat", ErrorMessageResourceType = typeof(DomainStrings))]
        [MaxLength(20)]
        public string CVRNR { get; set; }

        [Display(Name = "Established", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime? Established { get; set; }

        [Display(Name = "LastContactHQ", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(DataFormatString = "{0:dd-mm-yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime? LastContactHQ { get; set; }

        [Display(Name = "GovernanceType", ResourceType = typeof(DomainStrings))]
        public GovernanceType Governance { get; set; }

        [Display(Name = "TextServiceProviderUserName", Prompt = "TextServiceProviderUserNamePrompt", ResourceType = typeof(DomainStrings))]
        [MaxLength(20)]
        public string TextServiceProviderUserName { get; set; }

        /// <summary>
        /// Class for the TextServiceProvider used by this association
        /// </summary>
        [Display(Name = "TextServiceProviderPassword", Prompt = "TextServiceProviderPasswordPrompt", ResourceType = typeof(DomainStrings))]
        [MaxLength(20)]
        public string TextServiceProviderPassword { get; set; }

        public string TextServiceProvider { get; set; }

        [Display(Name = "TeamPhone", Prompt = "TeamPhonePrompt", ResourceType = typeof(DomainStrings))]
        [MaxLength(15)]
        public string TeamPhone { get; set; }

        [Display(Name = "ContactPhone", Prompt = "ContactPhonePrompt", ResourceType = typeof(DomainStrings))]
        [MaxLength(15)]
        public string ContactPhone { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string Address { get; set; }

        [MaxLength(15)]
        [StringLength(15, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "Zip", ResourceType = typeof(DomainStrings))]
        public string Zip { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [Display(Name = "City", ResourceType = typeof(DomainStrings))]
        public string City { get; set; }

        [NotMapped]
        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string FullAddress
        {
            get
            {
                return Address + ", " + Zip + " " + City;
            }
        }

        [Display(Name = "Issue", ResourceType = typeof(DomainStrings))]
        public bool Issue { get; set; }

        [Display(Name = "SendTeamText", ResourceType = typeof(DomainStrings))]
        public bool SendTeamText { get; set; }

        [Display(Name = "SendTeamTextDays", ResourceType = typeof(DomainStrings))]
        public int SendTeamTextDays { get; set; }

        [Display(Name = "TeamMessageTeamLeader", ResourceType = typeof(DomainStrings))]
        [StringLength(440, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 10)]
        [MaxLength(440)]
        [DataType(DataType.MultilineText)]
        public string TeamMessageTeamLeader { get; set; }

        [Display(Name = "TeamMessage", ResourceType = typeof(DomainStrings))]
        [StringLength(440, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 10)]
        [MaxLength(440)]
        [DataType(DataType.MultilineText)]
        public string TeamMessage { get; set; }

        [Display(Name = "SendNoteTeamleader", ResourceType = typeof(DomainStrings))]
        public bool SendNoteTeamleader { get; set; }

        [Display(Name = "NoteTextTime", ResourceType = typeof(DomainStrings))]
        public int NoteTextTime { get; set; }

        [Display(Name = "Scheduletext", ResourceType = typeof(DomainStrings))]
        [StringLength(5000, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings), MinimumLength = 10)]
        [MaxLength(5000)]
        [DataType(DataType.Html)]
        public virtual string Scheduletext { get; set; }

        public Nullable<System.DateTime> SidsteTurmappe { get; set; }

        [Display(Name = "UseSchedulePlanning", ResourceType = typeof(DomainStrings))]
        public virtual bool UseSchedulePlanning { get; set; }

        [Display(Name = "UsePolicePlanning", ResourceType = typeof(DomainStrings))]
        public virtual bool UsePolicePlanning { get; set; }

        [Display(Name = "UseLists", ResourceType = typeof(DomainStrings))]
        public bool UseLists { get; set; }

        [Display(Name = "UseAccounting", ResourceType = typeof(DomainStrings))]
        public bool UseAccounting { get; set; }

        [Display(Name = "UseLocalNewsletter", ResourceType = typeof(DomainStrings))]
        public bool UseLocalNewsletter { get; set; }

        [Display(Name = "UseNationalNewsletter", ResourceType = typeof(DomainStrings))]
        public virtual bool UseNationalNewsletter { get; set; }

        [Display(Name = "UseKeyBox", ResourceType = typeof(DomainStrings))]
        public bool UseKeyBox { get; set; }

        [Display(Name = "KeyBoxcode", ResourceType = typeof(DomainStrings))]
        [StringLength(10, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        [MaxLength(10)]
        public string KeyBoxcode { get; set; }

        [Display(Name = "UseShiftTeam", ResourceType = typeof(DomainStrings))]
        public bool UseShiftTeam { get; set; }

        [Display(Name = "UseTakeTeamSpot", ResourceType = typeof(DomainStrings))]
        public bool UseTakeTeamSpot { get; set; }

        [Display(Name = "DeadlineHoursTeamChange", ResourceType = typeof(DomainStrings))]
        public int DeadlineHoursTeamChange { get; set; }

        [Display(Name = "UseSponsorPage", ResourceType = typeof(DomainStrings))]
        public bool UseSponsorPage { get; set; }

        [Display(Name = "UsePressPage", ResourceType = typeof(DomainStrings))]
        public bool UsePressPage { get; set; }

        [Display(Name = "UseLinksPage", ResourceType = typeof(DomainStrings))]
        public bool UseLinksPage { get; set; }

        [Display(Name = "UseTeamExchange", ResourceType = typeof(DomainStrings))]
        public bool UseTeamExchange { get; set; }

        [Display(Name = "UseHotzones", ResourceType = typeof(DomainStrings))]
        public bool UseHotzones { get; set; }

        [Display(Name = "ShowAddresse", ResourceType = typeof(DomainStrings))]
        public bool ShowAddresse { get; set; }

        [Display(Name = "AssociationSeniorInstructor", ResourceType = typeof(DomainStrings))]
        public Nullable<Guid> SeniorInstructorID { get; set; }

        [Display(Name = "URL", ResourceType = typeof(DomainStrings))]
        public string URL { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(DomainStrings))]
        public string Comments { get; set; }

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

        public virtual ICollection<Location> Locations { get; set; }

        public virtual ICollection<NRMembership> Memberships { get; set; }

        [Display(Name = "AssociationSeniorInstructor", ResourceType = typeof(DomainStrings))]
        [ForeignKey("SeniorInstructorID")]
        public virtual Person SeniorInstruktoer { get; set; }

        [Display(Name = "Network", ResourceType = typeof(DomainStrings))]
        [ForeignKey("NetworkID")]
        public Network Network { get; set; }

        public ICollection<Folder> Folders { get; set; }

        public ICollection<Team> Teams { get; set; }

        public List<List> Lists { get; set; }

        public Content PageAbout { get; set; }

        public Content PagePress { get; set; }

        public Content PageSponsor { get; set; }

        public Content PageLink { get; set; }

        public IList<Sponsor> Sponsors { get; set; }

        public IList<Link> Links { get; set; }

        #endregion
    }
}
