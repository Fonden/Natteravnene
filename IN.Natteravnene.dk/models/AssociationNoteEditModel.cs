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

namespace NR.Models
{
    public class AssociationNoteEditModel
    {

        public Guid AssociationID { get; set; }

        [Display(Name = "NewNote", ResourceType = typeof(DomainStrings))]
        public string NewNote { get; set; }

        public string Note { get; set; }
    }
}