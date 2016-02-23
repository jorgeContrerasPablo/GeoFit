using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestServiceGeoFit.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace RestServiceGeoFit.Controllers
{
    public class PlayersController : BaseApiController
    {
        //TODO Acces to testDb
        static readonly PlayerManager playerManager = new PlayerManager();

         [HttpGet]
         public HttpResponseMessage GetPlayer(int playerId)
         {
             //return Request.CreateResponse(HttpStatusCode.OK, (playerManager.GetPlayer(playerId)));
             //TODO TRY CATcH
             Player player = playerManager.GetPlayer(playerId);
             return new HttpResponseMessage(HttpStatusCode.OK)
             {
                 Content = new ObjectContent<Player>(player, Configuration.Formatters.JsonFormatter)

             };
         }

        [HttpPost]
        public HttpResponseMessage CreatePlayer(Player player)
        {
            //TODO TRY CATcH
            int response = playerManager.CreatePlayer(player);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<int>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpDelete]
        public HttpResponseMessage DeletePlayer(int playerId)
        {
            //TODO TRY CATcH
            bool response = playerManager.DeletePlayer(playerId);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

        [HttpPut]
        public HttpResponseMessage UpdatePlayer(Player player)
        {
            //TODO TRY CATcH
            bool response = playerManager.UpdatePlayer(player);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Boolean>(response, Configuration.Formatters.JsonFormatter)

            };
        }

    }
}