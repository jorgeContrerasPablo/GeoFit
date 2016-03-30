using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class Place
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Place()
        {
            FeedBacks = new HashSet<FeedBack>();
            Games = new HashSet<Game>();
            Teams = new HashSet<Team>();
        }

        public int PlaceID { get; set; }

      //  public DbGeography Coordinates { get; set; }

        public string Direction { get; set; }

        public string PlaceName { get; set; }

        public int? PhoneNum { get; set; }

        public string PlaceMail { get; set; }

        public Guid? PhotoID { get; set; }

        public double? ValuationMed { get; set; }

        public string Link { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Game> Games { get; set; }

        public virtual Photo Photo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Teams { get; set; }
    }
}
