/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/

using NR.Localication;
using NR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DTA;

namespace NR.Models
{
    public class PrintMailingModel
    {

        public PrintMailingModel() { }

        public PrintMailingModel(Person P)
        {
            ID = P.PersonID.ToString().Replace("-","");
            UserName = P.UserName.Trim(); 
            FirstName = P.FirstName.TrimAndReduce();
            FamilyName = P.FamilyName.TrimAndReduce();
            Address = P.Address.TrimAndReduce();
            Zip = P.Zip.TrimAndReduce();
            City = P.City.TrimAndReduce();
            Country = P.Country;

        }

        public string ID { get; set; }

        [Display(Name = "Username", ResourceType = typeof(DomainStrings))]
        public string UserName { get; set; }
        
        [Display(Name = "FirstName", ResourceType = typeof(DomainStrings))]
        public string FirstName { get; set; }

        [Display(Name = "FamilyName", ResourceType = typeof(DomainStrings))]
        public string FamilyName { get; set; }


        [Display(Name = "FullName", ResourceType = typeof(DomainStrings))]
        public string FullName
        {
            get
            {
                return FirstName + " " + FamilyName;
            }
        }

        public string MailName
        {
            get
            {
                if (Names == null || Names.Count() == 1) return FirstName + " " + FamilyName;
                string result = "";
                foreach (var group in Names.GroupBy(x => x.FamilyName).Select(x => x))
                {
                    string subResultFirstName = "";
                    string subResultFamilyName = "";
                    foreach (NameModel N in group)
                    {
                        subResultFamilyName = N.FamilyName;
                        if (!string.IsNullOrWhiteSpace(subResultFirstName)) subResultFirstName += " og ";
                        subResultFirstName += N.FirstName;

                    }
                    result += subResultFirstName +" " + subResultFamilyName + ", ";
                }

                return result.TrimEnd(',',' ');
            }
        }

        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string FullAddress
        {
            get
            {
                return Address + ", " + Zip + " " + City;
            }
        }

        public string SortAddress
        {
            get
            {
                string result =   Address == null ? "" : Address.ToLower();

                result += ", ";
                result += Zip == null ? "" : Zip.ToLower();
                return result;
            }
        }


        [Display(Name = "Address", ResourceType = typeof(DomainStrings))]
        public string Address { get; set; }

        [Display(Name = "Zip", ResourceType = typeof(DomainStrings))]
        public string Zip { get; set; }

        [Display(Name = "City", ResourceType = typeof(DomainStrings))]
        public string City { get; set; }

        [Display(Name = "Country", ResourceType = typeof(DomainStrings))]
        public Country Country { get; set; }

        public IList<NameModel> Names { get; set; }

    }

    public class NameModel
    {
        [Display(Name = "FirstName", ResourceType = typeof(DomainStrings))]
        public string FirstName { get; set; }

        [Display(Name = "FamilyName", ResourceType = typeof(DomainStrings))]
        public string FamilyName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + FamilyName;
            }
        }

    }
}