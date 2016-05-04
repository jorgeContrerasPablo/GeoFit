namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Place")]
    public partial class Place
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Place()
        {
            FeedBacks = new HashSet<FeedBack>();
            Games = new HashSet<Game>();
            Teams = new HashSet<Team>();
        }

        public int PlaceID { get; set; }

        [Required]
        public DbGeography Coordinates { get; set; }

        [Required]
        [StringLength(100)]
        public string Direction { get; set; }

        [Required]
        [StringLength(20)]
        public string PlaceName { get; set; }

        public int? PhoneNum { get; set; }

        [Required]
        [StringLength(50)]
        public string PlaceMail { get; set; }

        public Guid? PhotoID { get; set; }

        public double? ValuationMed { get; set; }

        [Required]
        [StringLength(50)]
        public string Link { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Game> Games { get; set; }

        public virtual Photo Photo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Team> Teams { get; set; }
    }
}
