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
using System.Data.Entity.Spatial;
using System.Text;

namespace RestServiceGeoFit.Controllers
{
    public class GameController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");
        string dataBase = "GeoFitDB";
        string authData = string.Format("{0}:{1}", Constants.UserName, Constants.PassWord);

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetGame(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            string nativeSQLQuery = @"SELECT GameID, StartDate, EndDate, PlayersNum, Longitude, Latitude, Team1ID, Team2ID, PlaceID, CreatorID, SportID" +
                                     " FROM "+dataBase+".dbo.Game" +
                                    " WHERE GameID = @GameId;";
            var gameResult = db.Games.SqlQuery(nativeSQLQuery, gameId).FirstOrDefault();
            if (gameResult == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "game with this id don't exist");
            }
            gameResult.Place.Games.Clear();
            gameResult.Sport.Games.Clear();
            gameResult.Creator.GamesCreated.Clear();
            gameResult.Creator.Joineds.Clear();
            if (gameResult.Team1ID != null)
            {
                gameResult.Team.Joineds.Clear();
                gameResult.Team.Games.Clear();
                gameResult.Team.Games1.Clear();
            }
            if (gameResult.Team2ID != null)
            {
                gameResult.Team1.Joineds.Clear();
                gameResult.Team1.Games.Clear();
                gameResult.Team1.Games1.Clear();
            }         
            return BuildSuccesResult(HttpStatusCode.OK, gameResult);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateGame([Bind(Include = "StartDate, EndDate, PlayersNum, Longitude, Latitude, Team1ID, Team2ID, PlaceID, CreatorID,SportId")] Game game)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            List<Player> playersToAdd = game.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
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
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, game.GameID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteGame(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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
        public HttpResponseMessage UpdateGame([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] Game game)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            game.Players.Clear();
            game.Sport = null;
            game.Team = null;
            game.Team1 = null;
            game.Creator = null;
            game.Place = null;
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindOnTime(int parameter1, string parameter2, string parameter3)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            DateTime startDate = DateTime.ParseExact(parameter2, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(parameter3, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }
            var playerId = new SqlParameter("@PlayerId", parameter1);
            var startDatePrameter = new SqlParameter("@StartDate", startDate);
            var endDateParameter = new SqlParameter("@EndDate", endDate);
            string nativeSQLQuery = @"SELECT GameID" +
                                    " FROM " +dataBase+".dbo.Participate" +
                                    " WHERE PlayerID = @PlayerId AND GameID IN( SELECT GameID" +
                                    " FROM "+dataBase+".dbo.Game" +
                                    " WHERE NOT((StartDate < @StartDate AND EndDate < @StartDate) OR (StartDate > @EndDate AND EndDate > @EndDate)));";

            var gameIdReturn = db.Database.SqlQuery<int>(nativeSQLQuery, playerId, startDatePrameter, endDateParameter).FirstOrDefault();
            if (gameIdReturn == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, gameIdReturn);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPaginationByTime(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*sportId*/)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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
             
            if (resultGameList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games on this page");
            }
            foreach(Game g in resultGameList)
            {
                g.Place.Games.Clear();
                g.Sport.Games.Clear();
                g.Creator.GamesCreated.Clear();

                g.Creator.Joineds.Clear();
                if (g.Team1ID != null)
                {
                    g.Team.Joineds.Clear();
                    g.Team.Games.Clear();
                    g.Team.Games1.Clear();
                }
                if (g.Team2ID != null)
                {
                    g.Team1.Joineds.Clear();
                    g.Team1.Games.Clear();
                    g.Team1.Games1.Clear();
                }      
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultGameList);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPaginationByDistance(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*sportId*/, double parameter4 /*Longitude*/, double parameter5 /*Latitude*/)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }

            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            
            DateTime dateTimeNow = DateTime.Now;
            List<Game> resultGameList = db.Games.Where(g => g.StartDate.CompareTo(dateTimeNow) > 0 && g.SportId == parameter3)
                                 .OrderBy(g => g.StartDate)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();

            if (resultGameList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games on this page");
            }
            double nearestLongitude;
            double nearestLatitude;

            foreach (Game g in resultGameList)
            {               
                g.Place.Games.Clear();
                g.Sport.Games.Clear();
                g.Creator.GamesCreated.Clear();

                g.Creator.Joineds.Clear();
                if (g.Team1ID != null)
                {
                    g.Team.Joineds.Clear();
                    g.Team.Games.Clear();
                    g.Team.Games1.Clear();
                }
                if (g.Team2ID != null)
                {
                    g.Team1.Joineds.Clear();
                    g.Team1.Games.Clear();
                    g.Team1.Games1.Clear();
                }
            }
            resultGameList.Sort(delegate(Game toCompare, Game home) { return (int)CompareDistance(toCompare.Place.Longitude, toCompare.Place.Latitude, parameter4, parameter5); });
            return BuildSuccesResult(HttpStatusCode.OK, resultGameList);
        }

