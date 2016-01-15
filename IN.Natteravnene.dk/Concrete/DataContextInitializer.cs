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
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;


namespace NR.Entity
{




    public class DataContextInitializer : CreateDatabaseIfNotExists<Repository>
#if ALWAYS
 //DropCreateDatabaseAlways<Repository>
#else
    //DropCreateDatabaseIfModelChanges<Repository>
#endif
    {


        private string seedConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Ravnetur;Integrated Security=True";

        protected override void Seed(Repository context)
        {
            Person UserPerson = new Person();

            //Index on UserID for person to support WebSecurity and maintain PersonID as GUID
            context.Database.ExecuteSqlCommand("ALTER TABLE People ADD CONSTRAINT Persons_UserID UNIQUE (UserID);");
            //"ALTER TABLE [Personer] ADD CONSTRAINT DF_Personer_PersonID_Default DEFAULT newid() FOR [PersonID];");

            //Create constraint for Created and Lastchanged on Personer table
            CreateConstraint(context, "People", "PersonID");

            if (!WebSecurity.Initialized) WebSecurity.InitializeDatabaseConnection("Repository", "People", "UserID", "UserName", autoCreateTables: true);
            if (!Roles.RoleExists("Administrator")) Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Management")) Roles.CreateRole("Management");

            if (!WebSecurity.UserExists("ADMIN"))
            {
                UserPerson = new Person
                {
                    PersonID = new Guid(),
                    UserName = "ADMIN",
                    FirstName = "Admin",
                    FamilyName = "Adminsen",
                    Country = Country.DK,
                    Created = DateTime.Now,
                    Lastchanged = DateTime.Now
                };
                context.People.Add(UserPerson);

                context.SaveChanges();
                WebSecurity.CreateAccount("ADMIN", "N98ravne");
                if (!WebSecurity.ConfirmAccount("ADMIN"))
                {
                    //WebSecurity.CreateAccount("DEMO", "123456");
                   Roles.AddUserToRole("ADMIN", "Administrator");
                   Roles.AddUserToRole("ADMIN", "Management");
                }
            }
            
            #region Netværk Seedning
            CreateConstraint(context, "Networks", "NetworkID");
            foreach (Network netvaerk in new List<Network>
            {
            new Network {NetworkNumber = 1, NetworkName = "Vendsyssel/Hanherred" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 2, NetworkName = "Himmerland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 3, NetworkName = "Midt-/Vestjylland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 4, NetworkName = "København Sydkysten" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 5, NetworkName = "Østjylland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 6, NetworkName = "Vestjylland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 7, NetworkName = "Sønderjylland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 8, NetworkName = "Trekantsområdet" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 9, NetworkName = "Fyn/Langeland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 10, NetworkName = "Nordsjælland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 11, NetworkName = "Vestsjælland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 12, NetworkName = "Østsjælland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 13, NetworkName = "Sydsjælland/Sydhavet" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 14, NetworkName = "Hovedstaden" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 15, NetworkName = "København Vestegnen" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 16, NetworkName = "København Sydkysten" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 17, NetworkName = "Bornholm" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 18, NetworkName = "Færøerne" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 19, NetworkName = "Grønland" , Created = DateTime.Now, Lastchanged = DateTime.Now},
            new Network {NetworkNumber = 99, NetworkName = "Ikke defineret" , Created = DateTime.Now, Lastchanged = DateTime.Now, NetworkNotToShow = true}
            })
            {
                context.Networks.Add(netvaerk);
                //context.Entry(netvaerk).State = System.Data.Entity.EntityState.Modified;
            }
            context.SaveChanges();
            #endregion

            #region Forening Seedning

            CreateConstraint(context, "Associations", "AssociationID");

            #endregion

            #region Messages Seedning

            CreateDefault(context, "Messages");

            #endregion

            #region Notification Seedning

            CreateDefault(context, "Notifications");

            #endregion
 
            #region Lokations Seedning

            CreateConstraint(context, "Locations", "LocationID");

            #endregion

            #region Folder Seedning

            CreateConstraint(context, "Folders", "FolderID");

            CreateConstraint(context, "Teams", "TeamID");

            CreateConstraint(context, "FolderUserAnswers", "FolderUserAnswerID");

            #endregion

            #region Event Seedning

            CreateConstraint(context, "Events", "EventID");

                
            #endregion

            #region News Seedning

            CreateConstraint(context, "News", "NewsId");

           


            #endregion

            #region Wiki Seedning

            CreateConstraint(context, "Wikis", "WikiID");

            #endregion

            #region Cause Seedning

            CreateConstraint(context, "Causes", "CauseID");

            #endregion

            #region Files Seedning

            CreateConstraint(context, "Files", "FileID");

            #endregion

            #region Lists Seedning

            CreateConstraint(context, "Lists", "ListID");

            #endregion

            #region Content Seedning

            CreateConstraint(context, "Contents", "ContentID");

            #endregion

            #region Textmessages

            CreateDefault(context, "TextMessages");

            #endregion

            #region SignupList

            CreateDefault(context, "SignupLists");
            CreateDefault(context, "Signups");

            #endregion

            context.Events.Add(new Event  
                {
                    Headline = "Landsmøde 2015", 
                    Distribution =  LevelType.National,  
                    Source = LevelType.National , 
                    AuthorID = UserPerson.PersonID, 
                    Start = DateTime.Parse("2015-09-26 10:00"), 
                    Finish =  DateTime.Parse("2015-09-26 17:00"), 
                    Description = "Natteravnenes årlige landsmøde, hvor foreningerne mødes.", 
                    Location = "Hotel Nyborg Strand" 
                });



//            context.News.Add(new News
//            {
//                Headline = "Natteravnene er kommet til Ballerup",
//                Distribution = LevelType.National,
//                Source = LevelType.National,
//                AuthorID = UserPerson.PersonID,
//                Publish = DateTime.Parse("2014-11-20 10:00"),
//                Teaser = "”Vi er meget glade for at vi har fået mulighed for at være en del af etableringen, og vi ser frem til samarbejdet med den nye forening i Netværket” slutter Henrik Skude på vegne af SeniorTeamet og netværket.",
//                Content = @"<p>På en Stiftende generalforsamling den 18. november blev Natteravnene i Ballerup etableret som forening nr. 271</p>
//                            <p>”For år tilbage havde vi en lokalforening af Natteravne netop i Ballerup, men den har ikke været aktiv i flere år, så det glæder os meget at der nu igen er kommet frivillige nok til en ny forening så der kan skabes tryghed og omsorg blandt Ballerups børn og unge” fortæller Bente Gimlinge, konsulent hos Natteravnenes Landssekretariat.</p>
//                            <p>Den 1. september blev der afholdt formøde i beboerforeningen Hedeparken-Magleparkens lokaler. Initiativtagere til mødet var de to boligsociale medarbejdere Maj-Brit Jæger og Mikala Rossing, der ønskede mere tryghed i bebyggelsen.  I mødet deltog også to Natteravne fra Skovlunde, formand Kenneth Larsen samt Astrid Busk Sørensen, to repræsentanter for SSP og Seniorinstruktørerne Jørgen Andersen fra Rødovre og Henrik Skude fra Albertslund på vegne af Center For Socialt Ansvar og Netværk 15, samt et par beboerrepræsentanter.</p>
//                            <p>Den 28. oktober blev der afholdt et velbesøgt Borgermøde, hvor Ballerups borgmester Jesper Würtzen, Politiet og SSP anbefalede Natteravnene, hvorefter Lone Glad Seniorinstruktør i Albertslund og Henrik Skude først viste filmen ”Jeg ved at du er der” og derefter gennemførte en præsentation af konceptet. På mødet deltog også Natteravne fra Skovlunde. Desuden deltog Mariann Borregaard Seniorinstruktør fra Stenløse også. På aftenen skrev omkring 20 personer sig på en interesseliste.</p>
//                            <p>Den 11. og 18. november blev der afholdt to kursusaftener, hvor instruktørerne gennemgik Natteravnekonceptet, og på den sidste kursusaften deltog Astrid Busk Sørensen fra bestyrelsen i Skovlunde, som repræsentant for den nye forenings ”fadderforening”. Hermed er der lagt op til et samarbejde imellem de to foreninger i Ballerup Kommune. Natteravnene i Skovlunde donerede 200 kr. til den nye forening – så bestyrelsen kunne købe kager på sine kommende bestyrelsesmøder. Flot gestus..!</p>
//                            <p>På den stiftende generalforsamling, hvor der deltog 17 frivillige, blev Bent Nielsen valgt som formand. Herudover blev der valgt fire bestyrelsesmedlemmer, to suppleanter, en revisor og en revisorsuppleant.</p>
//                            <p>”Vi er meget glade for at vi har fået mulighed for at være en del af etableringen, og vi ser frem til samarbejdet med den nye forening i Netværket” slutter Henrik Skude på vegne af SeniorTeamet og netværket.</p>
//                            <p>Skulle du have lyst til at blive en del af Natteravnene i Ballerup eller bare komme med på en uforpligtende prøvetur, så kontakt Sekretariatet på tlf. 70 12 12 99 eller pr. mail på info@natteravnene.dk</p>"
//            });
 
//            context.News.Add(new News
//            {
//                Headline = "Arriva Danmark støtter igen Natteravnene",
//                Distribution = LevelType.National,
//                Source = LevelType.National,
//                AuthorID = UserPerson.PersonID,
//                Publish = DateTime.Parse("2014-11-19 10:00"),
//                Teaser = "I år har Arriva besluttet, at de medarbejdere, der ønsker at støtte en frivillig organisation i stedet for at få en julegave, kan støtte Natteravnene.",
//                Content = @"<p>I år har Arriva besluttet, at de medarbejdere, der ønsker at støtte en frivillig organisation i stedet for at få en julegave, kan støtte Natteravnene.</p>
//                            <p>Samarbejdet mellem Arriva og Natteravnene går mange år tilbage. Samarbejdet har bl.a. betydet, at Natteravnene kan køre gratis med tog og busser, og dermed følge rundt med de unge, samtidig med at trygheden for personale og passagerer er steget. Sidste år gennemførte man sammen hvervekampagner for flere Natteravne i Arrivas personaleblad, hvor en del ansatte både i Øst og Vest meldte sig som frivillige. Samtidig blev der reklameret for Natteravnene på Arrivas TV-skærme, bl.a. gennem små film, som var optaget i toget til Esbjerg.</p>
//                            <p>I år gik én af Arrivas interne internationale priser til Arne Ladefoged, for hans frivillige indsats som Natteravn i Ribe. Og året afsluttes med en ny understregning af samarbejdet, idet Arriva har besluttet, at de medarbejdere, der ønsker at donere deres julegave til et velgørende formål, kan donere den til Natteravnene.</p>
//                            <p>”Det er en smuk gestus fra Arrivas ledelse og medarbejdere”, siger Erik Thorsted, sekretariatschef og stifter af Natteravnene.</p>"
//            });

//            context.News.Add(new News
//            {
//                Headline = "Foreningernes Dag afholdt i Nuuk",
//                Distribution = LevelType.National,
//                Source = LevelType.National,
//                AuthorID = UserPerson.PersonID,
//                Publish = DateTime.Parse("2014-11-17 10:00"),
//                Teaser = "\"Det var andet år foreningen deltog i Foreningernes Dag. Børnene fik en ballon, mens vi fortalte deres forældre om Natteravnene\" fortæller Niels Nielsen som er formand for den lokale forening.",
//                Content = @"<p>Som der efterhånden er tradition for, var Natteravnene fra Nuuk også repræsenteret ved Foreningernes Dag i år i Inussivik.</p>
//                            <p>'Det var andet år foreningen deltog i Foreningernes Dag. Børnene fik en ballon, mens vi fortalte deres forældre om Natteravnene' fortæller Niels Nielsen som er formand for den lokale forening.</p>
//                            <p>En ny Natteravn tilmeldte sig. Hun havde gennem længere tid tænkt på, at gøre en forskel og ville gerne gøre en indsat for børn og de unge i Nuuk og Nuussuaq. Har du også lyst til at være en støtte de unge kan læne sig op ad, så kontakt Natteravnene i Nuuk på mail. nuuk@natteravnene.dk for en uforpligtende prøvevandring.</p>"
//            });

//            context.News.Add(new News
//            {
//                Headline = "TAK Natteravne fordi I er der",
//                Distribution = LevelType.National,
//                Source = LevelType.National,
//                AuthorID = UserPerson.PersonID,
//                Publish = DateTime.Parse("2014-11-17 10:00"),
//                Teaser = "Anna fortæller, at hun ikke havde fået nok at spise før hun begynde at drikke, så det slog meget hårdt ned da hun nåede ind til byen. - så hermed en opfordring til alle jer unge der ude, husk at passe på jer selv både før og efter I går i byen.",
//                Content = @"<p>Dette var overskriften på en mail vi modtog fra 17 årige Anna.</p>
//                            <p>Hej Natteravnene.<br />Jeg vil gerne sige mange tak for jeres indsats rundt omkring i landet (især i Næstved) I går aftes stod jeg i den situation at jeg fik det rigtig dårligt, og I stod klar lige med det samme og hjalp mig. Der blev ringet efter min far og i dag er jeg rigtig glad for, at I var der til at gøre min aften sikker, da jeg ikke selv var i stand til det.<br /> 
//                            <br />Mange tak.<br />Mvh Anna</p>
//                            <p>Anna fortæller, at hun ikke havde fået nok at spise før hun begynde at drikke, så det slog meget hårdt ned da hun nåede ind til byen. - så hermed en opfordring til alle jer unge der ude, husk at passe på jer selv både før og efter I går i byen.</p>
//                            <p>Tusinde tak til Anna for de søde ord og roser, det er det der gør Natteravnearbejdet så meget værd.</p>"
//            });

            context.Wikis.Add(new Wiki
            {
                Title = "Kassebeholdning",
                Words = new List<WikiWord> {
                    new WikiWord {Word = "Kassebeholdning"}
                },
                Body = @"<p>Foreningen bør ikke ligge inde med kontantbeholdning og dermed en kasse.</p>
                        <p>Foreningens beholdning bør være tilgængelig via foreningens VISA/Dankort, knyttet op på en netbank konto, for at sikre bedst mulig dokumentation for foreningens indtægter og udgifter.</p>"

            });

            context.Wikis.Add(new Wiki
            {
                Title = "Aldersgrænse",
                Words = new List<WikiWord> {
                    new WikiWord {Word = "Aldersgrænse"},
                    new WikiWord {Word = "Alder"}
                },
                Body = @"<p>Natteravnene har en vejledende aldersgrænse fra 20 år. Men livserfaring og modenhed er under alle omstændigheder det vigtigste kriterium for at være frivillig, og seriøst kunne fremstå som ”forældrenes forlængede arm i det offentlige rum”.</p> 
                         <p>Holdet kan glimrende tage et modent, yngre menneske med - blot man samtidig sikrer, at der er nogle ældre med. Rene unge-hold er således ikke en del af konceptet.</p>"

            });


            context.Wikis.Add(new Wiki
            {
                Title = "\"Medlemmer\"",
                Words = new List<WikiWord> {
                    new WikiWord {Word = "Medlemmer"}
                },
                Body = @"<p>Da medlemskab normalt forbindes med, at man skal betale et kontingent, og har en form for systematisk mødepligt og lignende, benyttes begrebet ”medlem” kun i Natteravneregi i forbindelse med styregruppe- og bestyrelsesmedlemmer.</p>
                         <p>I Natteravnene er alle frivillige, på nær medarbejderne i Landssekretariatet, der er ansat af FSA.</p>
                         <p>En person kan godt være frivillig i mere end én Natteravneforening, men skal oplyse dette til ledelserne.</p>"

            });


            context.Wikis.Add(new Wiki
            {
                Title = "Spilleregler",
                Words = new List<WikiWord> {
                    new WikiWord {Word = "Spilleregler"},
                    new WikiWord {Word = "Regler"}
                },
                Body = @"<p>Udover <i>De 5 Gyldne Regler</i> er der også nogle <i>Spilleregler</i>, som er en del af den samarbejdsaftale (se samarbejdsaftalen), der skal være mellem FSA og de lokale foreninger.</p> 
                        <p>Spillereglerne er nogle generelle og praktiske grundregler - en slags politikker - der gælder for alle foreninger. </p>"

            });

            Content FiveGyldne =  new Content
                {
                    Title = "De 5 gyldne regler",
                    Body = @"
                        <ol class=""goldenrules"">
                        <li><strong>Natteravnene</strong> er <strong>synlige</strong> og <strong>observerer</strong>, de blander sig aldrig fysisk i uroligheder o.lign. - men tilkalder professionel hjælp om fornødent.</li>
                        <li><strong>Natteravnene</strong> går <strong>altid i hold på tre</strong> i den karakteristiske gule beklædning. Helst med deltagelse af begge køn og forskellig baggrund - også etnisk.</li>
                        <li><strong>Natteravnene </strong>færdes altid <strong>i det offentlige rum</strong> (på gaden, i bus eller tog)- aldrig inde, hvor de unge morer sig, i ungdomsklubber, værtshuse, diskoteker.</li>
                        <li><strong>Natteravnene</strong> giver sig tid til at <strong>lytte</strong> og tale med børn og unge - på de unges initiativ - men uden at yde egentlig rådgivning. <br>Det overlades efter situationen til professionelle i det lokale netværk eller de unges forældre, familie og bekendte.</li>
                        <li><strong>Natteravnene hjælper</strong> ""forulykkede"" børn og unge - helst gennem deres venner og familie. Holdet følger ikke nogen hjem alene og låner aldrig penge ud.</li>
                        </ol>"
                };
            
            Content Landssekretariatet =  new Content
                {
                    Title = "Landssekretariatet",
                    Body = @"
                        <h2>Natteravnenes Landssekretariat er fødselshjælper</h2>
                        <p>Natteravnenes Landssekretariat er en non-profit organisation.<br /> 
                        Medarbejderne er ansat af Fonden for Socialt Ansvar, der finansieres af større og mindre virksomheder, fonde, offentlige myndigheder samt via fradragsberettigede bidrag fra privatpersoner og arv.</p>
                        <p>Sekretariat står for den overordnede kommunikation, udarbejder materialer og tilbyder løbende råd og vejledning.</p>
                        <p>Sekretariatet tilbyder erfaringsudveksling gennem nyhedsbreve og Landsmøder.</p>
                        <p>I forbindelse med etableringen af nye lokalforeninger tilbyder Sekretariatet fx. hjælp med planlægning, dele af undervisningen, ulykkesforsikring med krisehjælp, lokal hjemmeside. Derefter tager foreningen formelt selv ansvaret for sit fremtidige virke - i samarbejde med det lokale erhvervsliv.</p>
                        <p>Sekretariatet deltager gerne i informationsmøder om konceptet og frivillig ansvarlighed på skoler, institutioner, virksomheder, foreninger m.v.</p>
                        <p>Har du spørgsmål eller en god ide er du velkommen til at kontakte Sekretariatet på tlf. 70 12 12 99 eller på <a href=""mailto:info@natteravnene.dk"" target=""_blank"">info@natteravnene.dk</p>
                        "
                };
            
            Content SIContent =  new Content
                {
                    Title = "Seniorinstruktør",
                    Body = @"
                        <p>I 2009 er Sekretariatet startet på at invitere godt 50 tidligere og nuværende rutinerede Natteravneformænd fra hele landet, til et særligt kursusforløb, hvor de er blevet trænet til, at kunne afholde alle kurser ifm. start af nye foreninger, træning af nye frivillige og hjælp til nystartede styregrupper og bestyrelser.</p>
                        <p>SeniorInstruktørerne vil løbende få grundig indsigt i nye initiativer fra Sekretariatet som f.eks. opbygning og indhold i hjemmesider, E-learning program m.v.</p>
                        <p>Og nogle vil kunne bistå med idéer, kriseløsninger og coaching af de enkelte bestyrelser, i det omfang de selv måtte have interesse i denne hjælp.</p>
                        <p>SeniorInstruktørerne er tilknyttet et geografisk område, som de selv har fundet frem til.</p>
                        <p>Bestyrelserne i det pågældende område kan trække på disse, ligesom de fortsat kan kontakte Sekretariatet. Instruktørerne optræder ofte parvis.</p>
                        <p>SeniorInstruktørerne vil lejlighedsvis deltage i, eller invitere til netværksmøder for bestyrelserne i et givet geografisk område.</p>
                        <p>Få en aktuel liste over SeniorTeamet ved at klikke her</p>
                    "
                };

            context.Content.Add(FiveGyldne);
            context.Content.Add(Landssekretariatet);
            context.Content.Add(SIContent);


            context.SaveChanges();
            context.Database.ExecuteSqlCommand("UPDATE [News] SET [NewsId] = NEWID();");
            context.Database.ExecuteSqlCommand(string.Format("UPDATE [Contents] SET [ContentID] = '00A8EAD6-0A74-E411-BF1A-C485084553DE' WHERE [ContentID] = '{0}' ;", FiveGyldne.ContentID.ToString()));
            context.Database.ExecuteSqlCommand(string.Format("UPDATE [Contents] SET [ContentID] = '10A8EAD6-0A74-E411-BF1A-C485084553DE' WHERE [ContentID] = '{0}' ;", Landssekretariatet.ContentID.ToString()));
            context.Database.ExecuteSqlCommand(string.Format("UPDATE [Contents] SET [ContentID] = '20A8EAD6-0A74-E411-BF1A-C485084553DE' WHERE [ContentID] = '{0}' ;", SIContent.ContentID.ToString()));


#if SEED1

            int[] NoGoAss = { 12, 48, 2, 76 };

            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Foreninger]", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while ((reader.Read()))
                {
                    int StrId = (int)TjekDBNull(reader["ForeningsID"]);
                    if (StrId != 0 && !NoGoAss.Contains(StrId))
                    {
                        ConvertRT.MigrateAssociation(Convert.ToInt32(StrId));
                    }
                }
            }
#endif

        }

