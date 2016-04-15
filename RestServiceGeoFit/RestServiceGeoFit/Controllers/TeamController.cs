using RestServiceGeoFit.Models2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RestServiceGeoFit.Controllers
{
    public class TeamController : BaseApiController
    {
        static readonly bool test = true;
        private readonly AppGeoFitDBContext db = new AppGeoFitDBContext();

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetTeam(int parameter1)
        {
            Team team = new Team();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //playerManager = new PlayerManager(test);
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
                //playerManager = new PlayerManager(test);
            }
            if (ModelState.IsValid)
            {
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
                //   playerManager = new PlayerManager(test);
            }
            Team Team = db.Teams.Find(parameter1);
            if (Team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id: " + parameter1 + " don't exists.");
            }
            db.Teams.Remove(Team);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateTeam([Bind(Include = "TeamName, ColorTeam, EmblemID, Level, SportID")] Team team)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //   playerManager = new PlayerManager(test);
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

        [System.Web.Http.HttpGet]
        public HttpResponseMessage AddPlayer(int parameter1, int parameter2, bool parameter3)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //   playerManager = new PlayerManager(test);
            }
            Team team = db.Teams.Find(parameter1);
            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id: " + parameter1 + " don't exists.");
            }
            Player player = db.Players.Find(parameter2);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter2 + " don't exists.");
            }
            Joined joined = new Joined();
            joined.TeamID = parameter1;
            joined.PlayerID = parameter2;
            joined.Captain = parameter3;
            joined.Player = player;
            joined.Team = team;
            team.Joineds.Add(joined);
            //player.Joineds.Add(joined);
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
                //  playerManager = new PlayerManager(test);
            }
            Team team = db.Teams.Where(p => p.TeamName == parameter1 && p.SportID == parameter2).FirstOrDefault<Team>();
            if (team == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with name: " + parameter1 + " don't exists to sport with id: " +parameter2 +".");
            }
            return BuildSuccesResult(HttpStatusCode.OK, team.TeamID);
        }

    }
}