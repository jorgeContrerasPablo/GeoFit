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

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Direction { get; set; }

        public string PlaceName { get; set; }

        public int? PhoneNum { get; set; }

        public string PlaceMail { get; set; }

        public Guid? PhotoID { get; set; }

        public double? ValuationMed { get; set; }

        public string Link { get; set; }

        public int? SportId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Game> Games { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Team> Teams { get; set; }

        public override String ToString()
        {
            return PlaceName;
        }

        public override int GetHashCode()
        {
            return PlaceID;
        }
    }
}