        private void CreateConstraint(Repository context, string Table, string Key)
        {
            string DBConstartint1 = "CREATE TRIGGER {0}_updated_Lastchanged ON  [{0}] AFTER UPDATE AS BEGIN SET NOCOUNT ON;" +
                            "UPDATE [{0}] Set [Lastchanged] = GetDate() where [{1}] in (SELECT [{1}] FROM Inserted) END;";
            string DBConstartint2 = "ALTER TABLE [{0}] ADD CONSTRAINT DF_{0}_Created_Default DEFAULT GETDATE() FOR [Created];";
            string DBConstartint3 = "ALTER TABLE [{0}] ADD CONSTRAINT DF_{0}_Lastchanged_Default DEFAULT GETDATE() FOR [Lastchanged];";

            context.Database.ExecuteSqlCommand(string.Format(DBConstartint1, Table, Key));
            context.Database.ExecuteSqlCommand(string.Format(DBConstartint2, Table));
            context.Database.ExecuteSqlCommand(string.Format(DBConstartint3, Table));

        }

        private void CreateDefault(Repository context, string Table)
        {
            string DBConstartint = "ALTER TABLE [{0}] ADD CONSTRAINT DF_{0}_Created_Default DEFAULT GETDATE() FOR [Created];";
            
            context.Database.ExecuteSqlCommand(string.Format(DBConstartint, Table));
            
        }

