/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using NR.Abstract;
using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NR.Controllers
{
    [Authorize]
    [HandleError500]
    public class WikiController : Controller
    {
        
         //Repository
        private INRRepository reposetory;

        public WikiController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }
        
        // GET: Admin/Wiki
        public ActionResult Index(string selectedLetter)
        {
            var model = new WikiViewModel { SelectedLetter = selectedLetter };

            model.FirstLetters = reposetory.GetWikiWords()
             .GroupBy(p => p.Word.Substring(0, 1))
             .Select(x => x.Key.ToUpper())
             .ToList();

            if (string.IsNullOrEmpty(selectedLetter) || selectedLetter == "Alle")
            {
                model.Words = reposetory
                    .GetWikiWords()
                    .ToList();
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    model.Words = reposetory.GetWikiWords()
                        .Where(p => numbers.Contains(p.Word.Substring(0, 1)))
                        .ToList();
                }
                else
                {
                    model.Words = reposetory.GetWikiWords()
                        .Where(p => p.Word.StartsWith(selectedLetter))
                        .ToList();
                }
            }

            return View(model);
            
        }

        public ActionResult View(Guid Id)
        {
            if (Id == Guid.Empty) return HttpNotFound();
            Wiki Wiki = reposetory.GetWikisItem(Id);
            if (Wiki == null) return HttpNotFound();


            return View(Wiki);
        }

        public ActionResult Logo()
        {

            return View();
        }

    }
}