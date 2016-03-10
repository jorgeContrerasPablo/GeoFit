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
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            //TODO TRY CATcH
            bool response = playerManager.DeletePlayer(parameter1);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpPut]
        public HttpResponseMessage UpdatePlayer(Player player)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                playerManager = new PlayerManager(test);
            }
            //TODO TRY CATcH
            bool response = playerManager.UpdatePlayer(player);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpGet]
        public HttpResponseMessage FindPlayerByNickOrMail(string parameter1)
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

            return BuildSuccesResult(HttpStatusCode.OK, response);
        }

    }
}