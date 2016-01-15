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

namespace NR.Models
{
    public class PersonSettings
    {
        public virtual Guid PersonID
        {
            get;
            set;
        }

        [Display(Name = "ListLines", ResourceType = typeof(DomainStrings))]
        public virtual int ListLines
        {
            get;
            set;
        }

        [Display(Name = "CurrentAssociation", ResourceType = typeof(DomainStrings))]
        public virtual Guid CurrentAssociation
        {
            get;
            set;
        }

        [Display(Name = "SetupPrintNewslettet", ResourceType = typeof(DomainStrings))]
        public virtual bool PrintNewslettet
        {
            get;
            set;
        }

        [Display(Name = "SetupEmailNewsLetter", ResourceType = typeof(DomainStrings))]
        public virtual bool EmailNewsLetter
        {
            get;
            set;
        }

        public List<Association> Associations
        {
            get;
            set;
        }

    }
}