/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using NR.Abstract;
using NR.Entity;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NR.Infrastructure;
using System.Web.Mvc;
using System.Web.Routing;

namespace NR.Infrastructure
{
    public class Processes
    {

        public static string progress { get; set; }

        public static bool InProgress { get; set; }

        public string HandshakeUrl;

        private INRRepository reposetory;

        public Processes(INRRepository NTRepository)
        {
            reposetory = NTRepository;
        }

        /// <summary>
        /// Send tem notifications and team notes
        /// </summary>
        public void SendTeamTexts()
        {

            //DateTime LookAhead = DateTime.Now.AddDays(7);  //Time to look ahead for teams reminder normal reminder is sent 2 days before so plenty of room
            //DateTime Deadline = DateTime.Now.AddHours(1); //If reminde is no sent 6 hours before no need to go head
            //List<Team> TeamsTest = reposetory.Teams.ToList();

            List<Team> Teams = reposetory.TeamsToProcess(DateTime.Now.AddDays(7), DateTime.Now.AddHours(1));

            LogFile.Write("Process : Teams Selected (" + Teams.Count() + ")");

            foreach (Team team in Teams)
            {
                string result = "Process : TeamID(" + team.TeamID + ")";
                //LogFile.Write("Process: (TeamID = " + team.TeamID + ", Team OK = " + team.OK + ",ReminderTextSend = " + team.ReminderTextSent + ")");
                if (team.Association == null) continue;
                Association association = team.Association;

                //Team notification
                if (association.SendTeamText && team.OK && !team.ReminderTextSent && !(string.IsNullOrWhiteSpace(association.TeamMessageTeamLeader) & string.IsNullOrWhiteSpace(association.TeamMessage)))
                {
                    DateTime ReminderTextSendDeadline = DateTime.Now.Date.AddDays(association.SendTeamTextDays);
                    DateTime CountStart = team.Start.Hour < 5 ? team.Start.Date.AddDays(-1) : team.Start.Date;

                    //LogFile.Write("Process: OK to send (" + team.TeamID + ") - ( CountStart = " + CountStart.ToString() + ", ReminderTextSendDeadline = " + ReminderTextSendDeadline.ToString() + ", team.start = " + team.Start.AddMinutes(-10).ToString() + " / " + DateTime.Now.ToString() + ")");
                    result += "*Team notification-";
                    result += "CountStart/ReminderTextSendDeadline/team.start(" + CountStart.ToString() + "/" + ReminderTextSendDeadline.ToString() + "/" + team.Start.AddMinutes(-10).ToString() + ")";
                    if (CountStart <= ReminderTextSendDeadline && team.Start.AddMinutes(-10) >= DateTime.Now)
                    {
                        //LogFile.Write("Process: OK to Time " + team.TeamID + ")");
                        result += ">>READY<<";



                        foreach (Person P in team.Teammembers)
                        {

                            if (String.IsNullOrWhiteSpace(P.Mobile))
                            {
                                result += "/**NO MOBILE**";
                                continue;
                            }
                            result += "/" + P.Mobile;
#if DUMMYTEXT
                            ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", association.TextServiceProviderUserName, association.TextServiceProviderPassword);
#else
                            ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(association.TextServiceProvider, association.TextServiceProviderUserName, association.TextServiceProviderPassword);
#endif
                            string Message;
                            if (team.TeamLeaderId == P.PersonID)
                            {
                                Message = association.TeamMessageTeamLeader;
                                SMSGateway.FromText = General.SystemTextMessagesFrom;
                            }
                            else
                            {
                                Message = association.TeamMessage;
                                if (string.IsNullOrWhiteSpace(team.Teamleader.Mobile))
                                {
                                    SMSGateway.FromText = General.SystemTextMessagesFrom;
                                }
                                else
                                {
                                    SMSGateway.From = team.Teamleader;
                                }
                            }

                            if (team.Trial)
                            {
                                Message += "\n\r\n\r-----------\n\r\n\r" + DomainStrings.Trial;
                                if (!string.IsNullOrWhiteSpace(team.TrialInfo)) Message += ": " + team.TrialInfo;
                            }


                            //SMSGateway.FromText = General.SystemTextMessagesFrom;
                            SMSGateway.Message = Message.ReplaceTagTeamMembers(team, P).ReplaceTagTeamStart(team).ReplaceTagPerson(P);
                            SMSGateway.Recipient = new List<Person> { P };
                            if (DateTime.Now.AddMinutes(1).Hour < 11) SMSGateway.DeliveryTime = DateTime.Now.AddMinutes(1).Date.AddHours(11);
                            reposetory.NewTextMessage(SMSGateway, association.AssociationID);

                            if (HandshakeUrl != null && HandshakeUrl.Contains("##")) SMSGateway.HandShakeUrl = HandshakeUrl.Replace("##", SMSGateway.TextId);

                            if (SMSGateway.Send())
                            {
                                //team.ReminderTextSent = true;
                                //reposetory.SaveTeam(team);
                                reposetory.TeamReminderTextSent(team.TeamID);
                            };
                        }

                    }
                    else
                    {
                        result += ">>NO<<";
                    }
                    LogFile.Write(result);
                }
                else
                {
                    result += "*Team notification-NO-";
                    if (!association.SendTeamText) result += ", SendTeamText(" + association.SendTeamText + ")";
                    if (!team.OK) result += ", TeamOK(" + team.OK + ")";
                    if (team.ReminderTextSent) result += ", ReminderTextSent(" + team.ReminderTextSent + ")";
                    if (string.IsNullOrWhiteSpace(association.TeamMessageTeamLeader)) result += "TeamMessageTeamLeader(" + string.IsNullOrWhiteSpace(association.TeamMessageTeamLeader) + ")";
                    if (string.IsNullOrWhiteSpace(association.TeamMessage)) result += "TeamMessageTeamLeader(" + string.IsNullOrWhiteSpace(association.TeamMessage) + ")";
                    //LogFile.Write(result);
                }


                //Teamnotes
                result += "\r\n*Teamnotes-(" + (association.SendNoteTeamleader && team.OK && team.TeamLeaderId != Guid.Empty && !team.NoteTextSent &&
                   (!string.IsNullOrWhiteSpace(team.Note) | (association.UseKeyBox && !string.IsNullOrWhiteSpace(association.KeyBoxcode)) | team.Trial)) + ")" ;


                if (association.SendNoteTeamleader && team.OK && team.TeamLeaderId != Guid.Empty && !team.NoteTextSent &&
                   (!string.IsNullOrWhiteSpace(team.Note) | (association.UseKeyBox && !string.IsNullOrWhiteSpace(association.KeyBoxcode)) | team.Trial))
                {
                    DateTime ReminderTextSendDeadline = DateTime.Now.Date.AddDays(12);
                    DateTime CountStart = team.Start.Hour < 5 ? team.Start.Date.AddDays(-1) : team.Start.Date;
                    result += "*Teamnotes-";
                    result += "team.Start.AddHours(-8)/team.Start.AddMinutes(-10)/team.start(" + team.Start.AddHours(-8) + "/" + team.Start.AddMinutes(+10) + "/" + team.Start.AddMinutes(+10).ToString() + ")";

                    if (team.Start.AddHours(-8) <= DateTime.Now && team.Start.AddMinutes(-30) >= DateTime.Now)
                    {
                        Person TeamLeader = team.Teammembers.Where(x => x.PersonID == team.TeamLeaderId).FirstOrDefault();
                        if (TeamLeader != null)
                        {
#if DUMMYTEXT
                            ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", association.TextServiceProviderUserName, association.TextServiceProviderPassword);
#else
                            ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(association.TextServiceProvider, association.TextServiceProviderUserName, association.TextServiceProviderPassword);
#endif
                            string Message = "";
                            string KeyboxMessage;

                            if (association.SendNoteTeamleader & !string.IsNullOrWhiteSpace(team.Note))
                            {
                                Message = General.SystemTextMessagesTeamNote.ReplaceTagTeamStart(team).ReplaceTagPerson(TeamLeader) + "\n\r" + team.Note;
                            }
                            if (association.UseKeyBox & !string.IsNullOrWhiteSpace(association.KeyBoxcode))
                            {
                                KeyboxMessage = @General.KeyBoxHeadline + " " + association.KeyBoxcode;
                                Message += (string.IsNullOrWhiteSpace(Message) ? KeyboxMessage : "\n\r\n\r----\n\r\n\r" + KeyboxMessage);
                            }
                            if (team.Trial)
                            {
                                String TrialMessage = DomainStrings.Trial;
                                if (!string.IsNullOrWhiteSpace(team.TrialInfo)) TrialMessage += ": " + team.TrialInfo;
                                Message += (string.IsNullOrWhiteSpace(Message) ? TrialMessage : "\n\r\n\r----\n\r\n\r" + TrialMessage);
                            }

                            SMSGateway.FromText = General.SystemTextMessagesFrom;
                            SMSGateway.Message = Message;
                            SMSGateway.Recipient = new List<Person> { TeamLeader };
                            if (team.Start.AddHours(-association.NoteTextTime) < DateTime.Now) SMSGateway.DeliveryTime = team.Start.AddHours(-association.NoteTextTime);
                            reposetory.NewTextMessage(SMSGateway, association.AssociationID);

                            if (HandshakeUrl != null && HandshakeUrl.Contains("##")) SMSGateway.HandShakeUrl = HandshakeUrl.Replace("##", SMSGateway.TextId);

                            if (SMSGateway.Send())
                            {
                                //team.NoteTextSent = true;
                                //reposetory.SaveTeam(team);
                                reposetory.TeamNoteTextSent(team.TeamID);
                            };
                        }
                    }
                    else
                    {
                        result += "*Teamnotes-NO-";
                        if (!association.SendNoteTeamleader) result += ", SendNoteTeamleader(" + association.SendNoteTeamleader + ")";
                        if (!team.OK) result += ", TeamOK(" + team.OK + ")";
                        if (team.TeamLeaderId == Guid.Empty) result += ", TeamLeaderId(" + team.TeamLeaderId + ")";
                        if (team.NoteTextSent) result += ", NoteTextSent(" + team.NoteTextSent + ")";
                        if (string.IsNullOrWhiteSpace(team.Note)) result += "TeamMessageTeamLeader(" + string.IsNullOrWhiteSpace(team.Note) + ")";
                        if (!association.UseKeyBox) result += ", .UseKeyBox(" + association.UseKeyBox + ")";
                        if (string.IsNullOrWhiteSpace(association.KeyBoxcode)) result += "KeyBoxcode(" + string.IsNullOrWhiteSpace(association.KeyBoxcode) + ")";
                        if (!team.Trial) result += ", Trial(" + team.Trial + ")";
                    }
                }
                LogFile.Write(result);
            }

        }

