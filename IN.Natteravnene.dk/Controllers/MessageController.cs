/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using DTA;
using NR.Abstract;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IN.Natteravnene.dk.Controllers
{
    [Authorize]
    [HandleError500]
    public class MessageController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public MessageController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
        }

        // GET: Message
        public ActionResult Index(Guid? ID)
        {
            MessageListModel Messages = new MessageListModel()
            {
                Messages = reposetory.GetUserMessages(CurrentProfile.PersonID),
                SentMessages = reposetory.GetUserSentMessages(CurrentProfile.PersonID)
            };
            if (ID != null) Messages.PreloadID = (Guid)ID;
            return View(Messages);
        }

        public ActionResult Delete(Guid ID)
        {
            reposetory.DeleteMessage(ID, CurrentProfile.PersonID);

            return RedirectToAction("Index");
        }

        public ActionResult NewMessage()
        {
            List<SelectListItem> AssItems = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            AssItems.Insert(0, new SelectListItem { Text = General.GenderMale, Value = MistConversations.Int2Guid(4).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.GenderFemale, Value = MistConversations.Int2Guid(3).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Board, Value = MistConversations.Int2Guid(2).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Teamleaders, Value = MistConversations.Int2Guid(1).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.All, Value = MistConversations.Int2Guid(0).ToString() });


            MessageListModel Messages = new MessageListModel()
            {
                List = AssItems,
                NewMessageTo = new List<Guid>()
            };

            return View(Messages);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewMessage(MessageListModel MessageContext)
        {
            List<NR.Models.NRMembership> Active = new List<NR.Models.NRMembership>();
            if (MessageContext.NewMessageTo == null || !MessageContext.NewMessageTo.Any())
            {
                ModelState.AddModelError("NewMessageTo", General.ErrorNoRecipients);

            }

            if (MessageContext.Short)
            {
                if (String.IsNullOrWhiteSpace(MessageContext.BodyShort)) ModelState.AddModelError("BodyShort", General.ErrorNoBody);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(MessageContext.Head) & String.IsNullOrWhiteSpace(MessageContext.Body)) ModelState.AddModelError("", General.ErrorNoHeadOrBody);
            }

            if (ModelState.IsValid)
            {
                //An entity object cannot be referenced by multiple instances of IEntityChangeTracker

                Message Msg = new Message
                {
                    Recivers = new List<MessageReciver>(),
                    SenderID = CurrentProfile.PersonID
                };

                List<Guid> Recivers = new List<Guid>();

                Active = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID);



                foreach (Guid PersonID in MessageContext.NewMessageTo)
                {
                    if (PersonID == MistConversations.Int2Guid(0))
                    {
                        Recivers = Active.Select(m => m.Person.PersonID).ToList();
                    }
                    else if (PersonID == MistConversations.Int2Guid(1))
                    {
                        Recivers.AddRange(reposetory.GetAssociationTeamLeadersPersons(CurrentProfile.AssociationID).Select(m => m.PersonID).ToList());
                    }
                    else if (PersonID == MistConversations.Int2Guid(2))
                    {
                        Recivers.AddRange(reposetory.GetAssociationBoardPersons(CurrentProfile.AssociationID).Select(m => m.PersonID).ToList());
                    }
                    else if (PersonID == MistConversations.Int2Guid(3))
                    {
                        Recivers.AddRange(reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID, Gender.Woman).Select(m => m.PersonID).ToList());
                    }
                    else if (PersonID == MistConversations.Int2Guid(4))
                    {
                        Recivers.AddRange(reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID, Gender.Man).Select(m => m.PersonID).ToList());
                    }
                    else
                    {
                        Recivers.Add(PersonID);
                    }
                }

                //Recivers = Recivers.Distinct().ToList();

                Msg.Recivers = Recivers.Distinct().Select(Id => new MessageReciver { ReciverID = Id }).ToList();
                //.Add(
                //   new MessageReciver
                //   {
                //       ReciverID = Active.Where(p => p.PersonID == PersonID).FirstOrDefault().PersonID
                //   }
                //   );
                if (MessageContext.Short)
                {
                    Msg.Body = MessageContext.BodyShort;
                    Msg.Type = MessageType.shortMessage;
                }
                else
                {
                    Msg.Head = MessageContext.Head;
                    Msg.Body = MessageContext.Body;
                    Msg.Type = MessageType.LongMessage;
                }
                reposetory.MessageSend(Msg);
                ViewBag.FormSucces = true;

            }


            List<SelectListItem> AssItems = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID).Select(d => new SelectListItem { Value = d.Person.PersonID.ToString(), Text = d.Person.FullName }).ToList();
            AssItems.Insert(0, new SelectListItem { Text = General.GenderMale, Value = MistConversations.Int2Guid(4).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.GenderFemale, Value = MistConversations.Int2Guid(3).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Board, Value = MistConversations.Int2Guid(2).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.Teamleaders, Value = MistConversations.Int2Guid(1).ToString() });
            AssItems.Insert(0, new SelectListItem { Text = General.All, Value = MistConversations.Int2Guid(0).ToString() });

            MessageContext.List = AssItems;

            return View(MessageContext);
        }

        public ActionResult View(Guid ID)
        {

            return View();
        }

        public ActionResult _Message(Guid ID)
        {
            Message Message = reposetory.GetMessage(ID, CurrentProfile.PersonID);
            CurrentProfile.Clear();

            return PartialView(Message);


        }

        public ActionResult MailTo()
        {
            List<NRMembership> ActivePeople = reposetory.GetAssociationActivePersons(CurrentProfile.AssociationID);

            foreach (NRMembership M in ActivePeople)
            {
                if (!string.IsNullOrWhiteSpace(M.Person.Email))
                {
                    ViewBag.Emails += M.Person.FullName.Trim() + "<" + M.Person.Email.Trim() + ">\r\n";
                    ViewBag.EmailsNoName += M.Person.Email.Trim() + "\r\n";
                }

            }
            return View();

        }

    }
}