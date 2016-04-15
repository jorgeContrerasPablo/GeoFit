using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class DuplicateCaptainException : Exception
    {
        public DuplicateCaptainException(string message) : base(message){ }

    }
}
