using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class FeedBack
    {
        public int FeedbackID { get; set; }

        public string Description { get; set; }

        public double? Valuation { get; set; }

        public DateTime FeedBackDate { get; set; }

        public bool? OnTime { get; set; }

        public int? PlaceID { get; set; }

        public int? PlayerID { get; set; }

        public int? CreatorID { get; set; }

        public int? TeamID { get; set; }

        public int? GameID { get; set; }

        public virtual Game Game { get; set; }

        public virtual Place Place { get; set; }

        public virtual Player Player { get; set; }

        public virtual Player Creator { get; set; }

        public virtual Team Team { get; set; }

        public override int GetHashCode()
        {
            return FeedbackID;
        }
    }
}
