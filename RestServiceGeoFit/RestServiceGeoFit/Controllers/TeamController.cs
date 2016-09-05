using RestServiceGeoFit.Models2;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RestServiceGeoFit.Controllers
{
    public class TeamController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetTeam(int parameter1)
        {
            Team team = new Team();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            team = db.Teams.Find(parameter1);

            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id : " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, team);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateTeam([Bind(Include = "TeamName, ColorTeam, EmblemID, Level, SportID")] Team team)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            team.Sport = null;
            if (ModelState.IsValid)
            {
                team.Sport = db.Sports.Find(team.SportID);
                db.Teams.Add(team);
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, team.TeamID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteTeam(int parameter1)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Team team = db.Teams.Find(parameter1);
            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id: " + parameter1 + " don't exists.");
            }
            db.Teams.Remove(team);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateTeam([Bind(Include = "TeamName, ColorTeam, EmblemID, Level, SportID")] Team team)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddPlayer([Bind(Include = "PlayerID, TeamID, Captain")] Joined joined)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Team team = db.Teams.Find(joined.TeamID);
            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team : " + team.TeamName + " don't exists.");
            }
            Player player = db.Players.Find(joined.PlayerID);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player " + player.PlayerName + " don't exists.");
            }
            joined.Player = player;
            joined.Team = team;
            team.Joineds.Add(joined);
            //player.Joineds.Add(joined);
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage RemovePlayer([Bind(Include = "PlayerID, TeamID, Captain")] Joined joined)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Joined joinedDb = db.Joineds.Find(joined.PlayerID,joined.TeamID);
            if (joinedDb == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player : " + joinedDb.Player.PlayerNick + " dosen't join on team: "+joinedDb.Team.TeamName+".");
            }
            db.Joineds.Remove(joinedDb);
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetSports()
        {
            var Sports = from s in db.Sports
                    orderby s.SportName
                    select s;

            if (Sports == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Don't exists any sport!.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, Sports);

        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindTeamByNameOnSports(string parameter1, int parameter2)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Team team = db.Teams.Where(p => p.TeamName == parameter1 && p.SportID == parameter2).FirstOrDefault<Team>();
            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with name: " + parameter1 + " don't exists to sport with id: " +parameter2 +".");
            }
            return BuildSuccesResult(HttpStatusCode.OK, team.TeamID);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetCaptain(int parameter1)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var TeamId = new SqlParameter("@TeamId", parameter1);
            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName, PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    "FROM GeoFitDB.dbo.Player " +
                                    "WHERE PlayerID = (SELECT PlayerID " +
                                                       "FROM GeoFitDB.dbo.Joined " +
                                                       "WHERE TeamID = @TeamId AND Captain = 1);";
            var player = db.Players.SqlQuery(nativeSQLQuery, TeamId).FirstOrDefault<Player>();
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id: " +
                    parameter1 + " not has captain.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, player);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPlayersPendingToAdd(int parameter1, int parameter2, string parameter3)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            var messengerId = new SqlParameter("@MessengerId", parameter1);
            var sportId = new SqlParameter("@SportId", parameter2);
            var type = new SqlParameter("@type", parameter3);
            string nativeSQLQuery = @"SELECT PlayerID, Password, PlayerNick, PlayerName, LastName," +
                                    " PhoneNum, PlayerMail, PhotoID, Level, MedOnTime, FavoriteSportID, PlayerSesion " +
                                    "FROM GeoFitDB.dbo.Player " + 
                                    "WHERE PlayerID IN (SELECT ReceiverID "+ 
                                                        "FROM GeoFitDB.dbo.Notice "+
                                                        "WHERE MessengerID = @MessengerId AND " +
                                                        "SportID = @SportId AND Type = @type "+
                                                        "AND Accepted IS NULL)";
            var listPlayersPending = db.Players.SqlQuery(nativeSQLQuery, messengerId, sportId, type);
            if (listPlayersPending == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "This team dosen't have" +
                                        " any pending player to add.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, listPlayersPending);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsOnTeam(int parameter1, int parameter2)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var joined = db.Joineds.Where(j => j.TeamID == parameter1 && j.PlayerID == parameter2).FirstOrDefault<Joined>();

            if(joined == null)
                return BuildSuccesResult(HttpStatusCode.OK, false);
            else
                return BuildSuccesResult(HttpStatusCode.OK, true);

        }
    }
}