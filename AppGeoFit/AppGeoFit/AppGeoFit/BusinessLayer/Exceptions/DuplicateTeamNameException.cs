using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class DuplicateTeamNameException : Exception
    {
        public DuplicateTeamNameException(string message) : base(message){ }
    }
}
