using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions
{
    class NotPendingNoticeException : Exception
    {
        public NotPendingNoticeException(string message) : base(message){ }

    }
}
