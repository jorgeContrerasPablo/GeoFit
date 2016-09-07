using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class PlayerOnGameException : Exception
    {
        public PlayerOnGameException(string message) : base(message) { }
    }
}
