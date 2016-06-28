using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    class GameOnTimeException : Exception
    {
        public GameOnTimeException(string message) : base(message){ }

    }
}
