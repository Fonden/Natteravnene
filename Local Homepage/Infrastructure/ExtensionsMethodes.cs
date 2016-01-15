/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

<WORK'S NAME> is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DTA
{
    public static class ExtensionsMethodes
    {
        /// <summary>
        /// Convert Whitespaces in a string to single spaces
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            if (value == null) return value;
            return Regex.Replace(value, @"\s+", " ");
        }

        /// <summary>
        /// Convert Whitespaces in a string to single spaces and trim teh string.
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string TrimAndReduce(this string value)
        {
            if (value == null) return value;
            return ConvertWhitespacesToSingleSpaces(value).Trim();
        }

        /// <summary>
        /// Removes spaces and conver to Uppercase.
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string NoSpaceToUpper(this string value)
        {
            if (value == null) return value;
            return value.Replace(" ", "").ToUpper();
        }

        /// <summary>
        /// Trim a Phone/mobile number.
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string PhoneTrim(this string value)
        {
            if (value == null) return value;
            string result = value.Replace(" ", "");

            return result.Substring(0, result.Length > 15 ? 15 : result.Length); 
        }

        /// <summary>
        /// Trim Email to lowercase and remove spaces.
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string EmailTrim(this string value)
        {
            if (value == null) return value;
            return value.Replace(" ", "").ToLower();
        }

        /// <summary>
        /// Trim Name with Capital first letter and rest Lower case, on each word.
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string NameTrim(this string value)
        {
            if (value == null | value == string.Empty) return value;
            String str = value.TrimAndReduce();
            String[] strSegments = str.Split(' ');
            str = "";
            foreach (String s in strSegments)
            {

                char[] a = s.ToLower().ToCharArray();
                a[0] = char.ToUpper(a[0]);
                str += new string(a) + " ";
            }

            strSegments = str.Split('-');
            str = "";
            foreach (String s in strSegments)
            {
                if (!string.IsNullOrWhiteSpace(str)) str += "-";
                char[] a = s.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                str += new string(a);
            }

            return str.Trim();
        }

        /// <summary>
        /// Trim sentence converting whitespaces to singlespaces, trim and first letter capital
        /// </summary>
        /// <param name="value">String to be trimed</param>
        /// <returns>Formatted string or null</returns>
        public static string SentenceTrim(this string value)
        {
            if (value == null) return value;
            if (String.IsNullOrWhiteSpace(value)) return String.Empty;
            char[] a = value.TrimAndReduce().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string GeneratePassword(this string value)
        {
            Random Ran = new Random();
            string[] Sound = { "bio", "ota", "tin", "go", "to", "lo", "sko", 
            "bo", "so", "li", "fo", "pee", "us", "pi", "ge", 
            "ta", "la", "fi", "da", "va", "en", "ti", "bu", "en", "pas", "las"};
            string tmpPas = string.Empty;
            for (int i = 1; i < 5; i++)
            {
                tmpPas += Sound[(int)Ran.Next(0, Sound.Length)];
            }
            return tmpPas;
        }


        /// <summary>
        /// Create a valid filename converting national caracters
        /// </summary>
        /// <param name="value">String which needs to be converted into a filename</param>
        /// <returns>A valid filename</returns>
        /// <remarks></remarks>
        public static string ValidFileName(this string value)
        {
            if (value == null) return value;

            //Transform a string into a valid filename
            string validFileName = value.Trim().ToLower();

            //replacing key characters
            validFileName = validFileName.Replace("æ", "ae")
                .Replace("ø", "oe")
                .Replace("å", "aa")
                .Replace("ä", "a")
                .Replace("Б", "B")
                .Replace("б", "b")
                .Replace("Г", "G")
                .Replace("г", "g")
                .Replace("Д", "D")
                .Replace("д", "d")
                .Replace("Ё", "E")
                .Replace("ё", "e")
                .Replace("Ж", "ZH")
                .Replace("ж", "zh")
                .Replace("З", "Z")
                .Replace("з", "z")
                .Replace("И", "I")
                .Replace("и", "i")
                .Replace("Й", "J")
                .Replace("й", "j")
                .Replace("Л", "L")
                .Replace("л", "l")
                .Replace("П", "P")
                .Replace("п", "p")
                .Replace("Ф", "F")
                .Replace("ф", "f")
                .Replace("Ц", "C")
                .Replace("ц", "c")
                .Replace("Ш", "SH")
                .Replace("ш", "sh")
                .Replace("Щ", "SHCH")
                .Replace("щ", "shch")
                .Replace("Ы", "Y")
                .Replace("Э", "E")
                .Replace("э", "e")
                .Replace("Ю", "YU")
                .Replace("ю", "yu")
                .Replace("Я", "YA")
                .Replace("я", "ya")
                .Replace("&", "")
                .Replace("?", "")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("®", "")
                .Replace("´", "")
                .Replace("’", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("é", "e")
                .Replace("á", "a")
                .Replace("ü", "u")
                .Replace("ö", "o")
                .Replace("ó", "o")
                .Replace("ä", "a")
                .Replace("+", "plus")
                .Replace(" ", "-");

            validFileName = validFileName.Replace("---", "-").Replace("--", "-");

            //Replacinf remaining invalid char with ""
            foreach (char invalChar in System.IO.Path.GetInvalidFileNameChars())
            {
                validFileName = validFileName.Replace(invalChar.ToString(), "").Trim();
            }

            //Cut to max length
            if ((validFileName.Length > 160))
                validFileName = validFileName.Remove(156);

            return HttpUtility.UrlEncode(validFileName.ToLower());
        }

        /// <summary>
        /// Create a valid Domainname converting national caracters
        /// </summary>
        /// <param name="value">String which needs to be converted into a filename</param>
        /// <returns>A valid filename</returns>
        /// <remarks></remarks>
        public static string ValidDomainName(this string value)
        {
            if (value == null) return value;

            //Transform a string into a valid filename
            string validDomainName = value.Trim().ToLower();

            //replacing key characters
            validDomainName = validDomainName.Replace("æ", "ae")
                .Replace("ø", "oe")
                .Replace("å", "aa")
                .Replace("ä", "a")
                .Replace("Б", "B")
                .Replace("б", "b")
                .Replace("Г", "G")
                .Replace("г", "g")
                .Replace("Д", "D")
                .Replace("д", "d")
                .Replace("Ё", "E")
                .Replace("ё", "e")
                .Replace("Ж", "ZH")
                .Replace("ж", "zh")
                .Replace("З", "Z")
                .Replace("з", "z")
                .Replace("И", "I")
                .Replace("и", "i")
                .Replace("Й", "J")
                .Replace("й", "j")
                .Replace("Л", "L")
                .Replace("л", "l")
                .Replace("П", "P")
                .Replace("п", "p")
                .Replace("Ф", "F")
                .Replace("ф", "f")
                .Replace("Ц", "C")
                .Replace("ц", "c")
                .Replace("Ш", "SH")
                .Replace("ш", "sh")
                .Replace("Щ", "SHCH")
                .Replace("щ", "shch")
                .Replace("Ы", "Y")
                .Replace("Э", "E")
                .Replace("э", "e")
                .Replace("Ю", "YU")
                .Replace("ю", "yu")
                .Replace("Я", "YA")
                .Replace("я", "ya")
                .Replace("&", "")
                .Replace("?", "")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("®", "")
                .Replace("´", "")
                .Replace("’", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("é", "e")
                .Replace("á", "a")
                .Replace("ü", "u")
                .Replace("ö", "o")
                .Replace("ó", "o")
                .Replace("ä", "a")
                .Replace("+", "plus")
                .Replace(" ", "-");

            validDomainName = validDomainName.Replace("---", "-").Replace("--", "-");


            return Regex.Replace(validDomainName, @"^a-z0-9\-\.", "");
        }


        /// <summary>
        /// Create a valid .DK domainname converting national caracters
        /// </summary>
        /// <param name="value">String which needs to be converted into a filename</param>
        /// <returns>A valid filename</returns>
        /// <remarks></remarks>
        public static string ValidDKDomainName(this string value)
        {
            if (value == null) return value;

            //Transform a string into a valid filename
            string validDomainName = value.Trim().ToLower();

            //replacing key characters
            validDomainName = validDomainName
                .Replace("Б", "B")
                .Replace("б", "b")
                .Replace("Г", "G")
                .Replace("г", "g")
                .Replace("Д", "D")
                .Replace("д", "d")
                .Replace("Ё", "E")
                .Replace("ё", "e")
                .Replace("Ж", "ZH")
                .Replace("ж", "zh")
                .Replace("З", "Z")
                .Replace("з", "z")
                .Replace("И", "I")
                .Replace("и", "i")
                .Replace("Й", "J")
                .Replace("й", "j")
                .Replace("Л", "L")
                .Replace("л", "l")
                .Replace("П", "P")
                .Replace("п", "p")
                .Replace("Ф", "F")
                .Replace("ф", "f")
                .Replace("Ц", "C")
                .Replace("ц", "c")
                .Replace("Ш", "SH")
                .Replace("ш", "sh")
                .Replace("Щ", "SHCH")
                .Replace("щ", "shch")
                .Replace("Ы", "Y")
                .Replace("Э", "E")
                .Replace("э", "e")
                .Replace("Ю", "YU")
                .Replace("ю", "yu")
                .Replace("Я", "YA")
                .Replace("я", "ya")
                .Replace("&", "")
                .Replace("?", "")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("®", "")
                .Replace("´", "")
                .Replace("’", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("á", "a")
                .Replace("+", "plus")
                .Replace(" ", "-");

            validDomainName = validDomainName.Replace("---", "-").Replace("--", "-");

            return Regex.Replace(validDomainName, @"^a-zäåæéöøü0-9\-\.", "");
        }

        public static string CleanHTML(this string value)
        {
            return Regex.Replace(
                            value,
                            "(<(?<t>script|object|applet|embbed|frameset|iframe|form|textarea)(\\s+.*?)?>.*?</\\k<t>>)"
                                + "|(<(script|object|applet|embbed|frameset|iframe|form|input|button|textarea)(\\s+.*?)?/?>)"
                                + "|((?<=<\\w+)((?:\\s+)((?:on\\w+=((\"[^\"]*\")|('[^']*')|(.*?)))|(?<a>(?!on)\\w+=((\"[^\"]*\")|('[^']*')|(.*?)))))*(?=/?>))",
                            match =>
                            {
                                if (!match.Groups["a"].Success)
                                {
                                    return string.Empty;
                                }
        
                                var attributesBuilder = new StringBuilder();
        
                                foreach(Capture capture in match.Groups["a"].Captures)
                                {
                                    attributesBuilder.Append(' ');
                                    attributesBuilder.Append(capture.Value);
                                }
        
                                return attributesBuilder.ToString();
                            },
                            RegexOptions.IgnoreCase
                                | RegexOptions.Multiline
                                | RegexOptions.ExplicitCapture
                                | RegexOptions.CultureInvariant
                                | RegexOptions.Compiled
                        );

        }

        public static string ToString(this DateTime? dateTime, string format)
        {
            return dateTime.ToString(format, "");
        }

        public static string ToString(this DateTime? dateTime, string format, string returnIfNull)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString(format);
            else
                return returnIfNull;
        }

        public static string ToShortDateString(this DateTime? dateTime)
        {
            return dateTime.ToShortDateString("");
        }

        public static string ToShortDateString(this DateTime? dateTime, string returnIfNull)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToShortDateString();
            else
                return returnIfNull;
        }

        /// <summary>
        /// Gets the DataAnnotation DisplayName attribute for a given enum (for displaying enums values nicely to users)
        /// </summary>
        /// <param name="value">Enum value to get display for</param>
        /// <returns>Pretty version of enum (if there is one)</returns>
        /// <remarks>
        /// Inspired by :
        ///     http://stackoverflow.com/questions/9328972/mvc-net-get-enum-display-name-in-view-without-having-to-refer-to-enum-type-in-vi
        /// </remarks>
        public static string DisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];
            string outString = "";

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attrs.Any())
            {
                var displayAttr = ((DisplayAttribute)attrs[0]);

                outString = displayAttr.Name;

                if (displayAttr.ResourceType != null)
                {
                    outString = displayAttr.GetName();
                }
            }
            else
            {
                outString = value.ToString();
            }

            return outString;
        }


        public static string ScrubHtml(this string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        public static string String200(this string value)
        {
            return value.Substring(0, value.Length < 200 ? value.Length : 200);
        }
        
    }

    public static class Extensions
    {

        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        
    }

    public static class UriHelperExtensions
    {
        // Prepend the provided path with the scheme, host, and port of the request.
        public static string FormatAbsoluteUrl(this Uri url, string path)
        {
            return string.Format(
               "{0}/{1}", url.FormatUrlStart(), path.TrimStart('/'));
        }

        // Generate a string with the scheme, host, and port if not 80.
        public static string FormatUrlStart(this Uri url)
        {
            return string.Format("{0}://{1}{2}", url.Scheme,
               url.Host, url.Port == 80 ? string.Empty : ":" + url.Port);
        }
    }


    public static class UrlExtensions
    {
        public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false)
        {
            var path = urlHelper.Content(contentPath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return toAbsolute ? url.AbsoluteUri : path;
        }
    }


    public static class ShuffleList
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

   

}