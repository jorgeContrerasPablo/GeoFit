using RestServiceGeoFit.Models2;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
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
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            string nativeSQLQuery = @"SELECT GameID, StartDate, EndDate, PlayersNum, Coordinates, Team1ID, Team2ID, PlaceID, CreatorID, SportID" +
                                     " FROM GeoFitDB.dbo.Game" +
                                    " WHERE GameID = @GameId;";
            var gameResult = db.Games.SqlQuery(nativeSQLQuery, gameId).FirstOrDefault();
            if (gameResult == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "game with this id don't exist");
            }
            GameLatitudeLongitude gamLL = new GameLatitudeLongitude();            
            return BuildSuccesResult(HttpStatusCode.OK, gamLL.GameToGameLl(gameResult));
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
                game.Creator = null;
                game.Sport = null;                
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
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            gameLl.Players.Clear();
            gameLl.Sport = null;
            gameLl.Team = null;
            gameLl.Team1 = null;
            gameLl.Creator = null;
            gameLl.Place = null;
            Game game = new Game();
            if (ModelState.IsValid)
            {
                game = gameLl.GameLlToGame(gameLl);
                /*game.Team = null;
                game.Team1 = null;
                game.Creator = null;
                game.Place = null;
                game.Sport.Games.Clear();*/
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }
        //[System.Web.Http.HttpGet]
        //  [Route ("api/Game/{parameter1:int}/parameter2:datetime/paraneter3:datetime")]
        /*public HttpResponseMessage FindOnTime(int parameter1, string parameter2, string parameter3)
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
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, true);

        }*/
        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindOnTime(int parameter1, string parameter2, string parameter3)
        {
            DateTime startDate = DateTime.ParseExact(parameter2, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(parameter3, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var playerId = new SqlParameter("@PlayerId", parameter1);
            var startDatePrameter = new SqlParameter("@StartDate", startDate);
            var endDateParameter = new SqlParameter("@EndDate", endDate);
            string nativeSQLQuery = @"SELECT GameID" +
                                    " FROM GeoFitDB.dbo.Participate" +
                                    " WHERE PlayerID = @PlayerId AND GameID IN( SELECT GameID" +
                                    " FROM GeoFitDB.dbo.Game" +
                                    " WHERE NOT((StartDate < @StartDate AND EndDate < @StartDate) OR (StartDate > @EndDate AND EndDate > @EndDate)));";

            var gameIdReturn = db.Database.SqlQuery<int>(nativeSQLQuery, playerId, startDatePrameter, endDateParameter).FirstOrDefault();
            if (gameIdReturn == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, gameIdReturn);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPagination(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*sportId*/)
        {
            //int totalRows = db.Games.Count();
            //int totalPages = (int)Math.Ceiling((double)totalRows / parameter2);
            List<GameLatitudeLongitude> resultGameLLList = new List<GameLatitudeLongitude>();
            GameLatitudeLongitude GameLL = new GameLatitudeLongitude();
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            DateTime dateTimeNow = DateTime.Now;
            List<Game>resultGameList = db.Games.Where(g => g.StartDate.CompareTo(dateTimeNow)>0 && g.SportId == parameter3)
                                 .OrderBy(g => g.StartDate)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)                                 
                                 .ToList();
             
            if (resultGameList == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games on this page");
            }
          //  Sport sport = 
          //  Player player = 
            foreach(Game g in resultGameList)
            {
                //g.Sport = db.Sports.Find(g.SportId);
                // g.Creator = db.Players.Find(g.CreatorID);
                g.Sport.Games.Clear();
                g.Creator.GamesCreated.Clear();
                resultGameLLList.Add(GameLL.GameToGameLl(g));
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultGameLLList);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalGamesCount(int parameter1)
        {
            int numGames = 0;
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            DateTime dateTimeNow = DateTime.Now;
            numGames = db.Games.Where(g => g.StartDate.CompareTo(dateTimeNow) > 0 && g.SportId == parameter1).Count();
            if (numGames == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games");
            }
            return BuildSuccesResult(HttpStatusCode.OK, numGames);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsPlayerOnGame(int parameter1, int parameter2)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            var playerId = new SqlParameter("@PlayerId", parameter2);
            string nativeSQLQuery = @"SELECT playerID" +
                                    " FROM GeoFitDB.dbo.Participate" +
                                    " WHERE PlayerID = @PlayerId AND GameID = @GameId;";
            var playerIdReturn = db.Database.SqlQuery<int>(nativeSQLQuery, playerId, gameId).FirstOrDefault();
            if(playerIdReturn == 0)
            {
                return BuildSuccesResult(HttpStatusCode.OK, false);
            }
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage AddPlayer(int parameter1, int parameter2)
        {
            //bool playerAdded= false;
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = db.Games.Find(parameter1);
            db.Entry(game).State = EntityState.Unchanged;
            Player player = db.Players.Find(parameter2);
            db.Entry(player).State = EntityState.Unchanged;
            game.Players.Add(player);
            game.PlayersNum++;
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }
        
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddPlayers([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] GameLatitudeLongitude gameLl)
        {
            List<Player> playersToAdd = gameLl.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
           
            if (ModelState.IsValid)
            {
                Game game = db.Games.Find(gameLl.GameID);
                db.Entry(game).State = EntityState.Unchanged;

                /*  game = gameLl.GameLlToGame(gameLl);
                  //game.Players.Clear();
                  if (game.Team1ID != null)
                      game.Team = db.Teams.Find(game.Team1ID);
                  if (game.Team2ID != null)
                      game.Team1 = db.Teams.Find(game.Team2ID);
                  if (game.PlaceID != null)
                      game.Place = db.Places.Find(game.PlaceID);
                  game.Creator = null;
                  game.Sport = null;*/
                Player player;
                foreach (Player p in playersToAdd)
                {
                    player = db.Players.Find(p.PlayerID);
                    db.Entry(player).State = EntityState.Unchanged;
                    game.Players.Add(player);
                    game.PlayersNum++;
                }
                db.SaveChanges();
                // db.Games.Find(game.GameID);
                // game.Players = playersToAdd;
                // db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage RemovePlayers([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] GameLatitudeLongitude gameLl)
        {
            List<Player> playersToRemove = gameLl.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            if (ModelState.IsValid)
            {
                Game game = db.Games.Include("Players").FirstOrDefault(g => g.GameID == gameLl.GameID);
                Player player;
                foreach (Player p in playersToRemove)
                {
                    player = db.Players.Find(p.PlayerID);
                    game.Players.Remove(player);
                    game.PlayersNum--;
                }
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindTeamOnTime(int parameter1, string parameter2, string parameter3)
        {
            DateTime startDate = DateTime.ParseExact(parameter2, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(parameter3, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = db.Games.Where(g => (g.Team1ID == parameter1 || g.Team2ID == parameter1) && !((g.StartDate.CompareTo(startDate) < 0 && g.EndDate.CompareTo(startDate) < 0) || (g.StartDate.CompareTo(endDate) > 0 && g.EndDate.CompareTo(endDate) > 0))).FirstOrDefault<Game>();
            if (game == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, game.GameID);

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetParticipatePlayers(int parameter1)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName," +
                                    " PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    " FROM GeoFitDB.dbo.Player" +
                                    " WHERE PlayerID IN( SELECT PlayerID" +
                                    " FROM GeoFitDB.dbo.Participate" +
                                    " WHERE GameID = @GameId);";

            var playerListReturn = db.Players.SqlQuery(nativeSQLQuery, gameId);
            if (playerListReturn == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Any player participate on this game.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, playerListReturn);
        }
    }
}