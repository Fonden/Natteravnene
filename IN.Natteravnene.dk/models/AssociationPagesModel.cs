/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class AssociationPagesModel
    {

        public AssociationPagesModel() { }

        public AssociationPagesModel( Association association)
        {
            PageAbout = association.PageAbout;
            PagePress = association.PagePress;
            PageLink = association.PageLink;
            PageSponsor = association.PageSponsor;
            Sponsors = association.Sponsors;
        }
        
        
        public Content PageAbout { get; set; }
        public Content PagePress { get; set; }
        public Content PageSponsor { get; set; }
        public Content PageLink { get; set; }

        public IList<Sponsor> Sponsors { get; set; }
    }
}