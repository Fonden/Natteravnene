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
using System.Text;
using System.Data.Entity;
using NR.Models;

namespace NR.Abstract
{
    public interface INRRepository : IDisposable
    {
        
        
        #region Persons

        /// <summary>
        /// Read access to People entity object
        /// </summary>
        DbSet<Person> People { get; }
        DbSet<Team> Teams { get; }
        DbSet<MessageReciver> MessageRecivers { get; }
                             
        List<Person> GetAllPersons();

        List<Person> GetPrintNewsPersons();

        List<Person> GetVolenteerPersonEmails();

        List<Person> GetSeniorInstructors();

        List<Person> GetBoardMembers();

        List<Person> GetChairmen();

        /// <summary>
        /// Get current user based on Login UserID
        /// </summary>
        /// <returns>Person with Memberships</returns>
        Person CurrentUser();

        /// <summary>
        /// Get compleate information regarding a person 
        /// </summary>
        /// <param name="PersonID">PersonID</param>
        /// <returns>Person, Memeberships, Causes</returns>
        Person GetPersonComplete(Guid PersonID);

        DisplayProfile GetPersonDisplayProfile(Guid PersonID);

        /// <summary>
        /// Get a simplyfied person information based on UserID
        /// </summary>
        /// <param name="UserName">UserName</param>
        /// <returns>Person with memberships</returns>
        Person GetPerson(string UserName);

        /// <summary>
        /// Get a simplyfied person information based on PersonID
        /// </summary>
        /// <param name="PersonID">PersonID</param>
        /// <returns>Person with memberships</returns>
        Person GetPerson(Guid PersonID);

        /// <summary>
        /// Check against DB if pased UserName is uniq
        /// </summary>
        /// <param name="UserName">Username to be chrcked</param>
        /// <returns>Result of check</returns>
        bool IsUserNameUniqe(string Username);

        /// <summary>
        /// Generates a valid uniq username based om Firstname and Familiyname 
        /// </summary>
        /// <param name="person">Person calls on which uniq username i generated</param>
        void GenerateUniqueUserName(Person person);

        /// <summary>
        /// Get person based on Email or Mobile numer for "Forgot login" service
        /// </summary>
        /// <param name="Email">Email to serach for</param>
        /// <param name="Mobile">Mobile to search for</param>
        /// <returns>Persons with these criterias</returns>
        List<Person> GetPeople(string Email, string Mobile);





        List<NR.Models.NRMembership> GetAssociationActivePersons(Guid id);

        List<NRMembership> GetAssociationActivePersons(Guid id, Gender gender);

        List<NRMembership> GetAssociationLegalPersons(Guid id);

        List<Person> GetAssociationBoardPersons(Guid id);

        List<Person> GetAssociationTeamLeadersPersons(Guid id);

        List<NR.Models.NRMembership> GetAssociationPersons(Guid id);

        List<NRMembership> GetAssociationPersonsNoFeedback(Guid id, Guid Folder);
        

        

        

       

        /// <summary>
        /// Save a person after updating or creating
        /// </summary>
        /// <param name="person"></param>
        /// <returns>True when successfull, False if failed</returns>
        bool SavePerson(Person person);
        
        #endregion

        #region Membership

        NR.Models.NRMembership GetMembership(Guid ID);

        bool SavePerson(NR.Models.NRMembership person);

        #endregion

        #region Forening

        Association GetAssociation(Guid associationID);

        Association GetAssociationWithPages(Guid AssociationID);

        bool SaveAssociation(Association forening);

        bool Save(Association association);

        void CreateAssociation(Association forening);

        ICollection<Association> GetAssociations();
        List<AssociationListModel> GetAssociationList();

        BoardModel GetBoard(Guid AssociationID);
        BoardModelView GetBoardView(Guid Id);
        void SaveBoard(BoardModel model);

        AccessModel GetAccess(Guid AssociationID);

        void SaveAccess(List<PersonAccess> data);

       

        #endregion

        #region Associations

        Sponsor GetSponsor(Guid SponsorID);
        
        bool DeleteSponsor(Guid Id);

        bool Save(Sponsor sponsor);

        #endregion

        #region Netværk

        List<Network> GetAllNetworks();

        List<Network> GetActiveNetworks();

        Network GetNetwork(Guid NetworkID);

        bool Save(Network network);

        #endregion

        #region Locations

