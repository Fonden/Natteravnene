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
    public class NetworkController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public NetworkController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        } 
        
        
        public ActionResult Index()
        {

            List<Network> networks = reposetory.GetAllNetworks();

            return View(networks);
        }

        public ActionResult Edit(Guid? id)
        {
            Network network = new Network();
            if (id != null)
            {
                network = reposetory.GetNetwork((Guid)id);

            }

            return View(network);
        }

        [HttpPost]
        public ActionResult Edit(Network network)
        {
            if (ModelState.IsValid)
            {
                Network dbNetwork = new Network();
                if (network.NetworkID != Guid.Empty)
                    dbNetwork = reposetory.GetNetwork(network.NetworkID);
                dbNetwork.NetworkName = network.NetworkName;
                dbNetwork.NetworkNotToShow = network.NetworkNotToShow;
                dbNetwork.NetworkNumber = network.NetworkNumber;
                if (reposetory.Save(dbNetwork))
                {
                    ModelState.Clear();
                    ViewBag.FormSucces = true;
                    return View(dbNetwork);
                }
                else
                {
                    ViewBag.HandledException = true;
                }
            }
            return View(network);
        }

        public ActionResult View(Guid id)
        {
            Network  network = reposetory.GetNetwork((Guid)id);
            if (network == null) return RedirectToAction("Index");
            return View(network);
        }
    }
}