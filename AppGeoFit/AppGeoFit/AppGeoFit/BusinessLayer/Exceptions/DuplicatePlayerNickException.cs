using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class DuplicatePlayerNickException :  Exception
    {
        public DuplicatePlayerNickException(string message) : base(message){ }
    }
}

