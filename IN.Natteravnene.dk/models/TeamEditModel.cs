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
    public class TeamEditModel
    {
        public Team Team { get; set; }

        [Display(Name = "Teamleader", ResourceType = typeof(DomainStrings))]
        public Guid TeamLeader { get; set; }

        [Display(Name = "TeamMember", ResourceType = typeof(DomainStrings))]
        public List<Guid> Teammembers { get; set; }

        public List<SelectListItem> Active { get; set; }

        public DateTime Start { get; set; }

        public int minTeammembers { get; set; }

        public int maxTeammembers { get; set; }

    }
}