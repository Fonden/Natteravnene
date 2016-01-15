/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/


using NR.Infrastructure;
using NR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace NR.Entity
{
    public class NRDbContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<Association> Associations { get; set; }
        public DbSet<NRMembership> Memberships { get; set; } //TODO: How to only expose get par of this public
        public DbSet<Person> People { get; set; }   //TODO: How to only expose get par of this public
        public DbSet<Location> Locations { get; set; }

        public DbSet<Notification> Notifications { get; set; }  //TODO: How to only expose get par of this public
        public DbSet<NotificationReciver> NotificationRecivers { get; set; }  //TODO: How to only expose get par of this public
        public DbSet<Message> Messages { get; set; } //TODO: How to only expose get par of this public
        public DbSet<MessageReciver> MessageRecivers { get; set; }
        public DbSet<MessageGroup> MessageGroups { get; set; }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<FolderUserAnswer> FolderUserAnswers { get; set; }

        public DbSet<Content> Content { get; set; }
        public DbSet<Minutes> Minutes { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Wiki> Wikis { get; set; }
        public DbSet<WikiWord> WikiWords { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Cause> Causes { get; set; }

        public DbSet<AFile> Files { get; set; }

        public DbSet<TextMessage> Texts { get; set; }

        public DbSet<SignupList> SignupLists { get; set; }
        public DbSet<Signup> Signups { get; set; }
        public DbSet<Lead> Leads { get; set; }

        public override int SaveChanges()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    return base.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    LogFile.WriteError(exceptionMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (DbUpdateException ex)
                {
                    LogFile.Write(ex, "** DbUpdateException");
                    throw;
                }
                catch (Exception ex)
                {
                    LogFile.Write(ex, "** Exception SaveChange");
                }
            } while (saveFailed);
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Person>()
               .HasMany(P => P.Memberships)
               .WithRequired(M => M.Person)
               .WillCascadeOnDelete();

            modelBuilder.Entity<Association>()
               .HasMany(A => A.Memberships)
               .WithRequired(M => M.Association)
               .WillCascadeOnDelete();

            modelBuilder.Entity<Association>()
              .HasMany(A => A.Locations)
              .WithRequired(L => L.association)
              .WillCascadeOnDelete();

            //modelBuilder.Entity<Message>()
            //   .HasMany(M => M.Recivers)
            //   .WithRequired(R => R.Message)
            //   .WillCascadeOnDelete();

            modelBuilder.Entity<Cause>()
             .HasMany(C => C.Partisipants)
             .WithRequired(P => P.Cause)
             .WillCascadeOnDelete();

            modelBuilder.Entity<Notification>()
              .HasMany(N => N.Recivers)
              .WithRequired(R => R.Notification)
              .WillCascadeOnDelete();

            modelBuilder.Entity<Cause>()
              .HasMany(C => C.Partisipants)
              .WithRequired(P => P.Cause)
              .WillCascadeOnDelete();

            modelBuilder.Entity<Wiki>()
              .HasMany(C => C.Words)
              .WithRequired(P => P.Wiki)
              .WillCascadeOnDelete();

            //modelBuilder.Entity<TeamUserAnswer>()
            //    .HasRequired(m => m.Team)
            ////    .HasOptional(m => m.Team) 
            ////    .WithOptionalPrincipal() 
            //     .WithRequiredPrincipal()
            //    .WillCascadeOnDelete(false);




            //modelBuilder.Properties()

            /*modelBuilder.Entity<Person>()
                .Property(t => t.Username)
                .HasColumnAnnotation("Index", new IndexAnnotation(
            new IndexAttribute("IX_UsernameUniq", 1) { IsUnique = true }));*/

        }

        public NRDbContext()
            : base("Repository")
        {



        }


    }
}