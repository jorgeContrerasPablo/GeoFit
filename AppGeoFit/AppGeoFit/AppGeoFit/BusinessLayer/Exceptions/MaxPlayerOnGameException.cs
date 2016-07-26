using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class MaxPlayerOnGameException : Exception
    {
        public MaxPlayerOnGameException(string message) : base(message){ }

    }
}
