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

namespace NR.Models
{
    public class FolderPlanningModel
    {
        
        
        public Folder Folder { get; set; }

        public List<NR.Models.NRMembership> Active { get; set; }

        public List<FolderUserAnswer> FeedBack { get; set; }

        public int minTeammembers { get; set; }

        public int maxTeammembers { get; set; }

        public bool AutoRemove { get; set; }


    }
}