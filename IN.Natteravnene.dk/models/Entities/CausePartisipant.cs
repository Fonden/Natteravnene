/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class CausePartisipant
    {
        #region Primitive Properties
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PartisipantID { get; set; }

        [Display(Name = "Causedate", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public System.DateTime Date { get; set; }
        #endregion

        #region Navigation Properties

        public Cause Cause { get; set; }

        public Person person { get; set; }

        #endregion

    }
}