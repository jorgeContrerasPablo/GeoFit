using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions
{
    class NoticeNotFoundException : Exception
    {
        public NoticeNotFoundException(string message) : base(message){ }

    }
}
