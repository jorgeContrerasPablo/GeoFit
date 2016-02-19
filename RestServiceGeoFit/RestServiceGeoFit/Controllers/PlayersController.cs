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
        static readonly PlayerManager playerManager = new PlayerManager();

        [HttpGet]
        public HttpResponseMessage Player(int playerId)
        {
            //return Request.CreateResponse(HttpStatusCode.OK, (playerManager.GetPlayer(playerId)));
            //TODO TRY CATcH
            Player player = playerManager.GetPlayer(playerId);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Player>(player, Configuration.Formatters.JsonFormatter)
                
            };
        }

    }
}