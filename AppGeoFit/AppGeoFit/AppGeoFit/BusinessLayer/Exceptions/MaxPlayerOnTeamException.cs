using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class MaxPlayerOnTeamException : Exception
    {
        public MaxPlayerOnTeamException(string message) : base(message){ }
    }
}
