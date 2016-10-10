using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class AlreadyCaptainOnSportException : Exception
    {
        public AlreadyCaptainOnSportException(string message) : base(message){ }

    }
}
