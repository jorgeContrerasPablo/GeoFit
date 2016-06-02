using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Models
{
    public class Notice
    {
        public int NoticeID { get; set; }

        public string Type { get; set; }

        public bool? Accepted { get; set; }

        public int MessengerID { get; set; }

        public int ReceiverID { get; set; }

        public int SportID { get; set; }

        public virtual Player Messenger { get; set; }

        public virtual Player Receiver { get; set; }

        public virtual Sport Sport { get; set; }

    }
}
