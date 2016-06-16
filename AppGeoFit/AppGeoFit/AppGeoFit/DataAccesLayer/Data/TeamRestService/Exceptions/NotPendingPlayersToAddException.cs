using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions
{
    class NotPendingPlayersToAddException : Exception
    {
        public NotPendingPlayersToAddException(string message) : base(message){ }
    }
}
