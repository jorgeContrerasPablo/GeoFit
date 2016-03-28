using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Exceptions
{
    public class PasswordIncorrectException : Exception
    {
        public PasswordIncorrectException(string message) : base(message){ }
    }

}
