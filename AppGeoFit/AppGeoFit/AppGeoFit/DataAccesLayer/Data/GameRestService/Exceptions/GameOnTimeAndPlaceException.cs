using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    class GameOnTimeAndPlaceException : Exception
    {
        public GameOnTimeAndPlaceException(string message) : base(message){ }

    }
}
