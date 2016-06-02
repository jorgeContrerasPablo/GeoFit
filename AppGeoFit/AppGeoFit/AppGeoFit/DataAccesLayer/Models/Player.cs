using System;
using System.Collections.Generic;
using System.Text;

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
            NoticesMessege = new HashSet<Notice>();
            NoticesRecive = new HashSet<Notice>();
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


        public override String ToString()
        {
            return PlayerNick;
        }

        public override bool Equals(object obj)
        {
            var player = obj as Player;

            if (player == null)
            {
                return false;
            }
            return PlayerId == player.PlayerId 
                && Password.Equals(player.Password)
                && PlayerNick.Equals(player.PlayerNick)
                && PlayerName.Equals(player.PlayerName)
                && PhoneNum == player.PhoneNum
                && PlayerMail.Equals(player.PlayerMail)
                && FavoriteSportID == player.FavoriteSportID
                && Level == player.Level
                && MedOnTime == player.MedOnTime;
            
        }

    }
}
