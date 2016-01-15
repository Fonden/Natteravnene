using NR.Abstract;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DTA;
using System.IO;

namespace NR.Infrastructure
{
    public class CPSMSGateway : ITextMessage
    {
        private static UriBuilder CpTestUri = new UriBuilder("http://www.cpsms.dk/sms/");

        private string UserName;

        private string Password;

        public CPSMSGateway(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }

        public Person From { get; set; }

        public string FromText { get; set; }

        public ICollection<Person> Recipient { get; set; }
        
        public string Message { get; set; }

        /// <summary>
        /// URI to postback status of the S Text sending, if supported by the provider
        /// </summary>
        public string HandShakeUrl { get; set; }

        public string TextId { get; set; }

        public DateTime DeliveryTime { get; set; }

        public Boolean FlashText { get; set; }

        public string Error { get; set; }

        public Boolean Send()
        {
            if (Recipient == null || !Recipient.Any()) throw new NullReferenceException("Recipient");
            if (From == null && string.IsNullOrWhiteSpace(FromText)) throw new NullReferenceException("From");
            if (string.IsNullOrWhiteSpace(Message)) throw new NullReferenceException("Message");
            if (string.IsNullOrWhiteSpace(UserName)) throw new NullReferenceException("UserName");
            if (string.IsNullOrWhiteSpace(Password)) throw new NullReferenceException("Password");

            Recipient = Recipient.Where(R => R.Mobile != null && !string.IsNullOrWhiteSpace(R.Mobile)).ToList();
            if (!Recipient.Any()) return true;

            string Query = "utf8=1&from=" + (From == null ? HttpUtility.UrlEncode(FromText) : HttpUtility.UrlEncode(PhoneNumberBuild(From)));
            Query += string.Format("&username={0}&password={1}", HttpUtility.UrlEncode(UserName), HttpUtility.UrlEncode(Password));
            Query += "&message=" + HttpUtility.UrlEncode(Message);
            
            if (Recipient.Count() == 1)
            {
                Query += "&recipient=" + HttpUtility.UrlEncode(PhoneNumberBuild(Recipient.First()));
            }
            else
            {
                int i = 1;
            foreach (Person P in Recipient)
            {
                Query += "&recipient[" + i.ToString() + "]=" + HttpUtility.UrlEncode(PhoneNumberBuild(P));

            }
                }

            if (FlashText) Query += "&flash=1";

            if (DeliveryTime != new DateTime())  Query += "&timestamp=" + DeliveryTime.ToString("yyyyMMddHHmm");

            if (!string.IsNullOrWhiteSpace(HandShakeUrl))
            {
                UriBuilder HS = new UriBuilder(HandShakeUrl);
                Query += "&url=" +HS.ToString();
            }


            CpTestUri.Query = Query;



             //WebRequest theReq = WebRequest.Create();
             var request = (HttpWebRequest)WebRequest.Create(CpTestUri.ToString());
             try
             {
                 using (var response = request.GetResponse() as HttpWebResponse)
                 {
                     if (request.HaveResponse && response != null)
                     {
                         using (var reader = new StreamReader(response.GetResponseStream()))
                         {
                             string result = reader.ReadToEnd();
                         }
                     }
                 }
             }
            catch (Exception ex)
            {
                Error = ex.ToString();
                LogFile.Write(ex, CpTestUri.ToString());
                return false;
            }

             LogFile.Write("Text send ˃˃˃ " + CpTestUri.ToString());
            return true;
        }


        private string PhoneNumberBuild(Person Person)
        {
            if (Person.Mobile.StartsWith("+")) return Person.Mobile.PhoneTrim(); //Person.Mobile.Remove(0,1).PhoneTrim();
            if (Person.Country == 0 ) return "+45" + Person.Mobile.PhoneTrim();
            return "+" + ((int)Person.Country).ToString() + Person.Mobile.PhoneTrim();
        }

    }


   

}