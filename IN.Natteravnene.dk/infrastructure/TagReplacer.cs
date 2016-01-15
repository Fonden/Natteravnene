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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NR.Infrastructure
{
    public static class TagReplacer
    {

        private static string Firstname = "##Fornavn##";
        private static string LastName = "##Efternavn##";
        private static string Email = "##Email##";
        private static string Mobile = "##Mobil##";
        private static string TeamStart = "##Turstart##";
        private static string TeamMembers = "##Ravne##";


        

        public static string ReplaceTagTeamStart(this string value, Team team)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            string StartStr = team.Start.AddSeconds(-1).ToString("dddd, d. MMM") + team.Start.ToString(" H:mm").Replace("00:00", "24:00");   // Convert time 00:00 to 24:00 for better understanding
            string result = value.Replace(TeamStart, StartStr);

            return result;
        }

        public static string ReplaceTagTeamMembers(this string value, Team team, Person currentPerson)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            string result = "";

            foreach (Person P in team.Teammembers.ToList().Where(x => x.PersonID != currentPerson.PersonID).OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)))
            {
                if (result != "") result += ", ";
                result += P.FirstName + " ( ";
                result += P.Mobile == "" ? "--" : P.Mobile;
                result += " )";
            }

            
           

            return value.Replace(TeamMembers, result);
        }

        public static string ReplaceTagPerson(this string value, Person currentPerson)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return value.Replace(Firstname, currentPerson.FirstName).Replace(LastName, currentPerson.FamilyName).Replace(Email, currentPerson.Email).Replace(Mobile, currentPerson.Mobile);
        }

        public static string ReplaceTagContent(this string value)
        {
            string result = value;

            Regex rgx = new Regex(@"##(URLLOKAL|Snow|Wind|Standard)\b[^##]*##", RegexOptions.IgnoreCase);   
            MatchCollection matches = rgx.Matches(value);
            if (matches.Count > 0)
                {
                
                foreach (Match match in matches)

                    if (match.Value.ToLower().StartsWith("##urllokal"))
                    {
                        result = result.Replace(match.Value, "<a href=\"http://www.dr.dk\">www.dr.dk</a>");
                    }


                    
                }

            return result;
        }


    }
}