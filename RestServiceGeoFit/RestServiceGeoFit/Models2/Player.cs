namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Player")]
    public partial class Player
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Player()
        {
            FeedBacks = new HashSet<FeedBack>();
            Joineds = new HashSet<Joined>();
            Games = new HashSet<Game>();
            NoticesMessege = new HashSet<Notice>();
            NoticesRecive = new HashSet<Notice>();
            GamesCreated = new HashSet<Game>();
            FeedBacksCreated = new HashSet<FeedBack>();
        }

        public int PlayerID { get; set; }

        [Required]
        [StringLength(60)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string PlayerNick { get; set; }

        [Required]
        [StringLength(50)]
        public string PlayerName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public int PhoneNum { get; set; }

        [Required]
        [StringLength(50)]
        public string PlayerMail { get; set; }

        public Guid? PhotoID { get; set; }

        public double? Level { get; set; }

        public double? MedOnTime { get; set; }

        public int? FavoriteSportID { get; set; }

        public bool PlayerSesion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Joined> Joineds { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
       
        public ICollection<Game> Games { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Notice> NoticesMessege { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Notice> NoticesRecive { get; set; }
        [InverseProperty("Creator")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Game> GamesCreated { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<FeedBack> FeedBacksCreated { get; set; }
    }
}
