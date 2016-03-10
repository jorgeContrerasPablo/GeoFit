using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace RestServiceGeoFit.Controllers
{
    public class BaseApiController : ApiController
    {

        protected HttpResponseMessage BuildErrorResult(HttpStatusCode statusCode, string errorCode)
        {
            return new HttpResponseMessage(statusCode)
            {
                ReasonPhrase = errorCode
            };
        }

        protected HttpResponseMessage BuildSuccesResult(HttpStatusCode statusCode, Object content)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent<Object>(content, Configuration.Formatters.JsonFormatter)
            };
        }
    }
}