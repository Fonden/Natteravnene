/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DTA
{
  
        public class DateTimeBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                DateTime dateTime;

                if (DateTime.TryParse(value.AttemptedValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Ugyldig dato eller tid");

                return null;

            }
        }
        public class NullableDateTimeBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (value == null | value.AttemptedValue == string.Empty)
                    return null;
                
                DateTime dateTime;

                if (DateTime.TryParse(value.AttemptedValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Ugyldig dato eller tid");

                return null;
            }
        }

}