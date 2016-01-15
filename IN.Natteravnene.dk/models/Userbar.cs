/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System.Collections.Generic;

namespace NR.Models
{
    public class Userbar
    {
        public Person CurrentUser { get; set; }

        public Association CurrentAssociation { get; set; }

        public List<Message> Messages { get; set; }
        public int UnreadMessages { get; set; }

        public List<Notification> Notifications { get; set; }
        public int UnreadNotifications { get; set; }
        

    }
}