        /// <summary>
        /// Sends notification for new messages in the system
        /// </summary>
        public void SendMessages()
        {
            List<MessageReciver> Recivers = reposetory.ProcessMessageNotification();

            foreach (MessageReciver R in Recivers)
            {
                if (R.Message.Type == MessageType.shortMessage && R.Reciver != null && !string.IsNullOrWhiteSpace(R.Reciver.Mobile))
                {


#if DUMMYTEXT
                    ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance("NR.Infrastructure.DummyTextGateway", null, null);
#else
                    ITextMessage SMSGateway = TextServiceProviderFactory.GetTextServiceProviderrInstance(null, null, null);
#endif
                    if (R.Message != null && R.Message.Sender != null && string.IsNullOrWhiteSpace(R.Message.Sender.Mobile))
                    {
                        SMSGateway.FromText = General.SystemTextMessagesFrom;
                    }
                    else
                    {
                        SMSGateway.From = R.Message.Sender;
                    }
                    SMSGateway.Message = R.Message.Body + "\n\r---\n\r" + General.MessageNotificationFooter;
                    SMSGateway.Recipient = new List<Person> { R.Reciver };

                    if (HandshakeUrl != null && HandshakeUrl.Contains("##")) SMSGateway.HandShakeUrl = HandshakeUrl.Replace("##", SMSGateway.TextId);

                    if (SMSGateway.Send())
                    {
                        reposetory.MarkSent(R);
                        LogFile.Write("Send Message >>> ShortSend  Mobile: (" + R.Reciver.Mobile + ")");
                    }
                    else
                    {
                        LogFile.Write("Send Message >>> ShortSend ERROR  Mobile: (" + R.Reciver.Mobile + ")");
                    }
                    ;
                }
                else if (R.Reciver != null && !string.IsNullOrWhiteSpace(R.Reciver.Email))
                {
                    MessagNotification email = new MessagNotification();
                    // set up the email ...
                    email.To = R.Reciver.Email;
                    email.Subject = R.Message.Head;
                    email.ReplyTo = R.Message.Sender.Email;
                    email.Message = R.Message.Body;
                    email.Link = "http://eteelrande";
                    email.FromPerson = R.Message.Sender;
                    try
                    {
                        email.Send();
                        reposetory.MarkSent(R);
                        LogFile.Write("Send Message >>> LongSend  mail: (" + R.Reciver.Email + ")");
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Send Message Fejl >>> Messagetype " + R.Message.Type.ToString() + " Email: (" + R.Reciver.Email + ") + Mobile: (" + R.Reciver.Mobile + ")");
                    }
                }
                else if (R.Reciver == null)
                {
                    reposetory.MarkSent(R);
                    LogFile.Write("Send Message Fejl >>> Messagetype " + R.Message.Type.ToString() + " Reciever is null");
                }
                else
                {
                    reposetory.MarkSent(R);
                    LogFile.Write("Send Message Fejl >>> Messagetype " + R.Message.Type.ToString() + " Email: (" + R.Reciver.Email + ") + Mobile: (" + R.Reciver.Mobile + ")");
                }

            }


        }
    }
}