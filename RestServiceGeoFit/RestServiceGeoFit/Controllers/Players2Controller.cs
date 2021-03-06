﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using RestServiceGeoFit.Models2;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace RestServiceGeoFit.Controllers
{
    public class Players2Controller : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");
        string dataBase = "GeoFitDB";
        string authData = string.Format("{0}:{1}", Constants.UserName, Constants.PassWord);


        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlayer(int parameter1)
        {
            
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            Player player = new Player();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");               
            }

            player = db.Players.Find(parameter1);

            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id : " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, player);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAll()
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
                dataBase = "GeoFitDBTest";
            }

            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName," +
                                    " PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    "FROM " + dataBase+".dbo.Player ";
            var players = db.Players.SqlQuery(nativeSQLQuery);

            if (!players.Any())
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There aren't any player in this app.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, players);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreatePlayer([Bind(Include = "Password,PlayerNick,PlayerName,LastName,PhoneNum,PlayerMail,PhotoID,Level,MedOnTime,FavoriteSportID,PlayerSesion")] Player player)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                if (player.FavoriteSportID != null)
                    player.Sport = db.Sports.Find(player.FavoriteSportID);
                db.Players.Add(player);
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeletePlayer(int parameter1)
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
            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }
            db.Players.Remove(player);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdatePlayer([Bind(Include = "PlayerID,Password,PlayerNick,PlayerName,LastName,PhoneNum,PlayerMail,PhotoID,Level,MedOnTime,FavoriteSportID,PlayerSesion")] Player player)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                db.Entry(player).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindPlayerByMail(string parameter1, string parameter2)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            parameter1 = parameter1 + "." + parameter2;
            Player player = db.Players.Where(p => p.PlayerMail == parameter1).FirstOrDefault<Player>();
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with email: " + parameter1 + " don't exists.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerID);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindPlayerByNick(string parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Player player = db.Players.Where(p => p.PlayerNick == parameter1).FirstOrDefault<Player>();
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with nick: " + parameter1 + " don't exists.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerID);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage Session(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }
            player.PlayerSesion = true;
            db.Entry(player).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage OutSession(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }
            player.PlayerSesion = false;
            db.Entry(player).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsOnSession(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerSesion);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindCaptainOnSports(int parameter1, int parameter2)
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
            var playerId = new SqlParameter("@PlayerId", parameter1);
            var sportId = new SqlParameter("@SportId", parameter2);
            string nativeSQLQuery = @"SELECT PlayerID, TeamID, Captain " +
                                    "FROM "+dataBase+".dbo.Joined " +
                                    "WHERE PlayerID = @PlayerId AND Captain = 1 " +
                                             "AND TeamID in ( SELECT TeamID " +
                                                     "FROM "+dataBase+".dbo.Team " +
                                                     "WHERE SportID = @SportId)";

            var joined = db.Joineds.SqlQuery(nativeSQLQuery, playerId, sportId).FirstOrDefault<Joined>(); ;

            if (joined == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with name: " +
                    parameter1 + " is not a captain on sportId " + parameter2 + ".");
            }
            return BuildSuccesResult(HttpStatusCode.OK, joined.TeamID);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindTeamsJoined(int parameter1, int parameter2)
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

            var playerId = new SqlParameter("@PlayerId", parameter1);
            var sportId = new SqlParameter("@SportId", parameter2);
            string nativeSQLQuery = @"SELECT TeamID, TeamName, ColorTeam, EmblemID, Level, SportID " +
                                    "FROM "+dataBase+".dbo.Team " +
                                    "WHERE SportID = @SportId AND TeamID in (SELECT TeamID " +
                                                                            "FROM "+dataBase+".dbo.Joined " +
                                                                            "WHERE PlayerID = @PlayerId);";
            var listTeam = db.Teams.SqlQuery(nativeSQLQuery, playerId, sportId);

            if (listTeam == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with name: " +
                    parameter1 + " is not joined any team on sport " + parameter2 + ".");
            }
            return BuildSuccesResult(HttpStatusCode.OK, listTeam);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindPlayerOnTeam(string parameter1, int parameter2)
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
            var playerNick = new SqlParameter("@PlayerNick", parameter1);
            var sportId = new SqlParameter("@TeamId", parameter2);
            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName,"+
                                    " PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    "FROM " + dataBase+".dbo.Player " +
                                    "WHERE PlayerNick = @playerNick AND PlayerID in (SELECT PlayerID " +
                                                                            "FROM "+dataBase+".dbo.Joined " +
                                                                            "WHERE TeamID = @TeamId);";
            var player = db.Players.SqlQuery(nativeSQLQuery, playerNick, sportId).FirstOrDefault<Player>();

            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with nick: " +
                    parameter1 + " is not joined in team " + parameter2 + ".");
            }
            return BuildSuccesResult(HttpStatusCode.OK, player);

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetActualGames(int parameter1, int parameter2, int parameter3, int parameter4)
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

            var playerId = new SqlParameter("@PlayerId", parameter3);
            var actualDate = new SqlParameter("@ActualDate", DateTime.Now);
            var sportId = new SqlParameter("@SportId", parameter4);

            string nativeSQLQuery = @"SELECT GameID, StartDate, EndDate, PlayersNum, Longitude, Latitude, Team1ID, Team2ID, PlaceID, CreatorID, SportID" +
                                    " FROM "+dataBase+".dbo.Game" +
                                    " WHERE SportID = @SportId AND StartDate > @ActualDate AND GameID IN( SELECT GameID" +
                                    " FROM "+dataBase+".dbo.Participate" +
                                    " WHERE PlayerID = @PlayerId)"+
                                    " ORDER BY StartDate;";

            var resultGameList = db.Database.SqlQuery<Game>(nativeSQLQuery, playerId, actualDate, sportId)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();
            if (resultGameList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games");
            }

            foreach (Game g in resultGameList)
            {
                if (g.Sport == null)                    
                    g.Sport = db.Sports.Find(g.SportId);
                if (g.Creator == null)                    
                    g.Creator = db.Players.Find(g.CreatorID);
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultGameList);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalGamesCount(int parameter1, int parameter2)
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
            var playerId = new SqlParameter("@PlayerId", parameter1);
            var actualDate = new SqlParameter("@ActualDate", DateTime.Now);
            var sportId = new SqlParameter("@SportId", parameter2);

            string nativeSQLQuery = @"SELECT GameID " +
                                    " FROM "+dataBase+".dbo.Game" +
                                    " WHERE SportID = @SportId AND StartDate > @ActualDate AND GameID IN( SELECT GameID" +
                                    " FROM "+dataBase+".dbo.Participate" +
                                    " WHERE PlayerID = @PlayerId)";

            var numGamesSqlReturn = db.Database.SqlQuery<int>(nativeSQLQuery, playerId, actualDate, sportId).Count();
            if (numGamesSqlReturn == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no games");
            }
            return BuildSuccesResult(HttpStatusCode.OK, numGamesSqlReturn);
        }


    }
}