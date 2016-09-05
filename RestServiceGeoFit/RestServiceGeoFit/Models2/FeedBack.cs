namespace RestServiceGeoFit.Models2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FeedBack")]
    public partial class FeedBack
    {
        public int FeedbackID { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public double? Valuation { get; set; }

        public DateTime FeedBackDate { get; set; }

        public bool? OnTime { get; set; }

        public int? PlaceID { get; set; }

        public int? PlayerID { get; set; }

        public int CreatorID { get; set; }

        public int? TeamID { get; set; }

        public int? GameID { get; set; }

        public virtual Game Game { get; set; }

        public virtual Place Place { get; set; }

        public virtual Player Player { get; set; }

        public virtual Player Creator { get; set; }

        public virtual Team Team { get; set; }
    }
}
