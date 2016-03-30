using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class Joined
    {
        public int PlayerID { get; set; }

        public int TeamID { get; set; }

        public bool Captain { get; set; }

        public virtual Player Player { get; set; }

        public virtual Team Team { get; set; }
    }
}
