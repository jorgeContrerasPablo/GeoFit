using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class MaxPlayerOnTeamException : Exception
    {
        public MaxPlayerOnTeamException(string message) : base(message){ }
    }
}