        private string Quote(DateTime value)
        {
            return Quote(value.ToString("yyyy-MM-dd hh:mm:ss.mmm"));
        }

        private string Quote(bool value)
        {
            return Quote(value.ToString());
        }
        private string Quote(string value)
        {
            if (value == null) return "'NULL'";
            if (string.IsNullOrWhiteSpace(value)) return "''";
            return "'" + value.Replace("'", "''") + "'";
        }

        private string Quote(string value, int length)
        {
            if (value == null) return "'NULL'";
            if (string.IsNullOrWhiteSpace(value)) return "''";
            if (value.Length > length) value = value.Substring(0, length);

            return "'" + value.Replace("'", "''") + "'";
        }

        private object TjekDBNull(object valuebool)
        {
            if (valuebool == null | Convert.IsDBNull(valuebool))
            { return null; }
            else
            { return valuebool; }
        }

        private string ReturnProperty(string property, string PropertyNames, string PropertyValuesString)
        {
            int length = 0;
            int startpos = 0;
            //string result;
            //string PropertyNames = "Mobil:S:0:0:ForeningsID:S:0:2:Efternavn:S:2:6:gender:S:8:1:Fornavn:S:9:4:Postnr:S:13:4:IDWEB:S:17:1:By:S:18:7:Udmeldt:S:25:5:Vej:S:30:12:TurLeder:S:42:5:Telefon:S:47:8:";
            //string PropertyValuesString = "17GerberMJohn62300R�dekroFalseB�geparken 9False73694041";
            //string property = "Efternavn";

            int start = PropertyNames.IndexOf(property) + property.Length + 1;
            var type = PropertyNames.Substring(start, PropertyNames.IndexOf(":", start) - start);
            var startStr = PropertyNames.Substring(start + type.Length + 1, PropertyNames.IndexOf(":", start + type.Length + 1) - (start + type.Length + 1));
            var lengthStr = PropertyNames.Substring(start + type.Length + startStr.Length + 2, PropertyNames.IndexOf(":", start + type.Length + startStr.Length + 2) - (start + type.Length + startStr.Length + 2));
            if (!Int32.TryParse(startStr, out startpos) || !Int32.TryParse(lengthStr, out length))
            {
                return string.Empty;
            }
            if (length <= 0) return string.Empty;

            return PropertyValuesString.Substring(startpos, length);



        }

