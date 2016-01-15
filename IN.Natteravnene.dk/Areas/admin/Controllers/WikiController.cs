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

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator,Management")]
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
        public ActionResult Index()
        {
            List<Wiki> wikis = reposetory.GetWikis().OrderByDescending(n => n.Title).ToList();

            return View(wikis);

        }


        public ActionResult Edit(Guid? Id)
        {
            Wiki Wiki = new Wiki();
            if (Id != null) Wiki = reposetory.GetWikisItem((Guid)Id);
            if (Wiki == null) return HttpNotFound();
            if (Wiki.Words == null) Wiki.Words = new List<WikiWord>();
            Wiki.Words.Add(new WikiWord());
            return View(Wiki);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Wiki Wiki, string Action)
        {

            Wiki dbWiki = new Wiki();
            if (Wiki.WikiID != Guid.Empty) dbWiki = reposetory.GetWikisItem(Wiki.WikiID);
            if (dbWiki == null) return HttpNotFound();

            if (Wiki.Words == null)
            {
                Wiki.Words = new List<WikiWord> {
                    new WikiWord()
                };
            }
            else
            {
                Wiki.Words = Wiki.Words.GroupBy(w => w.Word.ToLower()).Select(w => w.First()).ToList();
            }

            if (Action == "addWord")
            {
                Wiki.Words.Add(new WikiWord());
                ModelState.Clear(); 
            }


            if (Action == "Edit" && ModelState.IsValid)
            {
               
                if (reposetory.RemoveWikiWords(dbWiki))
                {
                    //Wiki NewWiki = new Wiki();
                    dbWiki.Title = Wiki.Title;
                    dbWiki.Body = Wiki.Body;
                    dbWiki.Words = new List<WikiWord>();
                   // NewWiki.Trim();
                    
                    foreach (WikiWord w in Wiki.Words)
                    {
                        if (!string.IsNullOrEmpty(w.Word)) dbWiki.Words.Add(new WikiWord { Word = w.Word.Trim() });
                    }

                    dbWiki.Trim();

                    if (reposetory.Save(dbWiki))
                    {
                        Wiki = dbWiki;
                        ViewBag.FormSucces = true;
                        ModelState.Clear();
                    }
                }
            }

            


            return View(Wiki);
        }


        public ActionResult View(Guid Id)
        {
            if (Id == Guid.Empty) return HttpNotFound();
            Wiki Wiki = reposetory.GetWikisItem(Id);
            if (Wiki == null) return HttpNotFound();


            return View(Wiki);
        }

    }
}