        private double? CompareDistance(double? longitudeToCompare, double? latitudeToCompare, double? longitudeHome, double? latitudeHome)
        {
            string stringPointHomestring = string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", longitudeHome, latitudeHome);
            string stringPointToCompare = string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", longitudeToCompare, latitudeToCompare);

            DbGeography dbGeographyHome= DbGeography.PointFromText(stringPointHomestring, 4326);
            DbGeography dbGeographyToCompare = DbGeography.PointFromText(stringPointToCompare, 4326);

            return dbGeographyHome.Distance(dbGeographyToCompare);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPaginationByNumPlayers(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*sportId*/)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            DateTime dateTimeNow = DateTime.Now;
            List<Game> resultGameList = db.Games.Where(g => g.StartDate.CompareTo(dateTimeNow) > 0 && g.SportId == parameter3)
                                 .OrderBy(g => g.PlayersNum)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();

            if (resultGameList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games on this page");
            }
            foreach (Game g in resultGameList)
            {
                g.Place.Games.Clear();
                g.Sport.Games.Clear();
                g.Creator.GamesCreated.Clear();

                g.Creator.Joineds.Clear();
                if (g.Team1ID != null)
                {
                    g.Team.Joineds.Clear();
                    g.Team.Games.Clear();
                    g.Team.Games1.Clear();
                }
                if (g.Team2ID != null)
                {
                    g.Team1.Joineds.Clear();
                    g.Team1.Games.Clear();
                    g.Team1.Games1.Clear();
                }
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultGameList);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalGamesCount(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            var playerId = new SqlParameter("@PlayerId", parameter2);
            string nativeSQLQuery = @"SELECT playerID" +
                                    " FROM " +dataBase+".dbo.Participate" +
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
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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
        public HttpResponseMessage AddPlayers([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] Game game)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            List<Player> playersToAdd = game.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
           
            if (ModelState.IsValid)
            {
                game = db.Games.Find(game.GameID);
                db.Entry(game).State = EntityState.Unchanged;
                Player player;
                foreach (Player p in playersToAdd)
                {
                    player = db.Players.Find(p.PlayerID);
                    db.Entry(player).State = EntityState.Unchanged;
                    game.Players.Add(player);
                    game.PlayersNum++;
                }
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage RemovePlayers([Bind(Include = "Longitude, Latitude, CreatorID, PlaceID, PlayersNum, StartDate, Team1ID, Team2ID")] Game game)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            List<Player> playersToRemove = game.Players.ToList();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            if (ModelState.IsValid)
            {
                game = db.Games.Include("Players").FirstOrDefault(g => g.GameID == game.GameID);
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
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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
        public HttpResponseMessage FindOnTimeAndPlace(int parameter1, string parameter2, string parameter3)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            DateTime startDate = DateTime.ParseExact(parameter2, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(parameter3, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Game game = db.Games.Where(g => (g.PlaceID == parameter1) && !((g.StartDate.CompareTo(startDate) < 0 && g.EndDate.CompareTo(startDate) < 0) || (g.StartDate.CompareTo(endDate) > 0 && g.EndDate.CompareTo(endDate) > 0))).FirstOrDefault<Game>();
            if (game == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "place with id: " + parameter1 + " don't have any game on this time.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, game.GameID);

        }
       

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetParticipatePlayers(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }
            var gameId = new SqlParameter("@GameId", parameter1);
            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName," +
                                    " PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    " FROM "+dataBase+".dbo.Player" +
                                    " WHERE PlayerID IN( SELECT PlayerID" +
                                    " FROM "+dataBase+".dbo.Participate" +
                                    " WHERE GameID = @GameId);";

            var playerListReturn = db.Players.SqlQuery(nativeSQLQuery, gameId);
            if (playerListReturn == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Any player participate on this game.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, playerListReturn);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlacesWithOutSport()
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }

            string nativeSQLQuery = @"SELECT * FROM "+dataBase+".dbo.Place WHERE SportID IS NULL;";
            var Places = db.Places.SqlQuery(nativeSQLQuery);

            if (!Places.Any())
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Don't exists any place!.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, Places);

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlacesBySport(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }
            var Places = from p in db.Places
                         where p.SportId == parameter1
                         orderby p.ValuationMed
                         select p;

            if (Places.Count() == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Don't exists any place!.");
            }
            foreach (Place p in Places)
            {
                p.Sport.Places.Clear();
            }
            return BuildSuccesResult(HttpStatusCode.OK, Places);
            
        }
    }
}