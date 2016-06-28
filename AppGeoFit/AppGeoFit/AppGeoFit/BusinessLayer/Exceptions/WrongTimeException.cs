using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class WrongTimeException : Exception
    {
        public WrongTimeException(string message) : base(message){ }
    }
}
