using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions
{
    public class TeamNotFoundException : Exception
    {
        public TeamNotFoundException(string message) : base(message){ }
    }
}
