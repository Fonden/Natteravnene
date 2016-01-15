/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using Mehdime.Entity;
using NR.Abstract;
using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using WebMatrix.WebData;

namespace NR.Entity
{
    public class NRRepository : INRRepository, IDisposable
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        private NRDbContext DbContext
		{
			get
			{
                var dbContext = _ambientDbContextLocator.Get<NRDbContext>();

				if (dbContext == null)
					throw new InvalidOperationException("No ambient DbContext of type UserManagementDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
				
				return dbContext;
			}
		}

        public NRRepository(IAmbientDbContextLocator ambientDbContextLocator)
		{
			if (ambientDbContextLocator == null) throw new ArgumentNullException("ambientDbContextLocator");
			_ambientDbContextLocator = ambientDbContextLocator;
		} 
        
        public NRRepository()
        {



        }

        public NRRepository(NRDbContext dbContext)
        {



        }

        #region Netværk

        public List<Network> GetAllNetworks()
        {
            return this.Networks.ToList();
        }

        public List<Network> GetActiveNetworks()
        {

            return this.Networks.Where(N => N.NetworkNotToShow == false).ToList();

        }

        public Network GetNetwork(Guid NetworkID)
        {

            return this.Networks.Include(n => n.Associations).Where(n => n.NetworkID == NetworkID).First();

        }

        public bool Save(Network network)
        {
            this.Entry(network).State = network.NetworkID == Guid.Empty ?
                            EntityState.Added :
                            EntityState.Modified;

            try
            {
                this.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion

        #region Personer

        /// <summary>
        /// Get list of seniorinstructors
        /// </summary>
        /// <returns></returns>
        public List<Person> GetSeniorInstructors()
        {
            return this.People.Include(P => P.SiAssociation).Where(P => P.SeniorInstructor == true).OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
        }

        public List<Person> GetAllPersons()
        {
            return this.People.OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
        }

        public List<Person> GetPrintNewsPersons()
        {
            return this.Memberships
                .Where(M => M.Person.PrintNewslettet & M.AbsentDate == null & M.Person.Address.Trim() != "" & M.Person.Zip.Trim() != "" & M.Person.City.Trim() != "")
                .Select(M => M.Person).Distinct()
                .OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
        }

        public List<Person> GetBoardMembers()
        {
            return this.Memberships
                .Where(M => M.BoardFunction == BoardFunctionType.Accountant |
                    M.BoardFunction == BoardFunctionType.BoardMember |
                    M.BoardFunction == BoardFunctionType.Chairman)
                .Select(M => M.Person).Include(P => Memberships)
                .OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
        }

        public List<Person> GetChairmen()
        {
            return this.Memberships
                .Where(M => M.BoardFunction == BoardFunctionType.Chairman)
                .Select(M => M.Person).Include(P => Memberships)
                .OrderBy(m => String.Concat(m.FirstName, " ", m.FamilyName)).ToList();
        }


        /// <summary>
        /// Get current user based on Login UserID
        /// </summary>
        /// <returns>Person with Memberships</returns>
        public Person CurrentUser()
        {
            //var dbContext = new NRDataContext();

            var curuser = this.People.Include("NRMemberships").Where(p => p.UserID == WebSecurity.CurrentUserId).FirstOrDefault();

            //dbContext.Entry(curuser)
            //    .Collection(n => n.Notifications)
            //    .Query()
            //    .Where(n => n.Read == false)
            //    .Include(n => n.Notification)
            //    .OrderBy(n => n.Notification.Created)
            //    .Load();

            return curuser;
        }

        /// <summary>
        /// Get compleate information regarding a person 
        /// </summary>
        /// <param name="PersonID">PersonID</param>
        /// <returns>Person, Memeberships, Causes</returns>
        public Person GetPersonComplete(Guid PersonID)
        {
            return this.People.Include("Memberships.Association").Include("Causes.Cause").Where(p => p.PersonID == PersonID).FirstOrDefault();
        }

        /// <summary>
        /// Get a simplyfied person information based on UserID
        /// </summary>
        /// <param name="UserName">UserName</param>
        /// <returns>Person with memberships</returns>
        public Person GetPerson(string UserName)
        {
            return this.People.Include(p => p.Memberships).Where(p => p.UserName == UserName).FirstOrDefault();
        }

        /// <summary>
        /// Get a simplyfied person information based on PersonID
        /// </summary>
        /// <param name="PersonID">PersonID</param>
        /// <returns>Person with memberships</returns>
        public Person GetPerson(Guid PersonID)
        {
            return this.People.Include(p => p.Memberships).Where(p => p.PersonID == PersonID).FirstOrDefault();
        }

        /// <summary>
        /// Check against DB if pased UserName is uniq
        /// </summary>
        /// <param name="UserName">Username to be chrcked</param>
        /// <returns>Result of check</returns>
        public bool IsUserNameUniqe(string UserName)
        {
            return this.People.Where(p => p.UserName == UserName).Count() == 0;
        }

        /// <summary>
        /// Generates a valid uniq username based om Firstname and Familiyname 
        /// </summary>
        /// <param name="person">Person calls on which uniq username i generated</param>
        public void GenerateUniqueUserName(Person person)
        {
            string UserName;
            string Ftxt = String.IsNullOrWhiteSpace(person.FirstName) ? "" : Regex.Replace(person.FirstName.ToUpper(), @"[^A-Z0-9ÆØÅ]", "");
            string Ltxt = String.IsNullOrWhiteSpace(person.FamilyName) ? "" : Regex.Replace(person.FamilyName.ToUpper(), @"[^A-Z0-9ÆØÅ]", "");
            StringBuilder Extra = new StringBuilder();
            int Fno = Ftxt.Length > 1 ? 1 : Ftxt.Length;
            int Lno = Ltxt.Length > 2 ? 2 : Ltxt.Length;
            int ULength = 3;

            do
            {
                ULength += 1;
                if ((Lno > Fno | Fno == Ltxt.Length) & Fno < Ftxt.Length)
                { Fno += 1; }
                else if (Lno < Ltxt.Length)
                { Lno += 1; }

                if (Fno + Lno < ULength)
                {
                    Random random = new Random();
                    char ch;
                    for (int i = 0; i < ULength - Lno - Fno; i++)
                    {
                        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                        Extra.Append(ch);
                    }
                }

                UserName = Ftxt.Substring(0, Fno).ToUpper() + Ltxt.Substring(0, Lno).ToUpper() + Extra.ToString().ToUpper();

            } while (!IsUserNameUniqe(UserName));

            person.UserName = UserName;

        }

        /// <summary>
        /// Get person based on Email or Mobile numer for "Forgot login" service
        /// </summary>
        /// <param name="Email">Email to serach for</param>
        /// <param name="Mobile">Mobile to search for</param>
        /// <returns>Persons with these criterias</returns>
        public List<Person> GetPeople(string Email, string Mobile)
        {
            return this.People.Include(p => p.Memberships).Where(p => (p.Email == Email && p.Email != "") | (p.Mobile == Mobile && p.Mobile != "")).ToList();
        }

        public List<NRMembership> GetAssociationActivePersons(Guid id)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id & m.Type == PersonType.Active & m.AbsentDate == null).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList();
        }

        public List<NRMembership> GetAssociationActivePersons(Guid id, Gender gender)
        {
            if (gender == Gender.NotDefined) return GetAssociationActivePersons(id);
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id & m.Type == PersonType.Active & m.AbsentDate == null & m.Person.Gender == gender).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList();
        }

