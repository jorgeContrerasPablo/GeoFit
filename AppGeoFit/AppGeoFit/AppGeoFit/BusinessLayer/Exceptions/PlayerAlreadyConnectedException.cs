using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class PlayerAlreadyConnectedException : Exception
    {
        public PlayerAlreadyConnectedException(string message) : base(message){ }
    }
}
