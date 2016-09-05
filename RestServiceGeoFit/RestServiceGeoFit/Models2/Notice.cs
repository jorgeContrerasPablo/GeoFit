

namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notice")]
    public class Notice
    {
        public int NoticeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        public bool? Accepted { get; set; }

        [Required]
        public int MessengerID { get; set; }        

        [Required]
        public int ReceiverID { get; set; }        

        [Required]
        public int SportID { get; set; }
        
        public int? GameID { get; set; }

        public virtual Player Messenger { get; set; }

        public virtual Player Receiver { get; set; }

        public virtual Sport Sport { get; set; }

        public virtual Game Game { get; set; }

    }
}