﻿using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class Player
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Player()
        {
            FeedBacks = new HashSet<FeedBack>();
            Joineds = new HashSet<Joined>();
            Games = new HashSet<Game>();
        }

        public int PlayerId { get; set; }

        public string Password { get; set; }

        public string PlayerNick { get; set; }

        public string PlayerName { get; set; }

        public string LastName { get; set; }

        public int PhoneNum { get; set; }

        public string PlayerMail { get; set; }

        public Guid? PhotoID { get; set; }

        public double? Level { get; set; }

        public double? MedOnTime { get; set; }

        public int? FavoriteSportID { get; set; }

        public bool PlayerSesion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FeedBack> FeedBacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Joined> Joineds { get; set; }

        public virtual Photo Photo { get; set; }

        public virtual Sport Sport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Game> Games { get; set; }

    }
}
