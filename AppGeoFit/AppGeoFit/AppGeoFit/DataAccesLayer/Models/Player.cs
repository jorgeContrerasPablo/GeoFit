using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.DataAnnotations;

namespace AppGeoFit
{
    public class Player
    {
       
        public int PlayerId { get; set; }

        public string Password { get; set; }

        public string PlayerNick { get; set; }

        public string PlayerName { get; set; }

        public string LastName { get; set; }

        public int PhoneNum { get; set; }

        public string PlayerMail { get; set; }
        //TODO
//        public Guid PhotoId { get; set; }

        public double Level { get; set; }

        public double MedOnTime { get; set; }
    }
}
