using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions
{
    class NotFoundPlayerOnTeamException : Exception
    {
        public NotFoundPlayerOnTeamException(string message) : base(message){ }

    }
}
