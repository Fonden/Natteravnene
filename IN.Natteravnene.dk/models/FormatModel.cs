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
using System.Text.RegularExpressions;
using System.Web;
using DTA;

namespace NR.Models
{
    public static class FormatModel
    {
        /// <summary>
        /// Trim properties in Person object in relation to text formatting af name, phone ect.
        /// </summary>
        /// <param name="person">Person call to be trimed</param>
        /// <returns> Trimed Person</returns>
        public static Person Trim(this Person person)
        {
            person.UserName = person.UserName.NoSpaceToUpper();
            person.FirstName = person.FirstName.NameTrim();
            person.FamilyName = person.FamilyName.NameTrim();
            person.Address = person.Address.TrimAndReduce();
            person.City = person.City.TrimAndReduce();
            person.Zip = person.Zip.TrimAndReduce();
            person.Phone = person.Phone.PhoneTrim();
            person.Mobile = person.Mobile.PhoneTrim();
            person.Email = person.Email.EmailTrim();
            if (person.Country == 0) person.Country = Country.DK;
            return person;
        }

        /// <summary>
        /// Trim properties in Association object in relation to text formatting af name, phone ect.
        /// </summary>
        /// <param name="association">Association to be trimed</param>
        /// <returns>Trimed Association</returns>
        public static Association Trim(this Association association)
        {
            association.Name = association.Name.SentenceTrim();
            association.Address = association.Address.TrimAndReduce();
            association.City = association.City.TrimAndReduce();
            association.Zip = association.Zip.TrimAndReduce();
            association.ContactPhone = association.ContactPhone.PhoneTrim();
            association.TeamPhone = association.TeamPhone.PhoneTrim();
            association.AssociationEmail = association.AssociationEmail.EmailTrim();
            association.URL = association.URL;
            return association;
        }

        /// <summary>
        ///  Trim properties in Location object in relation to text formatting af name, Descriptione ect.
        /// </summary>
        /// <param name="location">Location to be trimed</param>
        /// <returns>Trimed location</returns>
        public static Location Trim(this Location location)
        {
            location.Name = location.Name.SentenceTrim();
            location.ShortName = location.ShortName.NoSpaceToUpper();
            location.Description = location.Description.SentenceTrim();
            return location;
        }

        /// <summary>
        ///  Trim properties in Notification object in relation to text formatting af message ect.
        /// </summary>
        /// <param name="notification">Notification to be trimed</param>
        /// <returns>Trimed notification</returns>
        public static Notification Trim(this Notification notification)
        {
            notification.Message = notification.Message.Substring(0, notification.Message.Length > 150 ? 150 : notification.Message.Length);
            if (notification.Recivers.Any()) notification.Recivers = notification.Recivers.Distinct().ToList();
            return notification;
        }

        /// <summary>
        ///  Trim properties in Eventn object in relation to text formatting af message ect.
        /// </summary>
        /// <param name="TheEvent">Eventn to be trimed</param>
        /// <returns>Trimed event</returns>
        public static Event Trim(this Event TheEvent)
        {
            return TheEvent;
        }

        /// <summary>
        ///  Trim properties in News object in relation to text formatting af message ect.
        /// </summary>
        /// <param name="News">News to be trimed</param>
        /// <returns>Trimed news</returns>
        public static News Trim(this News News)
        {
            News.Headline = News.Headline.Substring(0, News.Headline.Length > 50 ? 50 : News.Headline.Length);
            if (News.Teaser != News.Teaser) News.Teaser = News.Teaser.Substring(0, News.Teaser.Length > 500 ? 500 : News.Teaser.Length);
            return News;
        }

        /// <summary>
        ///  Trim properties in Wiki object in relation to text formatting af message ect.
        /// </summary>
        /// <param name="Wiki">Wiki to be trimed</param>
        /// <returns>Trimed wiki</returns>
        public static Wiki Trim(this Wiki Wiki)
        {
            if (Wiki.Title != null) Wiki.Title = Wiki.Title.Substring(0, Wiki.Title.Length > 50 ? 50 : Wiki.Title.Length);
            return Wiki;
        }

        /// <summary>
        ///  Trim properties in Wiki object in relation to text formatting af message ect.
        /// </summary>
        /// <param name="Wiki">Wiki to be trimed</param>
        /// <returns>Trimed wiki</returns>
        public static Cause Trim(this Cause Cause)
        {
            return Cause;
        }

    }
}