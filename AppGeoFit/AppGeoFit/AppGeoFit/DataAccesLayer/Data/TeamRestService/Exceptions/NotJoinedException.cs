using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions
{
    class NotJoinedException : Exception
    {
        public NotJoinedException(string message) : base(message){ }
    }
}
