using RestServiceGeoFit.Models2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RestServiceGeoFit.Controllers
{
    public class GameController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetGame(int parameter1)
        {
            Game gameToFind = new Game();
            GameLatitudeLongitude gameToReturn = new GameLatitudeLongitude();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            gameToFind = db.Games.Find(parameter1);

            if (gameToFind == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id : " + parameter1 + " don't exists.");
            }
            gameToReturn = gameToReturn.GameToGameLl(gameToFind);
            return BuildSuccesResult(HttpStatusCode.OK, gameToReturn);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateGame([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] GameLatitudeLongitude gameLl)
        {
            List<Player> playersToAdd = gameLl.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = new Game();
            if (ModelState.IsValid)
            {
                
                game = gameLl.GameLlToGame(gameLl);
                game.Players.Clear();
                if (game.Team1ID != null)
                    game.Team = db.Teams.Find(game.Team1ID);
                if (game.Team2ID != null)
                    game.Team1 = db.Teams.Find(game.Team2ID);
                if (game.PlaceID != null)
                    game.Place = db.Places.Find(game.PlaceID);
                game.Creator = db.Players.Find(game.CreatorID);
                foreach(Player p in playersToAdd)
                {
                    game.Players.Add(db.Players.Find(p.PlayerID));

                }
                db.Games.Add(game);
                db.SaveChanges();
               // db.Games.Find(game.GameID);
               // game.Players = playersToAdd;
               // db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, game.GameID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteGame(int parameter1)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = db.Games.Find(parameter1);
            if (game == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Notice with id: " + parameter1 + " don't exists.");
            }
            db.Games.Remove(game);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateGame([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] GameLatitudeLongitude gameLl)
        {
            // Acces Data Base Test according to request
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = new Game();
            if (ModelState.IsValid)
            {
                game = gameLl.GameLlToGame(gameLl);
                game.Team = null;
                game.Team1 = null;
                game.Creator = null;
                game.Place = null;
                db.Entry(game).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }
        [System.Web.Http.HttpGet]
      //  [Route ("api/Game/{parameter1:int}/parameter2:datetime/paraneter3:datetime")]
        public HttpResponseMessage FindOnTime(int parameter1, string parameter2, string parameter3)
        {
            DateTime startDate = DateTime.ParseExact(parameter2, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(parameter3, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = db.Games.Where(g => g.CreatorID == parameter1 && !((g.StartDate.CompareTo(startDate) <0 && g.EndDate.CompareTo(startDate) <0) || (g.StartDate.CompareTo(endDate) > 0 && g.EndDate.CompareTo(endDate) > 0 ))).FirstOrDefault<Game>();
            if (game == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with nick: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, true);

        }
    }
}