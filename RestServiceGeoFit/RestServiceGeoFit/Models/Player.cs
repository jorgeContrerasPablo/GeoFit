using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RestServiceGeoFit.Models
{
    public class Player
    {
        [Key]
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PlayerNick { get; set; }

        [Required]
        public string PlayerName { get; set; }

        public string LastName { get; set; }

        [Required]
        public int PhoneNum { get; set; }

        [Required]
        public string PlayerMail { get; set; }

        //public Guid PhotoId { get; set; }

        public double Level { get; set; }

        public double MedOnTime { get; set; }

       // public int FavoriteSport { get; set; }

        //public ICollection<Game> Games { get; set; }

        //public ICollection<Team> Teams { get; set; }


    }
}