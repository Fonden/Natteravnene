using NR.Abstract;
using NR.Localication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NR.Infrastructure
{
    /// <summary>
    /// TextServiceProvider select the Serviceprovider specified for sending Textmessages
    /// </summary>
    public static class TextServiceProviderFactory
    {
        public static ITextMessage GetTextServiceProviderrInstance(string Provider,string UserName, string Password)
        {
            Type  T = null;
            if (Provider != null) T = Type.GetType(Provider);
            if (T == null) T = Type.GetType("NR.Infrastructure.CPSMSGateway");

            string UN = string.IsNullOrWhiteSpace(UserName) ?  DefaultForening.TextServiceProviderUserName : UserName;
            string PW = string.IsNullOrWhiteSpace(Password) ?  DefaultForening.TextServiceProviderPassword : Password; 

            ITextMessage TextServiceProvider = (ITextMessage)Activator.CreateInstance(T, new object[] { UN, PW });
            return TextServiceProvider;

        }

    }
}