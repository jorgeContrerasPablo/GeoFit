using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGeoFit.Models.Managers.Player.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(int playerId) : base("Player dosen't exist with id : " + playerId)
        {
            this.PlayerId = playerId;
        }

        public PlayerNotFoundException(string mailOrNick) : base("Player dosen't exist with mail or nick : " + mailOrNick)
        {
            this.mailOrNick = mailOrNick;
        }

        public int PlayerId { get; private set; }
        public string mailOrNick { get; private set; }

    }
}