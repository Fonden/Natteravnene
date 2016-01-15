/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using DTA;
using HtmlAgilityPack;
using NR.Abstract;
using NR.Entity;
using NR.Infrastructure;
using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMatrix.WebData;

namespace NR.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    [HandleError500]
    public class MigrationController : Controller
    {
        //Repository
        private INRRepository reposetory;

        public MigrationController(INRRepository NTRepository)
        {
            this.reposetory = NTRepository;
            seedConnectionString = ConfigurationManager.AppSettings["Ravnetur"] ?? @"Data Source=.\SQLEXPRESS;Initial Catalog=Ravnetur;Integrated Security=True";
        }

        #region SEEDDATA


        static List<SeedData> Seed = new List<SeedData>{
                            new SeedData { Name = "Haslev", Number = 44, Network = 12, Established = "29-1-2001",
                            Governance = GovernanceType.Traditionel,
                            Chairmann ="DTA",
                            Auditor = "PECH",
                            Accountant = "JEDY",
                            BoardMembers = new List<string> 
                            { "KULA", "ULHA", "RIWI", "SUKI", "CLSM" }
                            },
                            
                            
                            
                            
                            new SeedData { Name = "Albertslund", Number = 217, Network = 15, Established = "26-1-2009"},
                            new SeedData { Name = "Almind", Number = 167, Network = 8, Established = "3-2-2011"},
                            new SeedData { Name = "Ansager (Ølgod)", Number = 199, Network = 6, Established = "?"},
                            new SeedData { Name = "Arden", Number = 65, Network = 2, Established = "5-9-2001"},
                            new SeedData { Name = "Askerød (Greve)", Number = 220, Network = 16, Established = "21-11-2008"},
                            new SeedData { Name = "Assens", Number = 72, Network = 9, Established = "26-9-2001"},
                            new SeedData { Name = "Ballerup", Number = 70, Network = 99, Established = "24-9-2001"},
                            new SeedData { Name = "Billund", Number = 118, Network = 6, Established = "26-3-2003"},
                            new SeedData { Name = "Birkerød", Number = 189, Network = 10, Established = "25-10-2006"},
                            new SeedData { Name = "Bispebjerg", Number = 179, Network = 14, Established = "5-4-2006"},
                            new SeedData { Name = "Bispehaven", Number = 48, Network = 4, Established = "21-2-2001"},
                            new SeedData { Name = "Bispehavens Kvindegruppe", Number = 251, Network = 4, Established = "11-6-2010"},
                            new SeedData { Name = "Bjerringbro", Number = 27, Network = 3, Established = "18-9-2000"},
                            new SeedData { Name = "Blaabjerg (Varde)", Number = 114, Network = 6, Established = "27-1-2003"},
                            new SeedData { Name = "Bogense", Number = 143, Network = 9, Established = "26-8-2004"},
                            new SeedData { Name = "Bramming", Number = 94, Network = 6, Established = "27-5-2002"},
                            new SeedData { Name = "Bramsnæs", Number = 210, Network = 11, Established = "23-8-2008"},
                            new SeedData { Name = "Brande", Number = 159, Network = 5, Established = "27-4-2005"},
                            new SeedData { Name = "Brandrupdam (Kolding)", Number = 216, Network = 8, Established = "7-10-2008"},
                            new SeedData { Name = "Brovst", Number = 151, Network = 1, Established = "28-10-2004"},
                            new SeedData { Name = "Brædstrup (Horsens)", Number = 264, Network = 8, Established = "28-12-2012"},
                            new SeedData { Name = "Brøndby Strand", Number = 43, Network = 16, Established = "23-1-2001"},
                            new SeedData { Name = "Brøndbyøster", Number = 121, Network = 15, Established = "12-5-2003"},
                            new SeedData { Name = "Brønderslev", Number = 37, Network = 1, Established = "7-11-2000"},
                            new SeedData { Name = "Brønshøj ", Number = 226, Network = 14, Established = "9-3-2009"},
                            new SeedData { Name = "Brønshøj-Husum", Number = 58, Network = 14, Established = "19-4-2001"},
                            new SeedData { Name = "Brønshøj-Husum", Number = 0, Network = 14, Established = "9-3-2009"},
                            new SeedData { Name = "Brørup", Number = 156, Network = 6, Established = "10-3-2005"},
                            new SeedData { Name = "Børkop", Number = 207, Network = 8, Established = "21-3-2005"},
                            new SeedData { Name = "Christiansfeld", Number = 135, Network = 8, Established = "18-3-2004"},
                            new SeedData { Name = "Dalby", Number = 191, Network = 12, Established = "12-3-2007"},
                            new SeedData { Name = "De etniske kvinder i Kbh.", Number = 200, Network = 14, Established = "31-10-2007"},
                            new SeedData { Name = "Den grønne Trekant", Number = 184, Network = 14, Established = "21-6-2006"},
                            new SeedData { Name = "Dronninglund", Number = 18, Network = 1, Established = "10-6-2000"},
                            new SeedData { Name = "Ebeltoft", Number = 136, Network = 4, Established = "23-3-2004"},
                            new SeedData { Name = "Esbjerg", Number = 267, Network = 6, Established = "12-3-2014"},
                            new SeedData { Name = "Esbjerg City", Number = 6, Network = 6, Established = "4-10-1999"},
                            new SeedData { Name = "Espergærde (Helsingør)", Number = 194, Network = 10, Established = "11-5-2007"},
                            new SeedData { Name = "Fakse og Ladepladsen", Number = 76, Network = 12, Established = "26-11-2001"},
                            new SeedData { Name = "Fakseladeplads", Number = 19, Network = 12, Established = "13-6-2000"},
                            new SeedData { Name = "Farsø", Number = 74, Network = 2, Established = "13-1-2001"},
                            new SeedData { Name = "Farum", Number = 176, Network = 10, Established = "7-3-2006"},
                            new SeedData { Name = "Fjerritslev", Number = 175, Network = 1, Established = "7-3-2006"},
                            new SeedData { Name = "Folehaven", Number = 174, Network = 14, Established = "22-2-2006"},
                            new SeedData { Name = "Fredericia", Number = 113, Network = 8, Established = "3-12-2002"},
                            new SeedData { Name = "Fredericia Vest (Fredericia)", Number = 248, Network = 8, Established = "12-5-2010"},
                            new SeedData { Name = "Frederiksberg", Number = 221, Network = 14, Established = "27-11-2008"},
                            new SeedData { Name = "Frederikshavn (Skagen)", Number = 23, Network = 1, Established = "31-8-2000"},
                            new SeedData { Name = "Frederikssund", Number = 81, Network = 10, Established = "12-2-2002"},
                            new SeedData { Name = "Faaborg-Midtfyn", Number = 11, Network = 9, Established = "29-2-2000"},
                            new SeedData { Name = "Gedved", Number = 232, Network = 8, Established = "25-6-2009"},
                            new SeedData { Name = "Gentofte", Number = 68, Network = 10, Established = "18-9-2001"},
                            new SeedData { Name = "Give", Number = 45, Network = 8, Established = "29-1-2001"},
                            new SeedData { Name = "Gjesing", Number = 178, Network = 6, Established = "28-3-2006"},
                            new SeedData { Name = "Gladsaxe", Number = 130, Network = 15, Established = "7-10-2003"},
                            new SeedData { Name = "Glamsbjerg", Number = 84, Network = 9, Established = "25-2-2002"},
                            new SeedData { Name = "Glostrup-Hvissinge", Number = 31, Network = 99, Established = "25-10-2000"},
                            new SeedData { Name = "Gram", Number = 166, Network = 7, Established = "3-11-2005"},
                            new SeedData { Name = "Grenå", Number = 195, Network = 4, Established = "16-5-2007"},
                            new SeedData { Name = "Greve", Number = 10, Network = 16, Established = "28-2-2000"},
                            new SeedData { Name = "Grindsted", Number = 17, Network = 6, Established = "6-6-2000"},
                            new SeedData { Name = "Græsted/Gilleleje", Number = 96, Network = 10, Established = "13-6-2002"},
                            new SeedData { Name = "Gråsten (under Åbenrå)", Number = 99, Network = 7, Established = "11-6-2012"},
                            new SeedData { Name = "Gullestrup", Number = 234, Network = 5, Established = "9-12-2009"},
                            new SeedData { Name = "Gørlev", Number = 149, Network = 11, Established = "27-9-2004"},
                            new SeedData { Name = "Haderslev - Syd", Number = 127, Network = 7, Established = "19-6-2007"},
                            new SeedData { Name = "Haderslev", NewName= "Haderslev C", Number = 168, Network = 7, Established = "1-12-2005"},
                            new SeedData { Name = "Hadsund", Number = 26, Network = 2, Established = "12-9-2000"},
                            new SeedData { Name = "Hals", NewName = "Hals-Aalborg", Number = 85, Network = 1, Established = "11-3-2002"},
                            new SeedData { Name = "Halsnæs", Number = 115, Network = 10, Established = "4-2-2003"},
                            new SeedData { Name = "Hammel", Number = 89, Network = 4, Established = "23-4-2002"},
                            new SeedData { Name = "Hedehusene", Number = 180, Network = 15, Established = "27-10-2009"},
                            new SeedData { Name = "Hedensted", Number = 205, Network = 8, Established = "7-5-2008"},
                            new SeedData { Name = "Hedensted - Vest", Number = 116, Network = 8, Established = "27-2-2003"},
                            new SeedData { Name = "Helsinge", Number = 134, Network = 10, Established = "10-12-2003"},
                            new SeedData { Name = "Helsingør", Number = 1, Network = 10, Established = "6-3-1998"},
                            new SeedData { Name = "Herlev", Number = 187, Network = 15, Established = "12-10-2006"},
                            new SeedData { Name = "Herning", Number = 34, Network = 3, Established = "30-10-2000"},
                            new SeedData { Name = "Herredsvang (Aarhus V)", Number = 80, Network = 4, Established = "20-2-2002"},
                            new SeedData { Name = "Hillerød", Number = 124, Network = 10, Established = "10-6-2003"},
                            new SeedData { Name = "Hinnerup", Number = 109, Network = 4, Established = "11-11-2002"},
                            new SeedData { Name = "Hirtshals", Number = 53, Network = 1, Established = "21-3-2001"},
                            new SeedData { Name = "Hjordkær (Aabenraa)", Number = 170, Network = 7, Established = "19-1-2006"},
                            new SeedData { Name = "Hjørring", Number = 57, Network = 1, Established = "9-4-2001"},
                            new SeedData { Name = "Hobro", Number = 120, Network = 2, Established = "10-4-2003"},
                            new SeedData { Name = "Holbæk", Number = 108, Network = 11, Established = "28-10-2002"},
                            new SeedData { Name = "Holmbladsgade", Number = 186, Network = 99, Established = "9-10-2006"},
                            new SeedData { Name = "Holstebro", Number = 4, Network = 5, Established = "20-5-1999"},
                            new SeedData { Name = "Holsted", Number = 209, Network = 6, Established = "18-6-2008"},
                            new SeedData { Name = "Holtbjerg", Number = 144, Network = 5, Established = "20-6-2004"},
                            new SeedData { Name = "Hornbæk (Helsingør)", Number = 139, Network = 10, Established = "31-3-2004"},
                            new SeedData { Name = "Hornslet", Number = 202, Network = 4, Established = "11-3-2008"},
                            new SeedData { Name = "Horsens", Number = 16, Network = 8, Established = "30-5-2000"},
                            new SeedData { Name = "Hovedgaard", Number = 238, Network = 8, Established = "27-1-2010"},
                            new SeedData { Name = "Hundested (Halsnæs)", Number = 188, Network = 11, Established = "?"},
                            new SeedData { Name = "Husum", Number = 246, Network = 14, Established = "1-3-2011"},
                            new SeedData { Name = "Hvide Sande (Holmsland)", Number = 153, Network = 5, Established = "17-11-2004"},
                            new SeedData { Name = "Hvidebæk Kommune", Number = 155, Network = 15, Established = "22-2-2005"},
                            new SeedData { Name = "Hvidovre (tidligere Avedøre)", Number = 95, Network = 99, Established = "6-6-2002"},
                            new SeedData { Name = "Hvidovre Syd", Number = 211, Network = 16, Established = "25-8-2008"},
                            new SeedData { Name = "Høje Gladsaxe (Gammel)", Number = 250, Network = 99, Established = "24-11-2010"},
                            new SeedData { Name = "Høng", Number = 63, Network = 11, Established = "7-8-2001"},
                            new SeedData { Name = "Hørning", Number = 243, Network = 4, Established = "8-3-2010"},
                            new SeedData { Name = "Hørsholm", Number = 240, Network = 10, Established = "16-3-2010"},
                            new SeedData { Name = "Haarby", Number = 87, Network = 9, Established = "6-4-2002"},
                            new SeedData { Name = "Ikast", Number = 138, Network = 5, Established = "25-3-2004"},
                            new SeedData { Name = "Ilulissat", Number = 92, Network = 19, Established = "09-052002"},
                            new SeedData { Name = "Indre Nørrebro", Number = 212, Network = 14, Established = "25-8-2008"},
                            new SeedData { Name = "Ishøj", Number = 47, Network = 16, Established = "15-2-2001"},
                            new SeedData { Name = "Juelsminde", Number = 148, Network = 8, Established = "23-9-2004"},
                            new SeedData { Name = "Jyderup (overgået til Holbæk)", Number = 196, Network = 11, Established = "19-6-2007"},
                            new SeedData { Name = "Jyllinge", Number = 163, Network = 10, Established = "30-6-2005"},
                            new SeedData { Name = "Kalundborg", Number = 62, Network = 11, Established = "7-6-2001"},
                            new SeedData { Name = "Karup", Number = 123, Network = 3, Established = "4-6-2003"},
                            new SeedData { Name = "Kastrup", NewName  = "Tårnby", Number = 219, Network = 14, Established = "18-11-2008"},
                            new SeedData { Name = "Kgs. Enghave", Number = 157, Network = 99, Established = "17-3-2005"},
                            new SeedData { Name = "Kjellerup", Number = 54, Network = 3, Established = "29-3-2001"},
                            new SeedData { Name = "Klaksvig", Number = 190, Network = 18, Established = "14-12-2006"},
                            new SeedData { Name = "Klarup", Number = 225, Network = 2, Established = "4-3-2009"},
                            new SeedData { Name = "Kokkedal", Number = 40, Network = 10, Established = "23-11-2000"},
                            new SeedData { Name = "Kolding", Number = 15, Network = 8, Established = "22-5-2000"},
                            new SeedData { Name = "Korsør", Number = 100, Network = 11, Established = "22-8-2002"},
                            new SeedData { Name = "Kvaglund-Stengårdsvej", Number = 214, Network = 6, Established = "16-9-2008"},
                            new SeedData { Name = "København C (Kritiske Muslimer)", Number = 99, Network = 14, Established = "19-8-2002"},
                            new SeedData { Name = "København City", Number = 204, Network = 14, Established = "2-11-2007"},
                            new SeedData { Name = "Køge", Number = 42, Network = 16, Established = "28-11-2000"},
                            new SeedData { Name = "Langeland", Number = 140, Network = 9, Established = "27-4-2004"},
                            new SeedData { Name = "Langeskov", Number = 197, Network = 9, Established = "8-10-2007"},
                            new SeedData { Name = "Lemvig", Number = 192, Network = 3, Established = "15-3-2007"},
                            new SeedData { Name = "Lunderskov", Number = 66, Network = 8, Established = "6-9-2001"},
                            new SeedData { Name = "Lundtoftegade", Number = 9, Network = 14, Established = "9-2-2000"},
                            new SeedData { Name = "Lyngby", Number = 229, Network = 10, Established = "13-5-2009"},
                            new SeedData { Name = "Løgstør", Number = 8, Network = 1, Established = "23-11-1999"},
                            new SeedData { Name = "Løgumkloster (Tønder)", Number = 161, Network = 7, Established = "2-5-2005"},
                            new SeedData { Name = "Løkken", Number = 101, Network = 1, Established = "5-9-2002"},
                            new SeedData { Name = "Maniitsoq", Number = 249, Network = 19, Established = "20-10-2010"},
                            new SeedData { Name = "Mariager", Number = 24, Network = 4, Established = "7-9-2000"},
                            new SeedData { Name = "Maribo", Number = 119, Network = 13, Established = "7-4-2003"},
                            new SeedData { Name = "MC - Natteravnene", Number = 215, Network = 10, Established = "29-6-2008"},
                            new SeedData { Name = "Middelfart", Number = 7, Network = 9, Established = "27-10-1999"},
                            new SeedData { Name = "Mjølnerparken-Kbh. N", Number = 83, Network = 99, Established = "21-2-2002"},
                            new SeedData { Name = "Munkebo", Number = 141, Network = 9, Established = "11-5-2004"},
                            new SeedData { Name = "Møldrup", Number = 12, Network = 3, Established = "4-5-2000"},
                            new SeedData { Name = "Nakskov", Number = 60, Network = 13, Established = "29-5-2001"},
                            new SeedData { Name = "Nanortalik", Number = 103, Network = 19, Established = "14-9-2002"},
                            new SeedData { Name = "Narsaq", Number = 255, Network = 19, Established = "12-11-2011"},
                            new SeedData { Name = "Nexø", Number = 46, Network = 17, Established = "13-2-2001"},
                            new SeedData { Name = "Nibe", Number = 51, Network = 2, Established = "19-3-2001"},
                            new SeedData { Name = "Nordals", Number = 41, Network = 7, Established = "23-11-2000"},
                            new SeedData { Name = "Nr. Rangstrup", Number = 150, Network = 7, Established = "27-10-2004"},
                            new SeedData { Name = "Nuuk", Number = 25, Network = 19, Established = "7-9-2000"},
                            new SeedData { Name = "Nykøbing Falster", Number = 61, Network = 13, Established = "6-6-2001"},
                            new SeedData { Name = "Nykøbing Falster", Number = 247, Network = 13, Established = "5-7-2010"},
                            new SeedData { Name = "Nykøbing Mors", Number = 133, Network = 3, Established = "7-10-2003"},
                            new SeedData { Name = "Næstved", Number = 3, Network = 13, Established = "23-4-1999"},
                            new SeedData { Name = "Nørager (Bero marts  2013)", Number = 110, Network = 2, Established = "14-11-2002"},
                            new SeedData { Name = "Odden (Odsherred)", Number = 145, Network = 11, Established = "30-6-2004"},
                            new SeedData { Name = "Odense", Number = 259, Network = 9, Established = "15-2-2012"},
                            new SeedData { Name = "Odsherred", Number = 97, Network = 11, Established = "17-6-2002"},
                            new SeedData { Name = "Otterup", Number = 49, Network = 9, Established = "27-2-2001"},
                            new SeedData { Name = "Padborg (Aabenraa)", Number = 128, Network = 7, Established = "9-9-2003"},
                            new SeedData { Name = "Pal Familie Natteravne ", Number = 237, Network = 99, Established = "?"},
                            new SeedData { Name = "Pandrup", Number = 77, Network = 1, Established = "22-11-2001"},
                            new SeedData { Name = "Præstø", Number = 86, Network = 99, Established = "18-3-2002"},
                            new SeedData { Name = "Paamiut", Number = 254, Network = 19, Established = "31-3-2011"},
                            new SeedData { Name = "Qaqortog", Number = 201, Network = 19, Established = "20-2-2008"},
                            new SeedData { Name = "Qasigiannguit", Number = 0, Network = 19, Established = "16-9-2004"},
                            new SeedData { Name = "Qasigiannguit", Number = 147, Network = 19, Established = "16-9-2004"},
                            new SeedData { Name = "Qaanaaq", Number = 262, Network = 19, Established = "24-5-2012"},
                            new SeedData { Name = "Ramløse (Helsinge)", Number = 252, Network = 10, Established = "0-1-1900"},
                            new SeedData { Name = "Randers", Number = 2, Network = 4, Established = "25-9-1998"},
                            new SeedData { Name = "Ribe", Number = 67, Network = 6, Established = "17-9-2001"},
                            new SeedData { Name = "Ringkøbing & Søndervig", NewName = "Ringkøbing", Number = 142, Network = 5, Established = "17-8-2004"},
                            new SeedData { Name = "Ringsted", Number = 73, Network = 11, Established = "9-10-2001"},
                            new SeedData { Name = "Roskilde/Viby",  NewName = "Roskilde", Number = 52, Network = 10, Established = "21-3-2001"},
                            new SeedData { Name = "Rudersdal", Number = 231, Network = 10, Established = "17-6-2009"},
                            new SeedData { Name = "Ruds Vedby", Number = 263, Network = 11, Established = "24-10-2012"},
                            new SeedData { Name = "Ry", Number = 182, Network = 4, Established = "18-5-2006"},
                            new SeedData { Name = "Røde Kro (Aabenraa)", Number = 171, Network = 7, Established = "0-1-1900"},
                            new SeedData { Name = "Rødovre", Number = 227, Network = 15, Established = "30-3-2009"},
                            new SeedData { Name = "Rønde", Number = 122, Network = 4, Established = "26-5-2003"},
                            new SeedData { Name = "Rønne", Number = 5, Network = 17, Established = "11-6-1999"},
                            new SeedData { Name = "Samsø", Number = 228, Network = 4, Established = "3-6-2009"},
                            new SeedData { Name = "Seest (Kolding)", Number = 233, Network = 8, Established = "0-1-1900"},
                            new SeedData { Name = "Silkeborg", Number = 56, Network = 3, Established = "20-6-2004"},
                            new SeedData { Name = "Sindal", Number = 36, Network = 1, Established = "1-11-2000"},
                            new SeedData { Name = "Sisimiut (tidligere)", Number = 22, Network = 19, Established = "25-8-2000"},
                            new SeedData { Name = "Sisimiut", Number = 268, Network = 19, Established = "18-1-2014"},
                            new SeedData { Name = "Sjælør", Number = 104, Network = 14, Established = "17-9-2002"},
                            new SeedData { Name = "Skagen (Frederikshavn)", Number = 32, Network = 1, Established = "26-10-2000"},
                            new SeedData { Name = "Skanderborg", Number = 206, Network = 4, Established = "13-5-2008"},
                            new SeedData { Name = "Skive", Number = 93, Network = 3, Established = "14-5-2002"},
                            new SeedData { Name = "Skjern-Egvad", Number = 132, Network = 5, Established = "28-10-2003"},
                            new SeedData { Name = "Skovlunde", Number = 106, Network = 15, Established = "18-9-2002"},
                            new SeedData { Name = "Skovparken (Kolding)", Number = 154, Network = 8, Established = "5-1-2005"},
                            new SeedData { Name = "Skælskør", Number = 125, Network = 11, Established = "16-6-2003"},
                            new SeedData { Name = "Skærbæk", Number = 177, Network = 7, Established = "14-3-2006"},
                            new SeedData { Name = "Skævinge", Number = 152, Network = 10, Established = "8-11-2004"},
                            new SeedData { Name = "Skørping-Terndrup", Number = 13, Network = 2, Established = "8-6-2000"},
                            new SeedData { Name = "Slagelse", Number = 29, Network = 11, Established = "16-10-2000"},
                            new SeedData { Name = "Slangerup", Number = 137, Network = 10, Established = "24-3-2004"},
                            new SeedData { Name = "Smidstrup-Skærup", Number = 208, Network = 8, Established = "21-5-2008"},
                            new SeedData { Name = "Smørum", Number = 185, Network = 10, Established = "29-6-2006"},
                            new SeedData { Name = "Solrød", Number = 164, Network = 16, Established = "19-9-2005"},
                            new SeedData { Name = "Sorø", Number = 242, Network = 11, Established = "26-4-2010"},
                            new SeedData { Name = "St.Heddinge", Number = 126, Network = 13, Established = "17-6-2003"},
                            new SeedData { Name = "Stege", Number = 35, Network = 13, Established = "31-10-2000"},
                            new SeedData { Name = "Stenløse", Number = 14, Network = 10, Established = "15-5-2000"},
                            new SeedData { Name = "Struer", Number = 64, Network = 3, Established = "16-8-2001"},
                            new SeedData { Name = "Støvring", Number = 88, Network = 2, Established = "10-4-2002"},
                            new SeedData { Name = "Suðuroy", Number = 256, Network = 18, Established = "19-11-2011"},
                            new SeedData { Name = "Sundbyvester", Number = 224, Network = 14, Established = "21-2-2009"},
                            new SeedData { Name = "Glumsø", NewName = "Suså-Glumsø", Number = 38, Network = 13, Established = "14-11-2000"},
                            new SeedData { Name = "Svendborg", NewName = "Svendborg - Ærø", Number = 69, Network = 9, Established = "20-9-2001"},
                            new SeedData { Name = "Svogerslev (Roskilde)", Number = 98, Network = 10, Established = "18-6-2002"},
                            new SeedData { Name = "Sæby", Number = 158, Network = 1, Established = "14-4-2005"},
                            new SeedData { Name = "Sønderborg", Number = 30, Network = 7, Established = "24-10-2000"},
                            new SeedData { Name = "Sønderris", Number = 82, Network = 6, Established = "21-2-2002"},
                            new SeedData { Name = "Tasiilaq", Number = 253, Network = 19, Established = "31-3-2011"},
                            new SeedData { Name = "Thisted", Number = 131, Network = 3, Established = "20-10-2003"},
                            new SeedData { Name = "Thyregod", Number = 230, Network = 8, Established = "13-5-2009"},
                            new SeedData { Name = "Tilst", Number = 21, Network = 4, Established = "17-8-2000"},
                            new SeedData { Name = "Tingbjerg (Husum)", Number = 236, Network = 14, Established = "11-1-2010"},
                            new SeedData { Name = "Tistrup (Ølgod)", Number = 198, Network = 6, Established = "0-1-1900"},
                            new SeedData { Name = "Tommerup", Number = 222, Network = 9, Established = "2-12-2008"},
                            new SeedData { Name = "Tórshavn", Number = 75, Network = 18, Established = "14-11-2001"},
                            new SeedData { Name = "Trige", Number = 160, Network = 4, Established = "28-4-2005"},
                            new SeedData { Name = "Tune (Greve)", Number = 146, Network = 16, Established = "0-1-1900"},
                            new SeedData { Name = "Tønder", Number = 181, Network = 7, Established = "9-5-2006"},
                            new SeedData { Name = "Taastrup", Number = 55, Network = 99, Established = "2-4-2001"},
                            new SeedData { Name = "Tåstrup (Gadehavegård)", Number = 112, Network = 15, Established = "07 03 2012"},
                            new SeedData { Name = "Ullerslev", Number = 102, Network = 9, Established = "12-9-2002"},
                            new SeedData { Name = "Urbanplanen", Number = 1, Network = 14, Established = "14-6-2005"},
                            new SeedData { Name = "Uummannaq", Number = 59, Network = 19, Established = "20-4-2001"},
                            new SeedData { Name = "Uummannaq", Number = 258, Network = 19, Established = "10-1-2012"},
                            new SeedData { Name = "Vagum", Number = 218, Network = 18, Established = "1-11-2008"},
                            new SeedData { Name = "Valby Centrum", NewName= "Valby", Number = 203, Network = 14, Established = "1-4-2008"},
                            new SeedData { Name = "Vallensbæk", Number = 20, Network = 16, Established = "14-6-2000"},
                            new SeedData { Name = "Vamdrup", Number = 28, Network = 8, Established = "2-10-2000"},
                            new SeedData { Name = "Vanløse", Number = 260, Network = 14, Established = "8-3-2012"},
                            new SeedData { Name = "Varde", Number = 33, Network = 6, Established = "30-10-2000"},
                            new SeedData { Name = "Vejen", Number = 50, Network = 6, Established = "19-3-2001"},
                            new SeedData { Name = "Vejle", Number = 241, Network = 8, Established = "21-4-2010"},
                            new SeedData { Name = "Vejle-C", Number = 105, Network = 8, Established = "17-9-2002"},
                            new SeedData { Name = "Vejle-Nord", Number = 90, Network = 8, Established = "2-5-2002"},
                            new SeedData { Name = "Vejle-Syd", Number = 91, Network = 8, Established = "2-5-2002"},
                            new SeedData { Name = "Vemmelev", Number = 213, Network = 11, Established = "3-9-2008"},
                            new SeedData { Name = "Viborg", Number = 107, Network = 3, Established = "1-12-2008"},
                            new SeedData { Name = "Viby Sjælland (Roskilde)", Number = 245, Network = 10, Established = "21-6-2010"},
                            new SeedData { Name = "Videbæk", Number = 183, Network = 5, Established = "7-6-2006"},
                            new SeedData { Name = "Vojens", Number = 79, Network = 7, Established = "5-2-2002"},
                            new SeedData { Name = "Vordingborg", Number = 39, Network = 13, Established = "20-11-2000"},
                            new SeedData { Name = "Ældre Sagens Natteravne", Number = 239, Network = 4, Established = "29-10-2010"},
                            new SeedData { Name = "Ældre Sagens Natteravne", Number = 0, Network = 1, Established = "0-1-1900"},
                            new SeedData { Name = "Ærø (Svendborg -  Ærø)", Number = 261, Network = 9, Established = "7-4-2012"},
                            new SeedData { Name = "Ølgod", Number = 111, Network = 6, Established = "21-11-2002"},
                            new SeedData { Name = "Ølsemagle/Ølby", Number = 129, Network = 1, Established = "6-10-2003"},
                            new SeedData { Name = "Ørbæk", Number = 169, Network = 9, Established = "5-12-2005"},
                            new SeedData { Name = "Ragnhildgade", NewName= "Østerbro", Number = 71, Network = 14, Established = "10-10-2001"},
                            new SeedData { Name = "AAB58/Nørrebro", Number = 257, Network = 14, Established = "14 11 2011"},
                            new SeedData { Name = "AAB58/Nørrebro", Number = 257, Network = 14, Established = "14-11-2011"},
                            new SeedData { Name = "Aabenraa", Number = 117, Network = 7, Established = "13-3-2003"},
                            new SeedData { Name = "Aakirkeby (Nexø)", Number = 165, Network = 17, Established = "29-9-2005"},
                            new SeedData { Name = "Aalborg C (slået sam<men med Hals marts 14)", Number = 235, Network = 1, Established = "7-12-2009"},
                            new SeedData { Name = "Aalborg Ø", Number = 223, Network = 2, Established = "19-1-2009"},
                            new SeedData { Name = "Aalestrup", Number = 173, Network = 2, Established = "30-1-2006"},
                            new SeedData { Name = "Ålholm", Number = 172, Network = 1, Established = "30-1-2006"},
                            new SeedData { Name = "Århus Nord", NewName = "Århus N", Number = 244, Network = 4, Established = "22-6-2010"},
                            new SeedData { Name = "Århus V ", Number = 193, Network = 4, Established = "24-4-2007"},
                            new SeedData { Name = "Års", Number = 78, Network = 2, Established = "4-12-2001"}

        
        };




        #endregion

        private static string seedConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Ravnetur;Integrated Security=True";

        int[] NoGoAss = { 12, 48, 2, 76 };

        private static IDictionary<int, string> tasks = new Dictionary<int, string>();

        private static string progress;

        private static string ProfilDirSetting;

        // GET: Migration
        public ActionResult Index()
        {
            List<SelectListItem> Forningliste = new List<SelectListItem>();
            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {


                SqlCommand cmd = new SqlCommand("SELECT * FROM [Foreninger] ORDER BY Navn", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                Forningliste.Add(new SelectListItem
                {
                    Text = "Alle",
                    Value = "0"
                });

                while ((reader.Read()))
                {
                    int StrId = (int)TjekDBNull(reader["ForeningsID"]);
                    if (StrId != 0 && !NoGoAss.Contains(StrId))
                    {
                        Forningliste.Add(new SelectListItem
                        {
                            Text = (String)reader["Navn"],
                            Value = StrId.ToString()
                        });
                    }
                }
            }


            return View(Forningliste);
        }

        public ActionResult Start(int? ID)
        {
            var taskId = ID == null ? 0 : (int)ID;
            progress = "*** Start ****";
            if (!tasks.Keys.Contains(taskId))
            {
                tasks.Add(taskId, string.Empty);
                ProfilDirSetting = Server.MapPath(Url.Content(ConfigurationManager.AppSettings["ProfileImage"]));
                Task.Factory.StartNew(() =>
                {
                    //for (var i = 0; i <= 100; i++)
                    //{
                    //    //tasks[taskId] = i; // update task progress
                    //    //progress += "<br/> NY:" + i.ToString(); 
                    //    //Thread.Sleep(50); // simulate long running operation


                    //}
                    if (taskId != 0) MigrateAssociation(taskId);

                    if (taskId == 0)
                    {
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
                                    Guid migID = MigrateAssociation(Convert.ToInt32(StrId));
                                    if (migID != Guid.Empty)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    tasks.Remove(taskId);
                    progress += "<br />*** Slut ****";
                });
            }
            return Json(taskId);

            //ConvertRT.MigrateAssociation(ForningID);


        }

        public ActionResult SendWelcome()
        {
            List<SelectListItem> Forningliste;
            using (var context = new Repository())
            {
                Forningliste = context.Associations
                    .OrderBy(m => m.Name).ToList().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.AssociationID.ToString()
                    }).ToList();
            }
            return View(Forningliste);
        }

        public ActionResult Sending(Guid? ID)
        {
            var taskId = ID == null ? Guid.Empty : (Guid)ID;
            progress = "*** Start ****";
            if (taskId != Guid.Empty)
            {
                List<WelcomeMailEmail> Mails;
                using (var context = new Repository())
                {
                    Mails = context.Memberships.Include(m => m.Person).Where(m => m.Association.AssociationID == ID).Where(m => m.Type == PersonType.Active & m.AbsentDate == null & m.Person.Email != "").OrderBy(m => String.Concat(m.Person.FirstName, " ", m.Person.FamilyName)).ToList()
                        .Select(u => new WelcomeMailEmail
                        {
                            To = u.Person.Email,
                            UserName = u.Person.UserName,
                            Password = u.Person.Password
                        }).ToList();
                }

                foreach (WelcomeMailEmail Mail in Mails)
                {
                    Mail.Send();
                    progress += "<br />->" + Mail.To;
                }

            }
            progress += "<br />*** Slut ****";


            return Json(taskId);

            //ConvertRT.MigrateAssociation(ForningID);


        }

        public ActionResult Progress(int? id)
        {
            return Json(progress);
        }

        public ActionResult ImportCSV()
        {


            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult _UploadCSV(HttpPostedFileBase file, Guid id, string Forening)
        {
            if (file != null)
            {
                string line;
                Dictionary<string, string> ProfilImages = new Dictionary<string, string>();
                ProfilDirSetting = Server.MapPath(Url.Content(ConfigurationManager.AppSettings["ProfileImage"]));
                var Memberships = new List<NRMembership>();
                List<String[]> Data = new List<String[]>();

                //FileStream aFile = new FileStream("../../sportsResults.csv", FileMode.Open);
                StreamReader sr = new StreamReader(file.InputStream, Encoding.GetEncoding(1252));

                // read data in line by line
                string header = sr.ReadLine();
                if (!header.StartsWith(@"""ID"";""Forening (R)"";""Username"";""Password"";""Indmeldt"";""Fodselsdag"";""Fornavn"";""Efternavn"";""Adresse"";""Postnr"";""By"";""Email"";""Mobil"";""Telefon"";""Notat"";""Aktiv"";""Udmeldt"";""Udmeldt"";""Ønsker IKKE Natteravnenyt"";""Idkort"";""Førstehjælp"";""Grunduddannelse"";""Billede"";""Login"";""Netværk"";""Login"";""Funktioner"";")) return Json(new { success = false, errorMessage = "Ikke korrekt header" });
                Data.Add(header.Split(';'));

                while ((line = sr.ReadLine()) != null)
                {
                    while (!line.Trim().EndsWith("\";"))
                    {
                        line += sr.ReadLine();
                    }
                    var AvivaData = line.Split(';');
                    for (int t = 0; t < AvivaData.Length; t++)
                    {
                        AvivaData[t] = AvivaData[t].TrimStart('"').TrimEnd('"');
                    }


                    if (AvivaData.Length > 15 & (Forening == string.Empty | Forening == AvivaData[1]))
                    {
                        Data.Add(AvivaData);
                        DateTime OutDate;
                        var membership = new NRMembership
                        {
                            Person = new Person()
                        };
                        int Avivaid;
                        if (int.TryParse(AvivaData[0].Trim(), out Avivaid)) membership.Person.IDWEB = Avivaid;
                        membership.Person.UserName = AvivaData[2] + new Guid().ToString();
                        membership.Person.Password = AvivaData[3];
                        if (DateTime.TryParse(AvivaData[4], out OutDate)) membership.SignupDate = OutDate;
                        if (DateTime.TryParse(AvivaData[5], out OutDate)) membership.Person.BirthDate = OutDate;
                        membership.Person.FirstName = AvivaData[6];
                        membership.Person.FamilyName = AvivaData[7];
                        membership.Person.Address = AvivaData[8];
                        membership.Person.Zip = AvivaData[9];
                        membership.Person.City = AvivaData[10];
                        membership.Person.Email = AvivaData[11];
                        membership.Person.Mobile = AvivaData[12];
                        membership.Person.Phone = AvivaData[13];
                        membership.Note = AvivaData[14];

                        membership.Type = AvivaData[15].Contains("x") ? PersonType.Active : PersonType.Passiv;

                        if (AvivaData[16].Contains("x")) membership.AbsentDate = DateTime.Now;
                        if (AvivaData[18].Contains("x")) membership.Person.PrintNewslettet = false;

                        if (AvivaData[26].Contains("Holdleder")) membership.Teamleader = true;
                        if (AvivaData[26].Contains("Administrator")) membership.Secretary = true;

                        if (AvivaData[26].Contains("Formand")) membership.BoardFunction = BoardFunctionType.BoardMember;
                        if (AvivaData[26].Contains("Næstformand")) membership.BoardFunction = BoardFunctionType.BoardMember;
                        if (AvivaData[26].Contains("Kasserer")) membership.BoardFunction = BoardFunctionType.Accountant;
                        if (AvivaData[26].Contains("Sekretær")) membership.BoardFunction = BoardFunctionType.BoardMember;
                        if (AvivaData[26].Contains("Bestyrelses medlem")) membership.BoardFunction = BoardFunctionType.BoardMember;
                        if (AvivaData[26].Contains("Suppleant")) membership.BoardFunction = BoardFunctionType.Alternate;
                        
                        
                        if (AvivaData[26].Contains("Revisor")) membership.BoardFunction = BoardFunctionType.Auditor;
                        if (AvivaData[26].Contains("Revisor Suppleant")) membership.BoardFunction = BoardFunctionType.AuditorAlternate;

                        membership.Person.Gender = Gender.NotDefined;

                        membership.Person.Trim();

                        if (!string.IsNullOrWhiteSpace(membership.Person.FullName) &
                            !string.IsNullOrWhiteSpace(membership.Person.Mobile) &
                            !string.IsNullOrWhiteSpace(membership.Person.Email)
                            )
                        {
                            membership.Person.Country = Country.DK;
                            if (string.IsNullOrWhiteSpace(membership.Person.Phone) | membership.Person.Phone.Length < 8) membership.Person.Phone = String.Empty;
                            if (string.IsNullOrWhiteSpace(membership.Person.Mobile) | membership.Person.Mobile.Length < 8) membership.Person.Mobile = String.Empty;
                            if (membership.Person.Address.Length < 2) membership.Person.Address = "  ";
                            if (membership.Person.Address.Length > 70) membership.Person.Address = membership.Person.Address.Substring(0, 70);
                            if (membership.Person.City.Length < 2) membership.Person.City = "  ";
                            if (membership.Person.Zip.Length < 2) membership.Person.Zip = "  ";
                            if (membership.Person.FirstName.Length < 2) membership.Person.FirstName = "**";
                            if (membership.Person.FamilyName.Length < 2) membership.Person.FamilyName = "**";

                            if (membership.Person.UserName.Length > 10 || Regex.Replace(membership.Person.UserName, @"[^A-Z0-9ÆØÅ]", "") != membership.Person.UserName || WebSecurity.GetUserId(membership.Person.UserName) > 0)
                                GenerateUniqUsername(membership.Person);

                            if (!string.IsNullOrEmpty(membership.Person.Email) && !Regex.IsMatch(membership.Person.Email, @"^\S+@\S+\.\S+$"))
                                membership.Person.Email = "";
                            if (membership.Person.UserName.Length < 3) GenerateUniqUsername(membership.Person);
                            if (Regex.IsMatch(membership.Person.Password, @"^[a-zA-Z0-9æøåÆØÅ*!#%&]{6,10}$")) membership.Person.Password.GeneratePassword();

                            Memberships.Add(membership);
                            if (AvivaData[22].Contains("media/"))
                            {
                                ProfilImages.Add(membership.Person.UserName, AvivaData[22]);
                            }
                        }
                    }

                    //line = sr.ReadLine();
                }
                sr.Close();

                string HtmlData = "<table border=\"1\">";


                foreach (var ld in Data)
                {
                    HtmlData += "<tr>";
                    foreach (string str in ld)
                    {
                        HtmlData += "<td>" + str + "</td>";
                    }

                    HtmlData += "</tr>";
                }
                HtmlData += "</table>";

                using (var context = new Repository())
                {

                    Association association = context.Associations.Find(id);

                    foreach (var M in Memberships)
                    {
                        M.AssociationID = association.AssociationID;
                        context.Entry(M).State = EntityState.Added;
                        try
                        {
                            context.SaveChanges();
                            if (!M.Absent) //&& !WebSecurity.UserExists(m.Person.UserName))
                            {
                                WebSecurity.CreateAccount(M.Person.UserName, M.Person.Password);
                                //if (WebSecurity.ConfirmAccount(M.Person.UserName))
                                //{
                                //    System.Diagnostics.Debug.WriteLine("**Migrating Person: \"" + M.Person.UserName + "\" / (" + M.Person.Password + ")");
                                //}


                            }

                            string ImagePath;
                            if (ProfilImages.TryGetValue(M.Person.UserName, out ImagePath))
                            {

                                var img = DownloadImageFromUrl(string.Format("http://www.natteravnene.dk/aviva/{0}", ImagePath));

                            if (img != null)
                            {
                                string newFileName = string.Format(ProfilDirSetting, M.Person.PersonID.ToString());
                                string newFileLocation = newFileName;
                                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                                }

                                img.Save(newFileLocation);
                                //progress += " - Image transferred";
                            }
                            }

                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("{0} Exception caught.", e));


                        }



                    }
                }


                return Json(new
                           {
                               success = true,
                               data = HtmlData
                           });

            }


            return Json(new { success = false, errorMessage = "Tjaa... Fejl" });
        }

        public ActionResult NewsMigration()
        {
            return View();
        }

        public ActionResult _NewsMigration(Guid? id, string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            int newsId;
            string Result = "";
            string[] Urls = url.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);


            foreach (string cururl in Urls)
            {
                string theUrl;
                if (int.TryParse(cururl, out newsId))
                {
                    theUrl = string.Format(@"http://www.natteravnene.dk/default.asp?page_id=12&news_id={0}", newsId.ToString());
                }
                else
                {
                    theUrl = cururl;
                    if (!theUrl.ToLower().StartsWith("http://www.")) continue;
                }

                HtmlWeb web = new HtmlWeb
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.GetEncoding("iso-8859-1"),
                };  //text/html; charset=iso-8859-1
                HtmlDocument doc = web.Load(theUrl);
                var nodes = doc.DocumentNode.SelectNodes("//td[@class='frontleft']");


                if (nodes != null && nodes.Count == 1)
                {
                    //List<HtmlNode> images = new List<HtmlNode>();
                    News news = new News
                     {
                         Publish = DateTime.Now.Date.AddHours(DateTime.Now.Hour),
                         Source = LevelType.National,
                         Distribution = LevelType.National,
                         AuthorID = CurrentProfile.PersonID,
                         Teaser = "",
                         Content = ""
                     };
                    if (id != null && id != Guid.Empty)
                    {
                        news.Source = LevelType.Local;
                        news.SourceLink = (Guid)id;
                        news.Distribution = LevelType.Local;
                        news.DistributionLink = (Guid)id;
                    }


                    DateTime Date;

                    var node = nodes.First();

                    news.Headline = node.SelectSingleNode("./h2").InnerText;
                    node.SelectSingleNode("./h2").Remove();

                    HtmlNode DateNode = node.ChildNodes[4];
                    if (DateTime.TryParse(DateNode.InnerText, out Date))
                    {
                        news.Publish = Date;
                    }

                    if (node.ChildNodes[0].Name == "#text") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "p") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "#text") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "br") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "#text") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "br") node.ChildNodes[0].Remove();
                    if (node.ChildNodes[0].Name == "br") node.ChildNodes[0].Remove();
                    news.Content = node.InnerHtml;
                    foreach (HtmlNode teser in node.ChildNodes)
                    {
                        if (!string.IsNullOrWhiteSpace(teser.InnerText))
                        {
                            news.Teaser = HtmlEntity.DeEntitize(teser.InnerText);
                            break;
                        }
                    }
                    news.Trim();
                    if (!string.IsNullOrWhiteSpace(news.Content) && reposetory.Save(news))
                    {
                        int num = 0;
                        var imgNodes = node.SelectNodes(".//img/@src");
                        if (imgNodes != null)
                        {
                            foreach (HtmlNode img in imgNodes)
                            {
                                string src = img.Attributes["src"].Value;
                                if (!src.StartsWith("http://"))
                                {
                                    //img.Attributes["src"].Value = "http://www.natteravnene.dk/" + src;
                                    img.Attributes["src"].Value = SaveImageFile("http://www.natteravnene.dk/" + src, news.NewsId, num);
                                    if (node.Attributes["width"] != null) img.Attributes["width"].Remove();
                                    if (node.Attributes["height"] != null) img.Attributes["height"].Remove();
                                    img.Attributes.Add("class", "img-responsive");
                                    num++;
                                }
                                //images.Add(img);                       //.Attributes["src"].Value);
                            }
                        }

                        news.Content = node.InnerHtml;
                        if (reposetory.Save(news))
                        {
                            Result += "<hr />" + node.InnerHtml;
                        }
                    }
                }


            }



            return Json(new
            {
                success = true,
                data = Result
            });
        }

        public ActionResult LegacyData()
        {
            List<SelectListItem> tt = reposetory.GetAssociations().Select(A => new SelectListItem { Value = A.AssociationID.ToString(), Text = A.Name }).OrderBy(S => S.Text).ToList();
            return View(tt);
        }

        public ActionResult ExportData(Guid ForningID)
        {
            Association Ass = reposetory.GetAssociation(ForningID);
            if (Ass != null)
            {
                var data = reposetory.GetAssociationPersons(ForningID).Select(A => new { PersonID = A.Person.PersonID, Membership = A.MembershipID,  UserName = A.Person.UserName, Navn = A.Person.FullName,  Turer = "", StartetDato = A.SignupDate.ToString("dd-MM-yyy") });
            GridView gv = new GridView();
            gv.DataSource = data;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + Ass.Name + "_LegacyData.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            }

            return RedirectToAction("LegacyData");
        }

        public ActionResult ImportData(HttpPostedFileBase file)
        {
            List<SelectListItem> tt = reposetory.GetAssociations().Select(A => new SelectListItem { Value = A.AssociationID.ToString(), Text = A.Name }).OrderBy(S => S.Text).ToList();
            if (file != null)
            {
                string line;
                StreamReader sr = new StreamReader(file.InputStream, Encoding.GetEncoding(1252));
                string header = sr.ReadLine();
                if (!header.StartsWith(@"PersonID;Membership;UserName;Navn;Turer;StartetDato")) return View("LegacyData", tt);

                while ((line = sr.ReadLine()) != null)
                {
                    var AvivaData = line.Split(';');
                    for (int t = 0; t < AvivaData.Length; t++)
                    {
                        AvivaData[t] = AvivaData[t].TrimStart('"').TrimEnd('"');
                    }
                    Guid PersonID;
                    Guid MembershipID;
                    if (Guid.TryParse( AvivaData[0], out PersonID) & Guid.TryParse( AvivaData[1], out MembershipID))
                    {
                        //Person P = reposetory.GetPerson(PersonID);
                        NRMembership M = reposetory.GetMembership(MembershipID);
                        if (M != null && (M.Person.UserName == AvivaData[2] & M.PersonID == PersonID))
                        {
                            int LegacyTure;
                            if (int.TryParse(AvivaData[4], out LegacyTure)) M.Person.DeltaActivity = LegacyTure;
                            DateTime Signup;
                            if (DateTime.TryParse(AvivaData[5], out Signup)) M.SignupDate = Signup;
                            reposetory.SavePerson(M);
                        }
                    }

                }
            }
            
            return View("LegacyData", tt);
        }

        //***********************************************************************************

        public static Guid MigrateAssociation(int id)
        {
            Guid assID = Guid.Empty;
            Dictionary<string, DateTime> PersonCreated = new Dictionary<string, DateTime>();
            Dictionary<string, Guid> PersonGuid = new Dictionary<string, Guid>();


            using (var context = new Repository())
            {
                StringBuilder SQLIdentity = new StringBuilder();
                Association ass = ExtractAssociation(id);
                System.Diagnostics.Debug.WriteLine("Start migrating Association: \"" + ass.Name + "\"");
                progress += String.Format("<br/>Start migrating Association: \"{0}\"", ass.Name.ToString());

                DateTime Created = ass.Created;

                SeedData rec = Seed.Find(n => n.Name.ToLower() == ass.Name.ToLower());
                if (rec != null)
                {
                    ass.Network = context.Networks.Where(n => n.NetworkNumber == rec.Network).FirstOrDefault();
                    ass.Established = rec.EstablishedDT;
                    if (!String.IsNullOrWhiteSpace(rec.NewName)) ass.Name = rec.NewName;
                    ass.Number = rec.Number;
                    ass.Governance = rec.Governance;
                }
                else
                {
                    progress += String.Format("<br/><span style=\"color: red\">No Migration Default for Association: \"{0}\"</span>", ass.Name.ToString());
                    ass.Network = context.Networks.Where(n => n.NetworkNumber == 99).FirstOrDefault();
                }

                foreach (NRMembership m in ass.Memberships)
                {
                    PersonCreated.Add(m.Person.UserName, m.Person.Created);
                    PersonGuid.Add(m.Person.UserName, m.Person.PersonID);
                    var tmpPerson = new Person
                    {
                        UserName = m.Person.UserName,
                        FirstName = "Test",
                        FamilyName = "Test",
                        Address = "Test",
                        City = "Test",
                        Zip = "Test"
                    };
                    context.Entry(tmpPerson).State = EntityState.Added;

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("{0} Exception caught.", e));
                    }

                    StringBuilder SQLIdentityPerson = new StringBuilder();
                    Created = (DateTime)PersonCreated[m.Person.UserName.ToString()];
                    Guid Gid = (Guid)PersonGuid[m.Person.UserName.ToString()];
                    SQLIdentityPerson.AppendLine(string.Format("UPDATE [People] SET Created = '{0}' WHERE Username = '{1}';", Created.ToString("yyyy-MM-dd hh:mm:ss.mmm"), m.Person.UserName.ToString()));
                    SQLIdentityPerson.AppendLine(string.Format("UPDATE [People] SET PersonID = '{0}' WHERE Username = '{1}';", Gid.ToString(), m.Person.UserName.ToString()));
                    context.Database.ExecuteSqlCommand(SQLIdentityPerson.ToString());

                    if (rec != null)
                    {
                        if (m.Person.UserName == rec.Chairmann)
                        { m.BoardFunction = BoardFunctionType.Chairman; }
                        else if (m.Person.UserName == rec.Accountant)
                        { m.BoardFunction = BoardFunctionType.Accountant; }
                        else if (m.Person.UserName == rec.Auditor)
                        { m.BoardFunction = BoardFunctionType.Auditor; }
                        else if (m.Person.UserName == rec.AuditorAlternate)
                        { m.BoardFunction = BoardFunctionType.AuditorAlternate; }
                        else if (rec.BoardMembers != null && rec.BoardMembers.Contains(m.Person.UserName))
                        { m.BoardFunction = BoardFunctionType.BoardMember; }
                        else if (rec.Alternate != null && rec.Alternate.Contains(m.Person.UserName))
                        { m.BoardFunction = BoardFunctionType.Alternate; }
                    }

                    context.Entry(m.Person).State = EntityState.Modified;
                    System.Diagnostics.Debug.WriteLine("  - (" + ass.Name + ") [" + m.Person.FullName + "]");
                    progress += String.Format("<br/> - ({0}) [{1}]", ass.Name.ToString(), m.Person.FullName);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("  - Error Migrating (" + ass.Name + ") [" + m.Person.FullName + "]");
                        System.Diagnostics.Debug.WriteLine(string.Format("{0} Exception caught.", e));
                    }

                    if (!m.Absent) //&& !WebSecurity.UserExists(m.Person.UserName))
                    {
                        WebSecurity.CreateAccount(m.Person.UserName, m.Person.Password);
                        if (WebSecurity.ConfirmAccount(m.Person.UserName))
                        {
                            System.Diagnostics.Debug.WriteLine("**Migrating Person: \"" + m.Person.UserName + "\" / (" + m.Person.Password + ")");
                        }

                        var img = DownloadImageFromUrl(string.Format("http://www.ravnetur.dk/Natteravnebilleder/{0}.jpg", m.Person.PersonID));

                        if (img != null)
                        {
                            string newFileName = string.Format(ProfilDirSetting, m.Person.PersonID.ToString());
                            string newFileLocation = newFileName;
                            if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                            }

                            img.Save(newFileLocation);
                            progress += " - Image transferred";
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("  - Saving (" + ass.Name + ")");
                context.Entry(ass).State = EntityState.Added;

                try
                {
                    context.SaveChanges();
                    assID = ass.AssociationID;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("  - Error saving (" + ass.Name + ")");
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} Exception caught.", e));
                    System.Diagnostics.Debug.WriteLine("-------------------------------------------------------------------------------------------");
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} Exception caught.", e.InnerException));


                }

                context.Memberships.Where(m => m.Association.AssociationID == ass.AssociationID)
                    .ToList()
                    .ForEach(p => p.Person.CurrentAssociation = p.Association.AssociationID);
                context.SaveChanges();


                //SQLIdentity.AppendLine("SET IDENTITY_INSERT [Associations] ON;");
                SQLIdentity.AppendLine(string.Format("UPDATE [Associations] SET Created = '{0}' WHERE AssociationID = '{1}';", Created.ToString("yyyy-MM-dd hh:mm:ss.mmm"), ass.AssociationID.ToString()));
                //SQLIdentity.AppendLine("SET IDENTITY_INSERT [Associations] OFF;");


                context.Database.ExecuteSqlCommand(SQLIdentity.ToString());



            }
            return assID;
        }

        public static Association ExtractAssociation(int id)
        {

            Dictionary<int, Guid> Assdictionary = new Dictionary<int, Guid>();
            Dictionary<string, string> UserNamedictionary = new Dictionary<string, string>();
            Dictionary<int, String> Locationdictionary = new Dictionary<int, String>();


            Random random = new Random();

            Association association = null;

            //Get Association
            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Foreninger] WHERE ForeningsID = " + id.ToString(), cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while ((reader.Read()))
                {
                    association = ConvertAssociation(reader);
                    association.UseSchedulePlanning = true;
                    association.PageSponsor = new NR.Models.Content { Title = DefaultForening.PageContentSponsorTitle, Body = DefaultForening.PageContentSponsor };
                    association.PageAbout = new NR.Models.Content { Title = DefaultForening.PageContentAboutTitle, Body = DefaultForening.PageContentAbout };
                    association.PagePress = new NR.Models.Content { Title = DefaultForening.PageContentPressTitle, Body = DefaultForening.PageContentPress };
                    association.PageLink = new NR.Models.Content { Title = DefaultForening.PageContentLinkTitle, Body = DefaultForening.PageContentLink };
                    Assdictionary.Add((int)reader["ForeningsID"], association.AssociationID);


                }
            }

            //Get persons
            association.Memberships = new List<NRMembership>();
            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT
                        [Ravnetur].[dbo].[aspnet_Users].[UserId]
                        ,[UserName]
                        ,[Email]
                        ,[Password]
	                    ,[PropertyNames]
                        ,[PropertyValuesString]
                        ,[LastActivityDate]
                        ,[CreateDate]
                        ,(SELECT CAST(1 AS BIT) AS Expr1
							FROM [Ravnetur].[dbo].[aspnet_UsersInRoles]
							WHERE ([Ravnetur].[dbo].[aspnet_UsersInRoles].UserId = [Ravnetur].[dbo].[aspnet_Users].UserId
									AND [Ravnetur].[dbo].[aspnet_UsersInRoles].RoleId = 'ED1CAE67-880D-4EC0-A00F-81DD0A191749')) as [Planner]	
                        FROM [Ravnetur].[dbo].[aspnet_Users]
                        LEFT JOIN [Ravnetur].[dbo].[aspnet_Profile]
                        ON [Ravnetur].[dbo].[aspnet_Users].UserId = [Ravnetur].[dbo].[aspnet_Profile].UserId
                        LEFT JOIN [Ravnetur].[dbo].[aspnet_Membership]
                        ON [Ravnetur].[dbo].[aspnet_Users].UserId = [Ravnetur].[dbo].[aspnet_Membership].UserId;", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while ((reader.Read()))
                {
                    if (reader["PropertyValuesString"] != null & !Convert.IsDBNull(reader["PropertyValuesString"]))
                    {
                        string StrId = ReturnProperty("ForeningsID", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
                        if (StrId != String.Empty && Convert.ToInt32(StrId) == id)
                        {
                            Person person = ConvertPerson(reader, association.Country);

                            List<NRMembership> ms = association.Memberships.ToList();

                            var ExistUser = from UN in ms
                                            where UN.Person.UserName == person.UserName
                                            select UN;

                            if (ExistUser.Any())
                            {

                                char ch;
                                for (int i = 0; i < 2; i++)
                                {
                                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                                    person.UserName = person.UserName + ch.ToString();
                                }
                            }

                            UserNamedictionary.Add((String)reader["UserName"], person.UserName);

                            NRMembership membership = new NRMembership();

                            if (ReturnProperty("Udmeldt", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]) == "True") membership.AbsentDate = DateTime.Now;
                            membership.Teamleader = ReturnProperty("TurLeder", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]) == "True" ? true : false;
                            membership.BoardFunction = BoardFunctionType.none;
                            membership.SignupDate = DateTime.Now;
                            membership.Type = PersonType.Active;
                            membership.SignupDate = person.Created;

                            membership.Person = person;

                            //membership.Association = association;

                            if (reader["Planner"] != null & !Convert.IsDBNull(reader["Planner"])) membership.Planner = true;


                            //person.Memberships.Add(membership);
                            association.Memberships.Add(membership);


                            //person.CurrentAssociation = association.AssociationID;
                            //association.People.Add(person);
                        }
                    }
                }
            }

            //Get locations
            association.Locations = new List<Location>();
            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [Lokationer] WHERE ForeningsID = " + id.ToString(), cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while ((reader.Read()))
                {
                    Location location = ConvertLocation(reader);
                    if (!String.IsNullOrWhiteSpace(location.Name.Replace(".", "")))
                    {
                        association.Locations.Add(location);
                        Locationdictionary.Add((int)TjekDBNull(reader["LokID"]), location.ShortName);
                    }
                }
            }

            if (!association.Locations.Any())
            {
                Location location = new Location
                {
                    Name = association.Name,
                    ShortName = "STD",
                    Description = "Standard lokationen for forningen"
                };
                association.Locations.Add(location);
            }

            //Get Folders
            association.Folders = new List<Folder>();
            using (SqlConnection cn = new SqlConnection(seedConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [TurMapper] WHERE ForeningsID = " + id.ToString(), cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while ((reader.Read()))
                {
                    Folder folder = ConvertFolder(reader);
                    folder.Start = folder.Start.Date;
                    folder.Finish = folder.Finish.Date.AddDays(1).AddSeconds(-1);

                    int TurmappeID = (int)TjekDBNull(reader["TurMappeID"]);
                    folder.Teams = new List<Team>();
                    using (SqlConnection cn1 = new SqlConnection(seedConnectionString))
                    {
                        SqlCommand cmd1 = new SqlCommand("SELECT * FROM [Turer] WHERE TurmappeID = " + TurmappeID.ToString(), cn1);
                        cmd1.CommandType = CommandType.Text;
                        cn1.Open();
                        var reader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while ((reader1.Read()))
                        {
                            Team team = new Team
                            {
                                Start = (DateTime)reader1["Start"],
                                Note = (String)TjekDBNull(reader1["Note"]),
                                Created = (DateTime)reader1["Oprettet"],
                                Lastchanged = (DateTime)reader1["SidstRettet"],
                                Association = association,
                                Teammembers = new List<Person>()
                            };

                            if (String.IsNullOrWhiteSpace(team.Note)) team.Note = null;
                            string Dur = (String)reader1["Tid"];
                            TimeSpan DurT;
                            if (TimeSpan.TryParse(Dur, out DurT))
                            {
                                team.Duration = DurT;
                            }

                            switch ((String)reader1["Status"])
                            {
                                case "OK":
                                    team.Status = TeamStatus.OK;
                                    break;
                                case "Aflyst":
                                    team.Status = TeamStatus.Cancelled;
                                    break;
                                case "Nedlagt":
                                    team.Status = TeamStatus.Droped;
                                    break;
                                default:
                                    team.Status = TeamStatus.Planned;
                                    break;
                            }
                            //UserNamedictionary
                            String UserName;
                            if (UserNamedictionary.TryGetValue((String)reader1["TurlederID"], out UserName))
                            {
                                Person TL = association.Memberships.Where(m => m.Person.UserName == UserName).Select(m => m.Person).FirstOrDefault();
                                team.Teammembers.Add(TL);
                                team.TeamLeaderId = TL.PersonID;
                            }
                            if (UserNamedictionary.TryGetValue((String)reader1["Ravn1ID"], out UserName))
                            {
                                team.Teammembers.Add(association.Memberships.Where(m => m.Person.UserName == UserName).Select(m => m.Person).FirstOrDefault());
                            }
                            if (UserNamedictionary.TryGetValue((String)reader1["Ravn2ID"], out UserName))
                            {
                                team.Teammembers.Add(association.Memberships.Where(m => m.Person.UserName == UserName).Select(m => m.Person).FirstOrDefault());
                            }
                            else if ((String)reader1["Ravn2ID"] == "PRØVETUR")
                            {
                                team.Trial = true;
                            }
                            string Loc;
                            if (Locationdictionary.Any() && TjekDBNull(reader1["Lokation"]) != null && Locationdictionary.TryGetValue((int)reader1["Lokation"], out Loc))
                            {
                                team.Location = association.Locations.Where(l => l.ShortName == Loc).FirstOrDefault();
                            }
                            else
                            {
                                team.Location = association.Locations.Where(l => l.ShortName == "STD").FirstOrDefault();
                            }
                            folder.Teams.Add(team);
                        }
                    }

                    association.Folders.Add(folder);
                }
            }
            return association;
        }

        public static Association ConvertAssociation(SqlDataReader reader)
        {
            Association association = new Association
            {
                Name = (String)reader["Navn"],
                TeamPhone = ((String)TjekDBNull(reader["Turtelefon"])),

                TeamMessage = (String)TjekDBNull(reader["TurbeskedRavne"]),
                TeamMessageTeamLeader = (String)TjekDBNull(reader["TurbeskedTurleder"]),
                SendTeamTextDays = (int)TjekDBNull(reader["TurSMSdage"]),
                SendNoteTeamleader = (Boolean)TjekDBNull(reader["SendNoteTurleder"]),
                SendTeamText = (Boolean)TjekDBNull(reader["SendturSMS"]),
                NoteTextTime = (int)TjekDBNull(reader["NoteSMStimer"]),
                Scheduletext = (String)TjekDBNull(reader["Turlistetext"]),

                UseLists = (Boolean)TjekDBNull(reader["Lister"]),
                UsePolicePlanning = (Boolean)TjekDBNull(reader["Politi"]),

                UseKeyBox = (Boolean)TjekDBNull(reader["KeyBox"]),
                KeyBoxcode = (String)TjekDBNull(reader["KeyBoxcode"]),

                TextServiceProviderUserName = (String)TjekDBNull(reader["SMSUserName"]),
                TextServiceProviderPassword = (String)TjekDBNull(reader["SMSPassword"]),
                Created = (DateTime)reader["Oprettet"],
                Lastchanged = (DateTime)reader["SidstRettet"]
            };
            association.Trim();
            if (association.Name.Length < 2) association.Name += "  ";
            if (association.Scheduletext.Length < 10) association.Scheduletext = "          ";
            //if (association.Country == String.Empty) association.Country = Country.DK;
            Country tmpCountry;
            if (Enum.TryParse((String)reader["Land"], true, out tmpCountry))
            {
                association.Country = tmpCountry;
            }
            else
            {
                association.Country = Country.DK;
            }

            if (association.TeamMessage.Length < 10) association.TeamMessage = "          ";
            if (association.TeamMessageTeamLeader.Length < 10) association.TeamMessageTeamLeader = "          ";
            //association.URL = new List<string> { association.Name.ValidFileName() };

            return association;
        }

        public static Person ConvertPerson(SqlDataReader reader, Country country)
        {
            var personen = new Person();
            personen.PersonID = (Guid)reader["UserId"];
            personen.Email = reader["Email"] == DBNull.Value ? "" : ((String)reader["Email"]);
            personen.UserName = ((String)reader["UserName"]);
            personen.Password = reader["Password"] == DBNull.Value ? "" : (String)reader["Password"];
            personen.FirstName = ReturnProperty("Fornavn", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.FamilyName = ReturnProperty("Efternavn", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.Address = ReturnProperty("Vej", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.City = ReturnProperty("By", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.Zip = ReturnProperty("Postnr", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.Phone = ReturnProperty("Telefon", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.Mobile = ReturnProperty("Mobil", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            String Koen = ReturnProperty("gender", (string)reader["PropertyNames"], (string)reader["PropertyValuesString"]);
            personen.Created = (DateTime)reader["CreateDate"];

            personen.Gender = Gender.NotDefined;
            if (Koen == "K") personen.Gender = Gender.Woman;
            if (Koen == "M") personen.Gender = Gender.Man;
            personen.Trim();

            personen.Country = country;
            if (string.IsNullOrWhiteSpace(personen.Phone) | personen.Phone.Length < 8) personen.Phone = String.Empty;
            if (string.IsNullOrWhiteSpace(personen.Mobile) | personen.Mobile.Length < 8) personen.Mobile = String.Empty;
            if (personen.Address.Length < 2) personen.Address = "  ";
            if (personen.Address.Length > 70) personen.Address = personen.Address.Substring(0, 70);
            if (personen.City.Length < 2) personen.City = "  ";
            if (personen.Zip.Length < 2) personen.Zip = "  ";
            if (personen.FirstName.Length < 2) personen.FirstName = "**";
            if (personen.FamilyName.Length < 2) personen.FamilyName = "**";

            if (personen.UserName.Length > 10 || Regex.Replace(personen.UserName, @"[^A-Z0-9ÆØÅ]", "") != personen.UserName || WebSecurity.GetUserId(personen.UserName) > 0)
                GenerateUniqUsername(personen);

            if (!string.IsNullOrEmpty(personen.Email) && !Regex.IsMatch(personen.Email, @"^(['\""]{1,}.+['\""]{1,}\s+)?<?[\w\.\-]+@([\w\-]+\.)+[A-Za-z]{2,}>?$"))
                personen.Email = "";

            var results = new List<ValidationResult>();
            var context = new ValidationContext(personen, null, null);
            if (!Validator.TryValidateObject(personen, context, results))
            {
                foreach (ValidationResult res in results)
                {
                    System.Diagnostics.Debug.WriteLine("ModelFaileur: " + res.ErrorMessage);

                }
            }

            return personen;
        }

        public static Location ConvertLocation(SqlDataReader reader)
        {

            Location location = new Location
            {
                Name = (String)reader["Navn"],
                ShortName = (String)reader["KortNavn"],
                Description = (String)TjekDBNull(reader["Beskrivelse"]),
                Created = (DateTime)reader["Oprettet"],
                Lastchanged = (DateTime)reader["SidstRettet"]
            };
            if (location.Name.Length > 25) location.Name = location.Name.Substring(0, 25);


            return location.Trim();
        }

        public static Folder ConvertFolder(SqlDataReader reader)
        {
            Folder folder = new Folder
            {
                FoldereName = (String)reader["TurMappeNavn"],
                Start = (DateTime)reader["Start"],
                Finish = (DateTime)reader["Slut"],
                Created = (DateTime)reader["Oprettet"],
                Lastchanged = (DateTime)reader["SidstRettet"]
            };
            if ((Boolean)TjekDBNull(reader["Aaben"])) folder.Status = FolderStatus.Input;
            if ((Boolean)TjekDBNull(reader["Publiceret"])) folder.Status = FolderStatus.Published;
            return folder;
        }

        private static string ReturnProperty(string property, string PropertyNames, string PropertyValuesString)
        {
            if (PropertyValuesString == null) return string.Empty;

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

        private static object TjekDBNull(object valuebool)
        {
            if (valuebool == null | Convert.IsDBNull(valuebool))
            { return null; }
            else
            { return valuebool; }
        }

        public static void GenerateUniqUsername(Person person)
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


            } while (WebSecurity.GetUserId(UserName) > 0);

            person.UserName = UserName;

        }

        public static System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }


        //private string GetWebsiteHtml(string url)
        //{
        //    WebRequest request = WebRequest.Create(url);
        //    WebResponse response = request.GetResponse();
        //    Stream stream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(stream);
        //    string result = reader.ReadToEnd();
        //    stream.Dispose();
        //    reader.Dispose();
        //    return result;
        //}

        private string SaveImageFile(string imageUrl, Guid id, int num)
        {
            // Define destination
            string folderName = Url.Content(ConfigurationManager.AppSettings["NewsImage"]);

            if (string.IsNullOrWhiteSpace(folderName)) { throw new ArgumentNullException(); }

            string FileUrl = num == 0 ? string.Format(folderName, id.ToString()) : string.Format(folderName, id.ToString() + "_" + num.ToString());
            string fullFileName = HttpContext.Server.MapPath(FileUrl);
            var serverPath = Path.GetDirectoryName(fullFileName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            //var fileName = id.ToString() + ".jpg";                                //Path.GetFileName(file.FileName);
            //fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);
            WebImage img;
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                img = new WebImage(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }






            int width = 1280;
            int height = 750;
            int minheight = 400;

            double ratio = (double)img.Width / img.Height;
            double desiredRatio = (double)width / height;

            if (ratio > desiredRatio)
            {
                height = Convert.ToInt32(width / ratio);
                if (height < minheight)
                {
                    int delta = Convert.ToInt32((minheight * ratio - img.Width) / 2);
                    img.Crop(0, delta, 0, delta);
                }
                //width = Convert.ToInt32(height * ratio);
            }
            if (ratio < desiredRatio)
            {
                int delta = Convert.ToInt32((img.Height - img.Width / desiredRatio) / 2);
                img.Crop(delta, 0, delta, 0);
            }


            img.Resize(width, height, true, true);





            //img.Resize(400, (int)(400 * ratio)); // ToDo - Change the value of the width of the image oin the screen
            //string fullFileName = string.Format(serverPath, id.ToString());

            if (System.IO.File.Exists(fullFileName))
                System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return FileUrl;
        }

        

    }




}