        public string LokationSQL()
        {

            string SeedSql;

            string InsertLokation =
              "INSERT INTO [Lokationer] ([LokationID], [ForeningsID] ,[Navn] ,[KortNavn] ,[Beskrivelse] ,[lng] ,[lat] ,[SidstRettet] ,[Oprettet]) VALUES (" +
           "{0}," + //LokationID
           "{1}," + //<ForeningsID, int,>
           "{2}," + //<Navn, nvarchar(max),>
           "{3}," + //<KortNavn, nvarchar(3),>
           "{4}," + //<Beskrivelse, nvarchar(max),>
           "NULL," + //<lng, real,>
           "NULL," + //<lat, real,>
           "{5}," + //<SidstRettet, datetime,>
           "{6})" + //<Oprettet, datetime,>))" 
              Environment.NewLine;

            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Lokationer] ", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                SeedSql = "SET IDENTITY_INSERT [Lokationer] ON;" + Environment.NewLine;

                while ((reader.Read()))
                {
                    try
                    {
                        SeedSql += string.Format(InsertLokation,
                            (int)reader["LokID"],//[EventID]
                            (int)TjekDBNull(reader["ForeningsID"]),//(<ForeningsID, int,>
                            Quote((String)reader["Navn"], 25),//<Navn, nvarchar(50),>
                            Quote((String)TjekDBNull(reader["KortNavn"]), 3),//<KortNavn, nvarchar(50),>
                            Quote((String)TjekDBNull(reader["Beskrivelse"]), 500),//<Beskrivelse, nvarchar(50),>
                            Quote((DateTime)reader["SidstRettet"]), //<SidstRettet, datetime,>
                            Quote((DateTime)reader["Oprettet"]) //<Oprettet, datetime,>)
                            ) + Environment.NewLine;

                        //Trace.WriteLine(string.Format("- Lokation processed ID:{0}", reader["LokID"]));
                    }
                    catch
                    {
                        //Trace.WriteLine(string.Format("- Lokation NOT prosessed ID:{0} ({1})", reader["LokID"], reader.ToString()));
                    }
                }
            };
            SeedSql += "SET IDENTITY_INSERT [Lokationer] OFF;" + Environment.NewLine;

            return SeedSql;

        }

        public string TurMappeSQL()
        {
            string SeedSql;

            string InsertTurmappe =
                "INSERT INTO [Turmapper] ([TurMappeID] ,[ForeningsID] ,[TurMappeNavn] ,[Start] ,[Slut] ,[Publiceret] ,[Aaben] ,[SidstRettet] ,[Oprettet]) VALUES " +
                "({0}," + //[TurMappeID]
                "{1}," + //(<ForeningsID, int,>
           "{2}," + //<TurMappeNavn, nvarchar(50),>
           "{3}," + //<Start, datetime,>
           "{4}," + //<Slut, datetime,>
           "{5}," + //<Publiceret, bit,>
           "{6}," + //<Aaben, bit,>
           "{7}," + //<SidstRettet, datetime,>
           "{8});" + //<Oprettet, datetime,>)" 
           Environment.NewLine;

            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [TurMapper] ", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                SeedSql = "SET IDENTITY_INSERT [Turmapper] ON;" + Environment.NewLine;

                while ((reader.Read()))
                {
                    try
                    {
                        SeedSql += string.Format(InsertTurmappe,
                            (int)reader["TurmappeID"],//[TurMappeID]
                            (int)TjekDBNull(reader["ForeningsID"]),//(<ForeningsID, int,>
                            Quote((String)reader["TurMappeNavn"]),//<TurMappeNavn, nvarchar(50),>
                            Quote((DateTime)reader["Start"]), //<Start, datetime,>
                            Quote((DateTime)reader["Slut"]), //<Slut, datetime,>
                            Quote((Boolean)TjekDBNull(reader["Publiceret"])), //<Publiceret, bit,>
                            Quote((Boolean)TjekDBNull(reader["Aaben"])), //<Aaben, bit,>
                            Quote((DateTime)reader["SidstRettet"]), //<SidstRettet, datetime,>
                            Quote((DateTime)reader["Oprettet"]) //<Oprettet, datetime,>)
                            ) + Environment.NewLine;

                        //Trace.WriteLine(string.Format("- TurMappe prosessed ID:{0}", reader["TurmappeID"]));
                    }
                    catch
                    {
                        //Trace.WriteLine(string.Format("- TurMappe NOT prosessed ID:{0} ({1})", reader["TurmappeID"], reader.ToString()));
                    }
                }
            };
            SeedSql += "SET IDENTITY_INSERT [Turmapper] OFF;" + Environment.NewLine;
            return SeedSql;

        }

        public string EventSQL()
        {
            string SeedSql;
            string InsertKalender =
               "INSERT INTO [Events] ([EventID] ,[ForeningsID] ,[Overskrift] ,[Beskrivelse] ,[Start] ,[Slut] ,[SidstRettet] ,[Oprettet]) VALUES " +
               "({0}, " + //EvnetID
               "{1}, " + //<ForeningsID, int,>
               "{2}, " + //<Overskrift, nvarchar(max),>
               "{3}, " + //<Beskrivelse, nvarchar(max),>
               "{4}, " + //<Start, datetime,>
               "{5}, " + //<Slut, datetime,>
               "{6}, " + //<SidstRettet, datetime,>
               "{7})" + //<Oprettet, datetime,>)" 
               Environment.NewLine;

            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Kalender] ", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                SeedSql = "SET IDENTITY_INSERT [Events] ON;" + Environment.NewLine;

                while ((reader.Read()))
                {
                    try
                    {
                        var Forening = (int)TjekDBNull(reader["ForeningsID"]);

                        SeedSql += string.Format(InsertKalender,
                            (int)reader["KalID"],//[EventID]
                            (int)TjekDBNull(reader["ForeningsID"]) > 0 ? ((int)TjekDBNull(reader["ForeningsID"])).ToString() : null,//(<ForeningsID, int,>
                            Quote((String)reader["Overskrift"]),//<Overskrift, nvarchar(50),>
                            Quote((String)TjekDBNull(reader["Krop"])),//<Beskrivelse, nvarchar(50),>
                            Quote((DateTime)reader["Start"]), //<Start, datetime,>
                            Quote((DateTime)reader["Slut"]), //<Slut, datetime,>
                            Quote((DateTime)reader["SidstRettet"]), //<SidstRettet, datetime,>
                            Quote((DateTime)reader["Oprettet"]) //<Oprettet, datetime,>)
                            ) + Environment.NewLine;

                        //Trace.WriteLine(string.Format("- Event processed ID:{0}", reader["KalID"]));
                    }
                    catch
                    {
                        //Trace.WriteLine(string.Format("- Event NOT prosessed ID:{0} ({1})", reader["KalID"], reader.ToString()));
                    }
                }
            };
            SeedSql += "SET IDENTITY_INSERT [Events] OFF;" + Environment.NewLine;
            return SeedSql;
        }

    }


}