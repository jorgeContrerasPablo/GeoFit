using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message) : base(message){ }

    }
}
