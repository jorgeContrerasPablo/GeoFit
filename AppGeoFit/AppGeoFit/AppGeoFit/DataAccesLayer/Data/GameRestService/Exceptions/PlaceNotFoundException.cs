using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    public class PlaceNotFoundException : Exception
    {
        public PlaceNotFoundException(string message) : base(message) { }

    }
}