        public List<NRMembership> GetAssociationLegalPersons(Guid id)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id & (m.Type == PersonType.Active | m.Type == PersonType.Passiv) & m.AbsentDate == null).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList();
        }

        public List<Person> GetAssociationBoardPersons(Guid id)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id).Where(m => m.Type == PersonType.Active & m.AbsentDate == null & m.BoardFunction != BoardFunctionType.none & m.BoardFunction != BoardFunctionType.Auditor & m.BoardFunction != BoardFunctionType.AuditorAlternate).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).Select(m => m.Person).ToList();
        }

        public List<Person> GetAssociationTeamLeadersPersons(Guid id)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id).Where(m => m.Type == PersonType.Active & m.AbsentDate == null & m.Teamleader).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).Select(m => m.Person).ToList();
        }

        public List<NRMembership> GetAssociationPersons(Guid id)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id & m.AbsentDate == null).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList();
        }

        public List<NRMembership> GetAssociationPersonsNoFeedback(Guid id, Guid Folder)
        {
            return this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == id & m.Type == PersonType.Active & m.AbsentDate == null & !m.Person.FolderAnswers.Where(F => F.FolderID == Folder).Any()).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList();
        }

        /// <summary>
        /// Save or update person
        /// </summary>
        /// <param name="person">Person object to be updated</param>
        /// <returns>Result of save</returns>
        public bool SavePerson(Person person)
        {
            if (person.PersonID == Guid.Empty)
            { this.Entry(person).State = EntityState.Added; }
            else
            { this.Entry(person).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save person");
                return false;
            }

            return true;
        }

        #endregion

        #region Membership

        public NR.Models.NRMembership GetMembership(Guid ID)
        { return this.Memberships.Include("Person").Where(M => M.MembershipID == ID).FirstOrDefault(); }

        public bool SavePerson(NR.Models.NRMembership person)
        {
            if (person.MembershipID == Guid.Empty)
            { this.Entry(person).State = EntityState.Added; }
            else
            { this.Entry(person).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Associations

        public void CreateAssociation(Association association)
        {
            //TODO: Create Association
        }

        public ICollection<Association> GetAssociations()
        {
            return this.Associations.Include(a => a.Network).ToList();
        }

        public Association GetAssociation(Guid AssociationID)
        {
            return this.Associations.Include(l => l.Locations).Where(a => a.AssociationID == AssociationID).FirstOrDefault();
        }

        public Association GetAssociationWithPages(Guid AssociationID)
        {
            return this.Associations.Include(l => l.PageAbout).Include(l => l.PageLink).Include(l => l.PagePress).Include(l => l.PageSponsor).Include(l => l.Sponsors).Where(a => a.AssociationID == AssociationID).FirstOrDefault();
        }

        public List<AssociationListModel> GetAssociationList()
        {
            return this.Associations
                .Where(a => a.Status == AssociationStatus.Active)
                .OrderBy(a => a.Name)
                .Select(p => new AssociationListModel
                {
                    AssociationID = p.AssociationID,
                    Name = p.Name
                })
                .ToList();
        }

        public bool SaveAssociation(Association association)
        {
            if (association.AssociationID == Guid.Empty)
            { this.Entry(association).State = EntityState.Added; }
            else
            { this.Entry(association).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save person");
                return false;
            }

            return true;
        }

        public bool Save(Association association)
        {
            this.Entry(association).State = association.AssociationID == Guid.Empty ?
                            EntityState.Added :
                            EntityState.Modified;

            try
            {
                this.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save association");
                return false;
            }
        }

        public BoardModelView GetBoardView(Guid Id)
        {
            Association association = this.Associations.FirstOrDefault(a => a.AssociationID == Id);
            if (association == null) return null;

            IQueryable<NR.Models.NRMembership> ManagementList = this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == Id).AsQueryable();

            BoardModelView Management = new BoardModelView
            {
                Chairmann = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Chairman).Select(m => m.Person).FirstOrDefault(),
                Accountant = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Accountant).Select(m => m.Person).FirstOrDefault(),
                Auditor = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Auditor).Select(m => m.Person).FirstOrDefault(),
                AuditorAlternate = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.AuditorAlternate).Select(m => m.Person).FirstOrDefault(),
                BoardMembers = (from m in ManagementList where m.BoardFunction == BoardFunctionType.BoardMember select m.Person).ToList(),
                Alternate = (from m in ManagementList where m.BoardFunction == BoardFunctionType.Alternate select m.Person).ToList(),
                Association = association
            };



            return Management;
        }

        public BoardModel GetBoard(Guid Id)
        {
            Association association = this.Associations.FirstOrDefault(a => a.AssociationID == Id);
            if (association == null) return null;

            IQueryable<NR.Models.NRMembership> ManagementList = this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == Id).AsQueryable();

            BoardModel Management = new BoardModel
            {
                Chairmann = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Chairman).Select(m => m.Person.PersonID).FirstOrDefault(),
                Accountant = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Accountant).Select(m => m.Person.PersonID).FirstOrDefault(),
                Auditor = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.Auditor).Select(m => m.Person.PersonID).FirstOrDefault(),
                AuditorAlternate = ManagementList.Where(m => m.BoardFunction == BoardFunctionType.AuditorAlternate).Select(m => m.Person.PersonID).FirstOrDefault(),

                BoardMembers = (from m in ManagementList where m.BoardFunction == BoardFunctionType.BoardMember select m.Person.PersonID).ToList(),
                Alternate = (from m in ManagementList where m.BoardFunction == BoardFunctionType.Alternate select m.Person.PersonID).ToList(),
                Active = ManagementList.Where(m => (m.Type == PersonType.Active | m.Type == PersonType.Passiv) & m.AbsentDate == null).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).Select(m => m.Person).ToList(),
                All = ManagementList.Where(m => m.AbsentDate == null).OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).Select(m => m.Person).ToList(),
                AssociationID = association.AssociationID,
                AssociationName = association.Name
            };



            return Management;
        }

        public void SaveBoard(BoardModel model)
        {
            IQueryable<NR.Models.NRMembership> ManagementList = this.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == model.AssociationID).AsQueryable();

            foreach (NR.Models.NRMembership m in ManagementList)
            {
                if (m.BoardFunction != BoardFunctionType.none) m.BoardFunction = BoardFunctionType.none;
            }

            if (model.AuditorAlternate != Guid.Empty) ManagementList.Single(a => a.Person.PersonID == model.AuditorAlternate).BoardFunction = BoardFunctionType.AuditorAlternate;
            if (model.BoardMembers != null)
            {
                foreach (Guid member in model.BoardMembers)
                {
                    if (member != Guid.Empty) ManagementList.Single(a => a.Person.PersonID == member).BoardFunction = BoardFunctionType.BoardMember;
                }
            }
            if (model.Alternate != null)
            {
                foreach (Guid alternate in model.Alternate)
                {
                    if (alternate != Guid.Empty) ManagementList.Single(a => a.Person.PersonID == alternate).BoardFunction = BoardFunctionType.Alternate;
                }
            }
            ManagementList.Single(a => a.Person.PersonID == model.Accountant).BoardFunction = BoardFunctionType.Accountant;
            if (model.Auditor != Guid.Empty) ManagementList.Single(a => a.Person.PersonID == model.Auditor).BoardFunction = BoardFunctionType.Auditor;
            ManagementList.Single(a => a.Person.PersonID == model.Chairmann).BoardFunction = BoardFunctionType.Chairman;

            this.SaveChanges();
        }

        public AccessModel GetAccess(Guid AssociationID)
        {
            AccessModel result = new AccessModel();

            result.Form = this.Memberships.Include(m => m.Person)
                .Where(m => m.Association.AssociationID == AssociationID & (m.Editor | m.Planner | m.Secretary))
                .Select(p => new PersonAccess
                {
                    FunctionID = p.MembershipID,
                    FullName = p.Person.FirstName + " " + p.Person.FamilyName,
                    Planner = p.Planner,
                    Secretary = p.Secretary,
                    Editor = p.Editor
                })
                .OrderBy(p => p.FullName)
                .ToList();

            result.SelectPersons = this.Memberships.Include(m => m.Person)
                .Where(m => m.Association.AssociationID == AssociationID && m.AbsentDate == null)
                .Select(p => new PersonAccess
                {
                    FunctionID = p.MembershipID,
                    FullName = p.Person.FirstName + " " + p.Person.FamilyName,
                    Planner = p.Planner,
                    Secretary = p.Secretary,
                    Editor = p.Editor
                })
                .OrderBy(p => p.FullName)
                .ToList();



            result.AssociationID = AssociationID;

            //result.Users = context.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == AssociationID & (m.Editor | m.Planner | m.Secretary)).ToList();
            return result;
        }

        public void SaveAccess(List<PersonAccess> data)
        {
            foreach (PersonAccess m in data)
            {
                var membership = this.Memberships.Find(m.FunctionID);
                if (membership != null)
                {
                    membership.Planner = m.Planner;
                    membership.Secretary = m.Secretary;
                    membership.Editor = m.Editor;
                }
            }
            this.SaveChanges();
        }

        #endregion

        #region Locations

        public Location GetLocation(Guid ID)
        {
            return this.Locations.Include(l => l.association).Where(l => l.LocationID == ID).FirstOrDefault();
        }

        public List<Location> GetNationalLocations()
        {
            return this.Locations.Include(l => l.association).Where(l => l.association.Status == AssociationStatus.Active & l.ShowNationalMap).ToList();
        }

        public bool SaveLocation(Location location)
        {
            if (location.LocationID == Guid.Empty)
            { this.Entry(location).State = EntityState.Added; }
            else
            { this.Entry(location).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Notification

        public List<Notification> GetNotifications(Guid Reciver)
        {
            return this.NotificationRecivers.Where(R => R.Reciver.PersonID == Reciver).Select(N => N.Notification).OrderByDescending(N => N.Created).ToList();
        }

        public Notification Notify(object RegardingObject, String Message)
        {
            if (RegardingObject == null) throw new ArgumentNullException("Regarding");
            return new Notification
            {
                Message = Message,
                Type = NotType(RegardingObject),
                Link = NotLink(RegardingObject),
                Recivers = new List<NotificationReciver>()
            };
        }

        public void NotifyAddAdministration(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException("Notification");
            var Users = Roles.GetUsersInRole("Management").ToList();
            Users.AddRange(Roles.GetUsersInRole("Administrator").ToList());
            foreach (String user in Users.Distinct())
            {
                Person person = this.People.Where(p => p.UserName == user).First();
                if (person != null && !notification.Recivers.Where(r => r.Reciver.PersonID == person.PersonID).Any())
                {
                    notification.Recivers.Add(new NotificationReciver
                    {
                        Reciver = person
                    });

                }

            }
            //return notification;
        }

        public void NotifyAddPerson(Notification notification, Person person)
        {
            if (notification == null) throw new ArgumentNullException("Notification");
            if (person != null && !notification.Recivers.Where(r => r.Reciver.PersonID == person.PersonID).Any())
            {
                notification.Recivers.Add(new NotificationReciver
                {
                    Reciver = person
                });

            }

            //return notification;
        }

        public void NotifyAddBoard(Notification notification, Guid AssociationID)
        {
            //return notification; //TODO: Implement AssBoard
        }

        public void NotifyAddAssociation(Notification notification, Guid AssociationID)
        {
            if (notification == null) throw new ArgumentNullException("Notification is null");

            foreach (Person p in this.Memberships.Where(m => m.Association.AssociationID == AssociationID & m.Type == PersonType.Active & m.AbsentDate == null).Select(m => m.Person).ToList() ?? new List<Person>())
            {
                notification.Recivers.Add(new NotificationReciver
                {
                    Reciver = p
                });
            }
            //return notification; 
        }

        public void NotifyAddPlanner(Notification notification, Guid AssociationID)
        {
            if (notification == null) throw new ArgumentNullException("Notification");
            foreach (Person P in this.Memberships.Where(m => m.Association.AssociationID == AssociationID & m.Planner == true).Select(m => m.Person).ToList() ?? new List<Person>())
            {
                notification.Recivers.Add(new NotificationReciver
                {
                    Reciver = P
                });
            }

            //return notification;
        }

        public void NotifyAddTeam(Notification notification, Team team)
        {
            if (notification == null) throw new ArgumentNullException("Notification");
            foreach (Person P in team.Teammembers)
            {
                notification.Recivers.Add(new NotificationReciver
                {
                    Reciver = P
                });
            }

            //return notification;
        }

        public void NotifyAddSystem(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException("Notification");
            List<NotificationReciver> SystemPerson = new List<NotificationReciver>();
            var Users = Roles.GetUsersInRole("Management").ToList();
            Users.AddRange(Roles.GetUsersInRole("Administrator").ToList());
            foreach (String user in Users.Distinct())
            {
                Person person = this.People.Where(p => p.UserName == user).First();
                if (person != null && !notification.Recivers.Where(r => r.Reciver.PersonID == person.PersonID).Any())
                {
                    SystemPerson.Add(new NotificationReciver
                    {
                        Reciver = person
                    });
                }
            }
            //List<NotificationReciver> AssAdmin = this.Memberships.Where(M => M.Secretary == true | M.Editor == true | M.Planner == true). Select(S => S.Person).ToList().Select(S => new NotificationReciver { Reciver = S }).ToList();
            SystemPerson.AddRange(this.Memberships.Where(M => M.Secretary == true | M.Editor == true | M.Planner == true).Select(S => S.Person).ToList().Select(S => new NotificationReciver { Reciver = S }).ToList());
            notification.Recivers = SystemPerson;

        }

        public Notification NotificationRead(Guid Notification, Guid Person)
        {
            NotificationReciver Not = this.NotificationRecivers.Include(N => N.Notification).Where(n => n.Notification.NotificationID == Notification & n.Reciver.PersonID == Person).FirstOrDefault();
            if (Not != null && Not.Read == false)
            {
                Not.Read = true;
                this.SaveChanges();
            }
            return Not.Notification;
        }

        public void NotifySave(Notification notification)
        {
            if (notification.Recivers.Any())
            {
                notification.Trim();
                notification.Recivers = notification.Recivers.Distinct().ToList();
                this.Entry(notification).State = EntityState.Added;
            }
            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Notify save");

            }
            CurrentProfile.Clear();
        }

        public void NotificationClear(Guid Reciver)
        {
            var Notifications = this.NotificationRecivers.Where(R => R.Reciver.PersonID == Reciver & R.Read == false);

            foreach (NotificationReciver N in Notifications)
            {
                N.Read = true;
            }

            this.SaveChanges();
        }

        #endregion

        #region Messages

        public List<Message> GetUserMessages(Guid Id)
        {

            return (from M in this.Messages
                    where (M.Recivers.Any(R => R.ReciverID == Id))
                    //let returnAllMessages = M.Recivers.Any(R => R.ReciverID == Id)
                    orderby M.Created descending
                    select new { Msg = M, Recivers = M.Recivers.Where(R => R.ReciverID == Id) }
                     ).ToList().Select(M => M.Msg).ToList();

            //this.MessageRecivers.     Where(R => R.ReciverID == Id).Select(M => M.Message).OrderByDescending(M => M.Created).ToList();
            //return this.MessageRecivers.Where(R => R.ReciverID == Id).Select(M => M.Message).Include(M => M.Recivers).OrderByDescending(M => M.Created).ToList();
        }

        public List<Message> GetUserUnreadMessages(Guid Id)
        {
            return this.MessageRecivers.Where(R => R.ReciverID == Id && R.Read == false).Select(M => M.Message).Include(M => M.Recivers).OrderByDescending(M => M.Created).ToList();
        }

        public List<Message> GetUserUnreadMessages(Guid Id, out int count)
        {
            count = this.MessageRecivers.Where(R => R.ReciverID == Id && R.Read == false).Select(M => M.Message).Include(M => M.Sender).Count();
            return this.MessageRecivers.Where(R => R.ReciverID == Id && R.Read == false).Select(M => M.Message).Include(M => M.Sender).OrderBy(M => M.Created).Take(5).ToList();
        }

        public List<Message> GetUserSentMessages(Guid Id)
        {
            return this.Messages.Where(M => M.SenderID == Id).Include(M => M.Recivers).ToList();
        }

        public Message GetMessage(Guid Id, Guid PersonID)
        {

            var tmp = this.Messages.Where(m => m.MessageID == Id).Include(m => m.Recivers).ToList();

            Message Msg = this.Messages.Where(m => m.MessageID == Id).Include(m => m.Recivers).FirstOrDefault();                    //.Select().Single(m => m.MessageID == Id);


            MessageReciver MR = Msg.Recivers.First(R => R.ReciverID == PersonID);

            if (MR != null)
            {
                MR.Read = true;
                this.Entry(MR).State = EntityState.Modified;

                try
                {
                    this.SaveChanges();
                }
                catch (Exception e)
                {
                    LogFile.Write(e, "Get Message");

                }
            }
            return Msg;

        }

        public void MessageSend(Message message)
        {
            if (!message.Recivers.Any()) throw new ArgumentNullException("Message.Recivers");
            if (message.SenderID == Guid.Empty && message.Sender == null) throw new ArgumentNullException("Message.Sender");

            this.Entry(message).State = EntityState.Added;
            //context.Messages.Attach(message);

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "MEssage send");
               
            }

        }

        public void DeleteMessage(Guid MessageID, Guid PersonID)
        {
            MessageReciver MR = this.MessageRecivers.Where(R => R.ReciverID == PersonID && R.Message.MessageID == MessageID).FirstOrDefault();

            if (MR != null)
            {
                if (this.MessageRecivers.Where(R => R.Message.MessageID == MessageID).Count() <= 1) this.Entry(MR.Message).State = EntityState.Deleted;
                this.Entry(MR).State = EntityState.Deleted;
                try
                {
                    this.SaveChanges();
                }
                catch (Exception e)
                {
                    LogFile.Write(e, "Delete Message");

                }
            }
        }

        #endregion

        #region Planning

        public List<MessageReciver> ProcessMessageNotification()
        {
            return this.MessageRecivers.Include(R => R.Message.Sender).Where(M => M.Sent == false).ToList();
        }

        public void MarkSent(MessageReciver Reciver)
        {
            Reciver.Sent = true;
            this.Entry(Reciver).State = EntityState.Modified;

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Mark send: MessageReciverID(" + Reciver.MessageReciverID +")");
            }
        }

        #endregion

        #region Folder
        public IList<Folder> GetAssociationFolders(Guid Id)
        {
            return this.Folders.Where(F => F.Association.AssociationID == Id).ToList();
        }

        public Folder GetFolder(Guid Id)
        {
            return this.Folders.Include(F => F.Teams.Select(T => T.Teammembers)).Where(T => T.FolderID == Id).FirstOrDefault();
        }

        public Team GetTeam(Guid Id)
        {
            return this.Teams.Include(T => T.Teammembers).Include(T => T.Association).Include(T => T.Folder).Where(T => T.TeamID == Id).FirstOrDefault();
        }

        public List<Team> GetAssociationPlan(Guid id)
        {

            DateTime DateCut = DateTime.Now.AddDays(-14).Date;
            return this.Teams.Include(T => T.Teammembers).Where(T => T.Start > DateCut & T.Folder.Status == FolderStatus.Published & T.Folder.Association.AssociationID == id).OrderBy(T => T.Start).ToList();
        }

        public List<Team> GetMyTeams(Guid id)
        {
            DateTime Deadline = DateTime.Now.AddDays(-14);
            return this.Teams.Include(T => T.Teammembers).Where(T => T.Start > Deadline && T.Folder.Status == FolderStatus.Published && T.Teammembers.Any(P => P.PersonID == id)).OrderBy(T => T.Start).ToList();
        }

        public List<Team> GetOpenTeams(Guid id, Guid Association, int minTeams)
        {
            DateTime Deadline = DateTime.Now.AddDays(1);
            return this.Teams.Include(T => T.Teammembers)
                .Where(T => T.Start > Deadline && T.Folder.Status == FolderStatus.Published && T.Association.AssociationID == Association &&
                    ((!T.Trial & T.Teammembers.Count < minTeams) | (T.Trial & T.Teammembers.Count < minTeams - 1)) &&
                    T.Teammembers.Any(P => P.PersonID != id))
                .OrderBy(T => T.Start).ToList();
        }

        public bool SaveFolder(Folder Folder)
        {
            if (Folder.FolderID == Guid.Empty)
            { this.Entry(Folder).State = EntityState.Added; }
            else
            { this.Entry(Folder).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save Folder");
                return false;
            }

            return true;
        }

        public bool DeleteFolder(Guid Id)
        {
            Folder DeleteFolder = this.Folders.Where(T => T.FolderID == Id).FirstOrDefault();
            if (DeleteFolder != null && DeleteFolder.Status == FolderStatus.New)
            {
                this.Entry(DeleteFolder).State = EntityState.Deleted;
                try
                {
                    this.SaveChanges();
                }
                catch
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public bool SaveTeam(Team Team)
        {
            if (Team.TeamID == Guid.Empty) throw new ArgumentNullException("TeamID");

            this.Entry(Team).State = EntityState.Modified;

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save Team");
                return false;
            }

            return true;
        }

        public bool Delete(Team Team)
        {
            //Folder DeleteFolder = this.Folders.Where(T => T.FolderID == Id).FirstOrDefault();
            if (Team != null && Team.Folder != null && Team.Folder.Status == FolderStatus.New)
            {
                this.Entry(Team).State = EntityState.Deleted;
                try
                {
                    this.SaveChanges();
                }
                catch (Exception e)
                {
                    LogFile.Write(e, "Delete team");
                    return false;
                }

                return true;
            }
            return false;
        }

        #endregion

        #region FolderUserAnswer

        public List<FolderUserAnswer> GetFolderUserAnswers(Guid FolderId)
        {
            return this.FolderUserAnswers.Include(F => F.Answers).Where(T => T.FolderID == FolderId).ToList();
        }

        public FolderUserAnswer GetFolderUserAnswer(Guid PersonId, Guid FolderId)
        {
            return this.FolderUserAnswers.Include(F => F.Answers).Where(T => T.FolderID == FolderId & T.PersonID == PersonId).FirstOrDefault();
        }

        public bool SaveFolderUserAnswer(FolderUserAnswer Answer)
        {
            if (Answer.FolderID == Guid.Empty) throw new ArgumentNullException("FolderID");
            if (Answer.PersonID == Guid.Empty) throw new ArgumentNullException("PersonID");

            if (Answer.FolderUserAnswerID == Guid.Empty)
            { this.Entry(Answer).State = EntityState.Added; }
            else
            { this.Entry(Answer).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SaveFolderUserAnswer Exception: \"" + e.Message + "\"");

                return false;
            }

            return true;
        }

        #endregion

        #region Event

        public List<Event> GetFrontEvents()
        {

            List<Event> FrontEvents = this.Events
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == CurrentProfile.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == CurrentProfile.AssociationID)) & e.Finish > DateTime.Now).OrderBy(e => e.Start)
                .Take(3).ToList();
            return FrontEvents;
        }

        public List<Event> GetUserEvents(int? index)
        {
            List<Event> UserEvents = this.Events
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == CurrentProfile.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == CurrentProfile.AssociationID)) & e.Finish > DateTime.Now).OrderBy(e => e.Start)
               .Skip(10 * index ?? 0).Take(10).ToList();

            return UserEvents;
        }

        public List<Event> GetEvents()
        {
            return this.Events.OrderBy(e => e.Start).ToList();
        }

        public Event GetEventItem(Guid id)
        {
            return this.Events.Where(n => n.EventID == id).FirstOrDefault();
        }

        public bool Save(Event Evt)
        {
            if (Evt.EventID == Guid.Empty)
            { this.Entry(Evt).State = EntityState.Added; }
            else
            { this.Entry(Evt).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.Write(e, "Save Event");
                return false;
            }

            return true;
        }

        #endregion

        #region News
        public List<News> GetFrontNews()
        {
            List<News> FrontNews = this.News
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == CurrentProfile.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == CurrentProfile.AssociationID)) & (e.Depublish == null || e.Depublish > DateTime.Now) & (e.Publish < DateTime.Now))
                .OrderByDescending(e => e.Publish).Take(10).ToList();

            return FrontNews;
        }

        public List<News> GetUserNews(int? index)
        {
            List<News> UserNews = this.News
                .Where(e => (e.Distribution == LevelType.National | (e.Distribution == LevelType.Network & e.DistributionLink == CurrentProfile.NetworkID) |
                (e.Distribution == LevelType.Local & e.DistributionLink == CurrentProfile.AssociationID)) & (e.Depublish == null | e.Depublish > DateTime.Now) & (e.Publish < DateTime.Now))
                .ToList();

            UserNews = UserNews.OrderByDescending(e => e.Publish).ToList();

            UserNews = UserNews.Skip(10 * index ?? 0).Take(10).ToList();

            return UserNews;
        }

        public List<News> GetNews()
        {
            return this.News.OrderByDescending(e => e.Publish).ToList();
        }

        public News GetNewsItem(Guid id)
        {
            return this.News.Where(n => n.NewsId == id).FirstOrDefault();
        }

        public News GetNewsItemCount(Guid id)
        {
            News news = this.News.Where(n => n.NewsId == id).FirstOrDefault();
            news.Views += 1;
            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {

            }

            return news;
        }

        public bool Save(News news)
        {
            if (news.NewsId == Guid.Empty)
            { this.Entry(news).State = EntityState.Added; }
            else
            { this.Entry(news).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("SaveFolderUserAnswer Exception: \"" + e.Message + "\"");

                throw;
#endif
                return false;
            }

            return true;
        }

        public void DeleteNews(Guid NewsID)
        {
            News news = this.News.Find(NewsID);

            if (news != null)
            {
                this.Entry(news).State = EntityState.Deleted;
                try
                {
                    this.SaveChanges();
                }
                catch
                {

                }
            }
        }

        #endregion

        #region Wiki

        public List<Wiki> GetWikis()
        {

            return this.Wikis.ToList();
        }

        public Wiki GetWikisItem(Guid id)
        {
            return this.Wikis.Where(n => n.WikiID == id).FirstOrDefault();
        }

        public List<WikiWord> GetWikiWords()
        {
            return this.WikiWords.Include(w => w.Wiki).ToList();
        }

        public bool RemoveWikiWords(Wiki wiki)
        {


            //this.Entry(wiki).State = EntityState.Deleted;
            if (wiki.Words != null && wiki.Words.Any())
            {
                foreach (WikiWord W in wiki.Words)
                {
                    this.Entry(W).State = EntityState.Deleted;
                }
          
               
                try
                {
                    this.SaveChanges();

                }
                catch (Exception e)
                {


                    return false;
                }

            } 

            return true;
        }


        public bool Save(Wiki wiki)
        {
            if (wiki.WikiID == Guid.Empty)
            { this.Entry(wiki).State = EntityState.Added; }
            else
            {
                this.Entry(wiki).State = EntityState.Modified;
            }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {


                return false;
            }

            return true;
        }

        #endregion

        #region Causes

        public List<Cause> GetCauses()
        {

            return this.Causes.ToList();
        }

        public Cause GetCauseItem(Guid id)
        {
            return this.Causes.Where(n => n.CauseID == id).FirstOrDefault();
        }

        public bool Save(Cause cause)
        {
            if (cause.CauseID == Guid.Empty)
            { this.Entry(cause).State = EntityState.Added; }
            else
            { this.Entry(cause).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SaveFolderUserAnswer Exception: \"" + e.Message + "\"");

                return false;
            }

            return true;
        }

        #endregion

        #region SI

        public SIViewModel GetSIViewModel(Guid id)
        {
            SIViewModel SIInfor = new SIViewModel
            {
                Associations = this.Associations.Include(a => a.Network).Where(a => a.SeniorInstructorID == id).ToList()
            };

            return SIInfor;
        }

        #endregion

        #region Content

        public Content GetContent(Guid ID)
        {

            return this.Content.Where(c => c.ContentID == ID).FirstOrDefault();
        }

        public bool Save(Content content)
        {
            if (content.ContentID == Guid.Empty)
            { this.Entry(content).State = EntityState.Added; }
            else
            { this.Entry(content).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {

                return false;
            }

            return true;
        }

        #endregion

        #region Files (AFile)

        public List<AFile> GetFiles()
        {

            return this.Files.OrderBy(f => f.Title).ToList();
        }

        public AFile GetFile(Guid ID)
        {
            return this.Files.Where(f => f.FileID == ID).FirstOrDefault();
        }

        public bool Save(AFile file)
        {
            if (file.FileID == Guid.Empty)
            { this.Entry(file).State = EntityState.Added; }
            else
            { this.Entry(file).State = EntityState.Modified; }

            try
            {
                this.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SaveFolderUserAnswer Exception: \"" + e.Message + "\"");

                return false;
            }

            return true;
        }

        #endregion

        #region TextMessages

        public void NewTextMessage(ITextMessage Message, Guid AssociationId)
        {
            TextMessage textMessage = new TextMessage
                {
                    Sender = Message.From == null ? Guid.Empty : Message.From.PersonID,
                    Reciver = Message.Recipient.Count > 1 ? Guid.Empty : Message.Recipient.First().PersonID,
                    Message = Message.Message,
                    AssociationID = AssociationId,
                    PlanedSending = Message.DeliveryTime > DateTime.Now ? Message.DeliveryTime : DateTime.Now

                };

            this.Entry(textMessage).State = EntityState.Added;

            try
            {
                this.SaveChanges();
                Message.TextId = textMessage.TextId.ToString();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SaveTextMessage Exception: \"" + e.Message + "\"");


            }


        }

        public List<TextMessage> GetMessages(Guid? Id)
        {

            return null;
        }

        public void UpdateStatusMessage(int ID, SMSStatus status)
        {
            try
            {
                TextMessage txtMsg = this.Texts.Find(ID);
                txtMsg.TextStatus = status;


                this.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SaveTextMessage Exception: \"" + e.Message + "\"");


            }
        }


        #endregion

        #region HelperFunctions

        public static NotificationType NotType<T>(T parm)
        {
            if (parm is Person) return NotificationType.Person;
            if (parm is Association) return NotificationType.Association;
            if (parm is Folder) return NotificationType.Folder;
            if (parm is Event) return NotificationType.Event;
            if (parm is Wiki) return NotificationType.Wiki;
            if (parm is Team) return NotificationType.Team;
            if (parm is Content) return NotificationType.Content;
            if (parm is AssociationNoteEditModel) return NotificationType.AssociationNote;


            if (parm is NewVersion) return NotificationType.NewVersion;
            return NotificationType.Unknown;
        }

        public static Guid NotLink<T>(T parm)
        {
            if (parm is Person) return (Guid)parm.GetType().GetProperty("PersonID").GetValue(parm, null);
            if (parm is Association) return (Guid)parm.GetType().GetProperty("AssociationID").GetValue(parm, null);
            if (parm is Folder) return (Guid)parm.GetType().GetProperty("FolderID").GetValue(parm, null);
            if (parm is Team) return (Guid)parm.GetType().GetProperty("TeamID").GetValue(parm, null);
            if (parm is Event) return (Guid)parm.GetType().GetProperty("EventID").GetValue(parm, null);
            if (parm is Wiki) return (Guid)parm.GetType().GetProperty("WikiID").GetValue(parm, null);
            if (parm is Content) return (Guid)parm.GetType().GetProperty("ContentID").GetValue(parm, null);
            if (parm is AssociationNoteEditModel) return (Guid)parm.GetType().GetProperty("AssociationID").GetValue(parm, null);
            if (parm is NewVersion) return Guid.Empty;
            return Guid.Empty;
        }

        #endregion


       


        /// <summary>
        /// COnverts a int to a Guid
        /// </summary>
        /// <param name="value">Int</param>
        /// <returns>Guid</returns>
        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}