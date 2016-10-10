using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class DuplicateNoticeException : Exception
    {
        public DuplicateNoticeException(string message) : base(message){ }

    }
}
