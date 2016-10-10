using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions
{
    public class GameOnTimeException : Exception
    {
        public GameOnTimeException(string message) : base(message){ }

    }
}
