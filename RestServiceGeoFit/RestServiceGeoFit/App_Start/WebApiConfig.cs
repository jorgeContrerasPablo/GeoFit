﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace RestServiceGeoFit.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{Parameter1}/{Parameter2}/{Parameter3}/{Parameter4}/{Parameter5}",
                defaults: new { Parameter1 = RouteParameter.Optional, Parameter2 = RouteParameter.Optional, Parameter3 = RouteParameter.Optional, Parameter4 = RouteParameter.Optional, Parameter5 = RouteParameter.Optional }
            );

            

            config.Routes.MapHttpRoute(
                name: "DefaultApiTest",
                routeTemplate: "apiTest/{controller}/{action}/{Parameter1}/{Parameter2}/{Parameter3}/{Parameter4}/{Parameter5}",
                defaults: new { Parameter1 = RouteParameter.Optional, Parameter2 = RouteParameter.Optional, Parameter3 = RouteParameter.Optional, Parameter4 = RouteParameter.Optional, Parameter5 = RouteParameter.Optional }
            );
        }
    }
}