        Location GetLocation(Guid ID);

        List<Location> GetNationalLocations();

        bool SaveLocation(Location location);

        #endregion

        #region Notification

        List<Notification> GetNotifications(Guid Reciver);

        Notification Notify(object RegardingObject, String Message);

        void NotifyAddAdministration(Notification notification);

        void NotifyAddPerson(Notification notification, Person person);

        void NotifyAddBoard(Notification notification, Guid AssociationID);

        void NotifyAddAssociation(Notification notification, Guid AssociationID);

        void NotifyAddPlanner(Notification notification, Guid AssociationID);

        void NotifyAddTeam(Notification notification, Team team);

        void NotifyAddSystem(Notification notification);

        void NotifySave(Notification notification);

        Notification NotificationRead(Guid Notification, Guid Person);

        void NotificationClear(Guid Reciver);
        
        #endregion

        #region Folders / Teams

        IList<Folder> GetAssociationFolders(Guid Id);

        Folder GetFolder(Guid Id);

        bool ChangeStatus(Folder folder);

        bool SaveFolder(Folder Folder);
        bool DeleteFolder(Guid Id);
        bool SaveTeam(Team Team);
        bool Delete(Team Team);

        #endregion

        #region Planning

        List<MessageReciver> ProcessMessageNotification();

        void MarkSent(MessageReciver Reciver);
        void TeamNoteTextSent(Guid Id);
        void TeamReminderTextSent(Guid Id);

        #endregion

        #region Planning

        List<Team> GetMyTeams(Guid id);

        List<Team> GetOpenTeams(Guid id, Guid Association, int minTeams);

        List<Team> GetAssociationPlan(Guid id);

        Team GetTeam(Guid Id);

        List<FolderUserAnswer> GetFolderUserAnswers(Guid FolderId);

        FolderUserAnswer GetFolderUserAnswer(Guid PersonId, Guid FolderId);

        bool SaveFolderUserAnswer(FolderUserAnswer Answer);

        Team UpdateStatus(Guid Team, bool status);

        #endregion

        #region Messages
        List<Message> GetUserMessages(Guid Id);

        List<Message> GetUserUnreadMessages(Guid Id);

        List<Message> GetUserUnreadMessages(Guid Id, out int count);

        List<Message> GetUserSentMessages(Guid Id);

        Message GetMessage(Guid Id, Guid PersonID);

        void MessageSend(Message message);

        void DeleteMessage(Guid MessageID, Guid PersonID);

        #endregion

        #region Event

        List<Event> GetFrontEvents();

        List<Event> GetUserEvents(int? index);

        List<Event> GetEvents();

        Event GetEventItem(Guid id);

        bool Save(Event Evt);


        #endregion


        #region News

        List<News> GetFrontNews();

        List<News> GetNews();

        List<News> GetUserNews(int? index, bool Editor);

        List<News> GetUserNews(int? index);

        News GetNewsItem(Guid id);

        News GetNewsItemCount(Guid id);

        bool Save(News news);

        void DeleteNews(Guid NewsID);

        #endregion

        #region Wikis

        List<Wiki> GetWikis();

        Wiki GetWikisItem(Guid id);

        List<WikiWord> GetWikiWords();

        bool RemoveWikiWords(Wiki wiki);

        bool Save(Wiki wiki);

        #endregion


        #region SI

        SIViewModel GetSIViewModel(Guid id);

        #endregion

        #region Causes

        List<Cause> GetCauses();

        Cause GetCauseItem(Guid id);

        bool Save(Cause cause);

        #endregion

        #region Content

       Content GetContent(Guid ID);

       bool Save(Content content);

        #endregion

       #region Files (AFile)

       List<AFile> GetFiles();

       AFile GetFile(Guid ID);

       bool Save(AFile file);

       #endregion
       
        #region Leads

       List<Lead> GetLeads();

       List<Lead> GetLeads(Guid associationId);

       Lead GetLead(Guid lead);

       bool Save(Lead lead);

        #endregion


       #region TextMessages

       void NewTextMessage(ITextMessage Message, Guid AssociationId);

       List<TextMessage> GetMessages(Guid? Id);

       void UpdateStatusMessage(int ID, SMSStatus status);


       #endregion

       #region Processes

       List<Team> TeamsToProcess(DateTime LookAhead, DateTime Deadline);
      

       #endregion
       
        

    }
}
