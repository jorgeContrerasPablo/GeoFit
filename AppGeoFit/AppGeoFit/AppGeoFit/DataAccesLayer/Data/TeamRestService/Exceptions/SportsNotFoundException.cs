using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions
{
    public class SportsNotFoundException : Exception
    {
        public SportsNotFoundException(string message) : base(message){ }
    }
}
