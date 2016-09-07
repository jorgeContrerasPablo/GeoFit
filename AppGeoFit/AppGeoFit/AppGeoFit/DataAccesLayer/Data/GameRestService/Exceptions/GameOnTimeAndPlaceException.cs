using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    public class GameOnTimeAndPlaceException : Exception
    {
        public GameOnTimeAndPlaceException(string message) : base(message){ }

    }
}
