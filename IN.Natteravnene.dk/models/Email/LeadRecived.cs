/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class LeadRecived : Email
    {

        public string to { get; set; }

        public List<string> cc { get; set; }

        public Lead lead { get; set; } 

    }
}