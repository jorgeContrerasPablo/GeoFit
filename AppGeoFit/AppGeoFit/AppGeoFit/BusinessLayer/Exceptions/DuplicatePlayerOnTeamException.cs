using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class DuplicatePlayerOnTeamException : Exception
    {
        public DuplicatePlayerOnTeamException(string message) : base(message){ }

    }
}
