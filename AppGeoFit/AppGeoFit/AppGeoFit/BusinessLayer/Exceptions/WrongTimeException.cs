using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class WrongTimeException : Exception
    {
        public WrongTimeException(string message) : base(message){ }
    }
}
