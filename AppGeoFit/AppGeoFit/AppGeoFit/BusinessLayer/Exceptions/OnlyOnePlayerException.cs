using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    class OnlyOnePlayerException : Exception
    {
        public OnlyOnePlayerException(string message) : base(message){ }

    }
}
