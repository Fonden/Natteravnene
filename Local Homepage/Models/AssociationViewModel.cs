using NR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local_Homepage.Models
{
    public class AssociationViewModel
    {

        public Guid AssociationID { get; set; }

        public Guid NetworkID { get; set; }

        public string AssociationName { get; set; }

        public string urlCanonical { get; set;}

        public string Host { get; set; }

        public string RequestHost { get; set; }

        public bool MainUrl { get; set; }

        public DateTime Established { get; set; }

        public string AssociationEmail { get; set; }

        public string CVRNR { get; set; }

        public string ContactPhone { get; set;}

        public Content Content { get; set; }

        public bool UseSponsorPage { get; set; }

        public bool UsePressPage { get; set; }

        public bool UseLinksPage { get; set; }


        public Content PageAbout { get; set; }

        public Content PagePress { get; set; }

        public Content PageSponsor { get; set; }

        public Content PageLink { get; set; }

        public IList<Sponsor> Sponsors { get; set; }

        public List<News> News { get; set; }

        public List<Event> Events { get; set; }

        public Event Event { get; set; }

        public News theNews { get; set; }

        public Person Chairmann { get; set; }

        public Person Accountant { get; set; }

        public List<Person> BoardMembers { get; set; }



        public bool SEO { get; set; }



        public string AssociationNameGenitive
        {
            get
            {
                return string.Format("{0}s", AssociationName);
            }
        }

    }
}