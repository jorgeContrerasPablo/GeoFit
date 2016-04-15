using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions
{
    public class CaptainNotFoundException : Exception
    {
        public CaptainNotFoundException(string message) : base(message){ }

    }
}
