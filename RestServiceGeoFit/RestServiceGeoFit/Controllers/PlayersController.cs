using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestServiceGeoFit.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using RestServiceGeoFit.Models.Managers.Player.Exceptions;

namespace RestServiceGeoFit.Controllers
{
    public class PlayersController : BaseApiController
    {
        static readonly bool test = true;
        PlayerManager playerManager = new PlayerManager(!test);

        [HttpGet]
        public HttpResponseMessage GetPlayer(int parameter1)
        {
            Player player = new Player();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            try
            {
                player = playerManager.GetPlayer(parameter1);
            }
            catch (PlayerNotFoundException ex)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, ex.Message);
            }
            

            return BuildSuccesResult(HttpStatusCode.OK, player);
        }

        [HttpPost]
        public HttpResponseMessage CreatePlayer(Player player)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            //TODO TRY CATcH
            int response = playerManager.CreatePlayer(player);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<int>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpDelete]
        public HttpResponseMessage DeletePlayer(int parameter1)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            try
            {
                response = playerManager.DeletePlayer(parameter1);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpPut]
        public HttpResponseMessage UpdatePlayer(Player player)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            try
            {
                response = playerManager.UpdatePlayer(player);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpGet]
        public HttpResponseMessage FindPlayerByMail(string parameter1, string parameter2)
        {
            int response = 0;
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }

            parameter1 = parameter1 + "." + parameter2;

            try
            {   
                response = playerManager.FindPlayerByNickOrMail(parameter1);
            }
            catch (PlayerNotFoundException ex)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return BuildSuccesResult(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage FindPlayerByNick(string parameter1)
        {
            int response = 0;
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }

            try
            {
                response = playerManager.FindPlayerByNickOrMail(parameter1);
            }
            catch (PlayerNotFoundException ex)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return BuildSuccesResult(HttpStatusCode.OK, response);
        }

        [HttpPut]
        public HttpResponseMessage Session(int parameter1)
        {
           
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }

            try
            {
                playerManager.Session(parameter1, true);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [HttpPut]
        public HttpResponseMessage OutSession(int parameter1)
        {

            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }

            try
            {
                playerManager.Session(parameter1, false);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }

            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [HttpGet]
        public HttpResponseMessage IsOnSession(int parameter1)
        {
            bool response = false;
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            try
            {
                response = playerManager.IsOnSession(parameter1);
            }
            catch (Exception ex)
            {
                return BuildErrorResult(HttpStatusCode.BadRequest, ex.Message);
            }
            return BuildSuccesResult(HttpStatusCode.OK, response);

        }
    }
}