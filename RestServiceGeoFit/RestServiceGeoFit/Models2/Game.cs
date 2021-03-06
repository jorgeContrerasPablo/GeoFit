namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Game")]
    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            FeedBacks = new HashSet<FeedBack>();
            Players = new HashSet<Player>();
            Notices = new HashSet<Notice>();
        }

        public int GameID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PlayersNum { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public int? Team1ID { get; set; }

        public int? Team2ID { get; set; }

        public int? PlaceID { get; set; }

        public int? CreatorID { get; set; }

        [Required]
        public int SportId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        public virtual Place Place { get; set; }

        public virtual Team Team { get; set; }

        public virtual Team Team1 { get; set; }

        public virtual Player Creator { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Player> Players { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Notice> Notices { get; set; }
    }
}
