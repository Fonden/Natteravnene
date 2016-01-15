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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Models
{
    public class MessageListModel
    {
        public MessageListModel()
        {
            NewMessage = new Message();
            NewMessageTo = new List<Guid>();
        }
        
        public List<Message> Messages { get; set; }

        public List<Message> SentMessages { get; set; }

        public int Total { get; set; }
        
        public int UnRead { get; set; }

        public List<Guid> NewMessageTo { get; set; }

        public Boolean Short { get; set; }

        [MaxLength(35)]
        public virtual string Head { get; set; }

        [AllowHtml]  
        public virtual string Body { get; set; }

        public virtual string BodyShort { get; set; }
        
        public Message NewMessage { get; set; }

        public List<SelectListItem> List { get; set; }

        public Guid PreloadID { get; set; }

    }
}