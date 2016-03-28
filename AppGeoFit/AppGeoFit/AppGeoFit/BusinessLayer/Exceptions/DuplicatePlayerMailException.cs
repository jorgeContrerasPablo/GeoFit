using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class DuplicatePlayerMailException : Exception
    {
        public DuplicatePlayerMailException(string message) : base(message){ }

    }
}
