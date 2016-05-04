using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class AlreadyCaptainOnSport : Exception
    {
        public AlreadyCaptainOnSport(string message) : base(message){ }

    }
}
