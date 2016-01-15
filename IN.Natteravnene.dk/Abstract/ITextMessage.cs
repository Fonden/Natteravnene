/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

<WORK'S NAME> is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Abstract
{
    public interface ITextMessage
    {


        Person From { get; set; }

        string FromText { get; set; }

        ICollection<Person> Recipient { get; set; }

        string Message { get; set; }

        string HandShakeUrl { get; set; }

        string TextId { get; set; }

        DateTime DeliveryTime { get; set; }

        Boolean FlashText { get; set; }

        string Error { get; set; }

        Boolean Send();

    }
}