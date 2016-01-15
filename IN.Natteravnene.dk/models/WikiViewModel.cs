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
using System.Web;

namespace NR.Models
{
    public class WikiViewModel
    {
        public List<WikiWord> Words { get; set; }
        public List<string> Alphabet
        {
            get
            {
                var alphabet = Enumerable.Range(65, 26).Select(i => ((char)i).ToString()).ToList();
                alphabet.Add("Æ");
                alphabet.Add("Ø");
                alphabet.Add("Å");
                alphabet.Insert(0, "Alle");
                alphabet.Insert(1, "0-9");
                return alphabet;
            }
        }
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }
        public bool NamesStartWithNumbers
        {
            get
            {
                var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                return FirstLetters.Intersect(numbers).Any();
            }
        }
    }
}