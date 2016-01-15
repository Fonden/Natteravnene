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

namespace NR.Models
{
    public class Notification
    {
        public Notification()
        {
            Created = DateTime.Now;
        }


        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NotificationID { get; set; }

        [MaxLength(150)]
        [Display(Name = "Message", ResourceType = typeof(DomainStrings))]
        public String Message { get; set; }

        public NotificationType Type { get; set; }

        public Guid Link {get; set;}

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; } 

        #endregion

        #region Navigation Properties

        public ICollection<NotificationReciver> Recivers { get; set; }

        #endregion

    }

    public class NotificationReciver
    {

        #region Primitive Properties

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid NotificationReciverID { get; set; }

        public bool Read { get; set; }

        public bool Sent { get; set; }

        public int SentAttempt { get; set; }

        #endregion

        #region Navigation Properties

        public Person Reciver { get; set; }

        public Notification Notification { get; set; }

        #endregion
    }

}