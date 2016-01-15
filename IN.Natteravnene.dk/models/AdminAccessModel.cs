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
using System.Web;
using System.Web.Mvc;

namespace NR.Models
{
    public class AdminAccessModel
    {

        public SelectList DropRoles {get; set;}

        public string Role { get; set; }
        public string UserName { get; set; }

        public List<string> UserInAdminRole { get; set; }
        public List<string> UserInManagementRole { get; set;}
    }
}