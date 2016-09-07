using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message) : base(message){ }

    }
}
