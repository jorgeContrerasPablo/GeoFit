using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions
{
    class NotTeamJoinedOnSportException : Exception
    {
        public NotTeamJoinedOnSportException(string message) : base(message) { }

    }
}
