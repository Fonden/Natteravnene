using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NR.Localication;
using System.Globalization;

namespace NR.Models
{
    [Table("Locations")]
    public class Location
    {
        #region Primitive Properties

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LocationID { get; set; }

        [Display(Name = "LocationName", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Display(Name = "ShortName", ResourceType = typeof(DomainStrings))]
        [Required]
        [MaxLength(3)]
        public string ShortName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(DomainStrings))]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "LocationActive", ResourceType = typeof(DomainStrings))]
        public bool Active { get; set; }

        [Display(Name = "LocationShowNationaMap", ResourceType = typeof(DomainStrings))]
        public bool ShowNationalMap { get; set; }

        [Display(Name = "lng", ResourceType = typeof(DomainStrings))]
        public Nullable<double> Lng { get; set; }

        [Display(Name = "lat", ResourceType = typeof(DomainStrings))]
        public Nullable<double> Lat { get; set; }

        [NotMapped]
        public string latstring
        {
            get
            {
                if (Lat == null) { return "55.6669761"; }
                else
                {
                    double tmp = (double)Lat;
                    return tmp.ToString(new CultureInfo("en-US"));
                }
            }
            set
            {
                double tmpLat = 0;
                if (!double.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"), out tmpLat)) double.TryParse(value, NumberStyles.Any, new CultureInfo("da-DK"), out tmpLat);
                Lat = tmpLat;
            }
        }

        [NotMapped]
        public string lngstring
        {
            get
            {
                if (Lng == null) { return "12.5333127"; }
                else
                {
                    double tmp = (double)Lng;
                    return tmp.ToString(new CultureInfo("en-US"));
                }
            }
            set
            {
                double tmpLng = 0;
                if (!double.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"), out tmpLng)) double.TryParse(value, NumberStyles.Any, new CultureInfo("da-DK"), out tmpLng);
                Lng = tmpLng;
            }
        }

        [Display(Name = "Lastchanged", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Lastchanged { get; set; }

        [Display(Name = "Created", ResourceType = typeof(DomainStrings))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Created { get; set; }

        #endregion

        #region Navigation Properties

        public Association association { get; set; }

        #endregion
    }
}
