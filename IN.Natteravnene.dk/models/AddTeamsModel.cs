/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Models
{
    public class AddTeamsModel
    {
        public Folder Folder { get; set; }

        public Guid Location { get; set; }

        [Display(Name = "Start", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime StartDate { get; set; }

        [Display(Name = "Start", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm}")]
        public System.DateTime StartTime { get; set; }

        [Display(Name = "Duration", ResourceType = typeof(DomainStrings))]
        [DataType(DataType.Duration)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm}")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Note", ResourceType = typeof(DomainStrings))]
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DomainStrings))]
        public string Note { get; set; }

        public RepetitionType Repetition { get; set; }

        public List<SelectListItem> LocationsSelectList { get; set; }
    }


    public enum RepetitionType
    {
        [Display(Name = "RepetitionTypeNone", ResourceType = typeof(DomainStrings))]
        None = 0,

        [Display(Name = "RepetitionTypeDaily", ResourceType = typeof(DomainStrings))]
        Daily = 1,

        [Display(Name = "RepetitionTypeSecondday", ResourceType = typeof(DomainStrings))]
        Secondday = 2,

        [Display(Name = "RepetitionTypeWeekly", ResourceType = typeof(DomainStrings))]
        Weekly = 7,

        [Display(Name = "RepetitionTypeBiWeekly", ResourceType = typeof(DomainStrings))]
        BiWeekly = 14,

        [Display(Name = "RepetitionTypeMonthly", ResourceType = typeof(DomainStrings))]
        Monthly = 30
    }
}