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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NR.Models
{
    public class Message
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageID { get; set; }

        public MessageType Type { get; set; }

        [NotMapped]
        public string HeadTxt
        {
            get
            {
                if (Type == MessageType.shortMessage) return Body; 
                return Head;
            }
        }

        [MaxLength(35)]
        public string Head { get; set; }

        public string Body { get; set; }

        public Guid SenderID { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("SenderID")]
        public Person Sender { get; set; }

        public ICollection<MessageReciver> Recivers
        {
            get;
            set;
        }

        #endregion
    }

    public class MessageReciver
    {

        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageReciverID { get; set; }

        public bool Read { get; set; }

        public bool Sent { get; set; }

        public SMSStatus TextStatus { get; set; }

        public Guid ReciverID { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("ReciverID")]
        public Person Reciver { get; set; }

        public Message Message { get; set; }

        #endregion
    }

}