/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

<WORK'S NAME> is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DTA
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DTAEmailAddressAttribute : RegularExpressionAttribute
    {
        private const string pattern = @"^\S+@\S+\.\S+$";

        static DTAEmailAddressAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(DTAEmailAddressAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public DTAEmailAddressAttribute()
            : base(pattern)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CVRAttribute : RegularExpressionAttribute
    {
        private const string pattern = @"^[0-9]{8}$";

        static CVRAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(CVRAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public CVRAttribute()
            : base(pattern)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class URLAttribute : RegularExpressionAttribute
    {
        private const string pattern = @"^(HTTP|HTTPS|http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$";

        static URLAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(URLAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public URLAttribute()
            : base(pattern)
        {
        }
    }

}