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
    public class FrontModel
    {
        public FrontModel() 
        {
            this.News = new List<News>();
            this.Events = new List<Event>();
            this.MyTeams = new List<Team>();
        }

        public List<News> News { get; set; }

        public List<Event> Events { get; set; }

        public List<Team> MyTeams { get; set; }

    }
}