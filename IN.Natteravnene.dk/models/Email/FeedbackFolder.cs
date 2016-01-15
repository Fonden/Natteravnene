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
using Postal;
using System.Net.Mail;

namespace NR.Models
{
    public class FeedbackFolder : Email
    {
        //public FeedbackFolder
        //{
        //ReplyToList = new MailAddressCollection();
        //}
        public string To { get; set; }
        public Folder folder { get; set; }
        public bool FeedbackLink { get; set; }
        public string message { get; set; }
        public Person Person { get; set; }
        public bool Link { get; set; }
        public string ReplyTo { get; set; }
        

        //public MailAddressCollection ReplyToList { get; set; }
    }
}