using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class MaxPlayerOnGameException : Exception
    {
        public MaxPlayerOnGameException(string message) : base(message){ }

    }
}
