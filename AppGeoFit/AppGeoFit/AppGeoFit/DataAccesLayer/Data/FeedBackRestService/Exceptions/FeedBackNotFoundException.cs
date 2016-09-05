using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.FeedBackRestService.Exceptions
{
    class FeedBackNotFoundException : Exception
    {
        public FeedBackNotFoundException(string message) : base(message){ }

    }
}
