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
    public class FeedBackController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetFeedBack(int parameter1)
        {
            FeedBack feedBack = new FeedBack();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            feedBack = db.FeedBacks.Find(parameter1);

            if (feedBack == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "FeedBack with id : " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, feedBack);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateFeedBack([Bind(Include = "Description, Valuation, FeedBackDate, OnTime, PlaceID, PlayerID, CreatorID, TeamID, GameID")] FeedBack feedBack)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                feedBack.Player = feedBack.PlayerID != null ? db.Players.Find(feedBack.PlayerID) : null;
                feedBack.Place = feedBack.PlaceID != null ? db.Places.Find(feedBack.PlaceID) : null;
                feedBack.Team = feedBack.TeamID != null ? db.Teams.Find(feedBack.TeamID) : null;
                feedBack.Creator = db.Players.Find(feedBack.CreatorID);
                db.FeedBacks.Add(feedBack);
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, feedBack.FeedbackID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteFeedBack(int parameter1)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            FeedBack feedBack = db.FeedBacks.Find(parameter1);
            if (feedBack == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "FeedBack with id: " + parameter1 + " don't exists.");
            }
            db.FeedBacks.Remove(feedBack);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateFeedBack([Bind(Include = "Description, Valuation, FeedBackDate, OnTime, PlaceID, PlayerID, CreatorID, TeamID, GameID")] FeedBack feedBack)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                feedBack.Creator = null;
                feedBack.Game = null;
                feedBack.Place = null;
                feedBack.Player = null;
                feedBack.Team = null;
                db.Entry(feedBack).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateLvlAndOnTimePlayer( int parameter1)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var player = db.Players.Find(parameter1);           
            //Recuperamos la media de las valoraciones del jugador.
            var valuations = db.FeedBacks.Where(f => f.PlayerID == parameter1).Average(f => f.Valuation);
            //Recuperamos el total de valoraciones y calculamos la media de veces que ha llegado en hora.
            double allFeedBacks = db.FeedBacks.Where(f => f.PlayerID == parameter1).Count();
            double timesOnTime = db.FeedBacks.Where(f => f.PlayerID == parameter1 && f.OnTime == true).Count();        
            double OnTime = allFeedBacks == 0.0 ? 0 : timesOnTime / allFeedBacks;
            double? level = valuations == null ? 0 : valuations;
            //Actualizamos el jugador.
            player.MedOnTime = OnTime;
            player.Level = level;
            db.Entry(player).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateLvlTeam(int parameter1)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var team = db.Teams.Find(parameter1);
            //Recuperamos la media de las valoraciones del equipo.
            var valuations = db.FeedBacks.Where(f => f.TeamID == parameter1).Average(f => f.Valuation);
            double? level = valuations == null ? 0 : valuations;

            //Actualizamos el equipo.
            team.Level = level;
            db.Entry(team).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateLvlPlace(int parameter1)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            var place = db.Places.Find(parameter1);
            //Recuperamos la media de las valoraciones del lugar.
            var valuations = db.FeedBacks.Where(f => f.PlaceID == parameter1).Average(f => f.Valuation);
            double? level = valuations == null ? 0 : valuations;

            //Actualizamos el lugar.
            place.ValuationMed = level;

            db.Entry(place).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, "");
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalPlayerCommentsCount(int parameter1)
        {
            int numComments = 0;
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            numComments = db.FeedBacks.Where(f => f.PlayerID == parameter1 && f.Description != null).Count();
           
            return BuildSuccesResult(HttpStatusCode.OK, numComments);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalPlaceCommentsCount(int parameter1)
        {
            int numComments = 0;
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            numComments = db.FeedBacks.Where(f => f.PlaceID == parameter1 && f.Description != null).Count();
            
            return BuildSuccesResult(HttpStatusCode.OK, numComments);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalGameCommentsCount(int parameter1)
        {
            int numComments = 0;
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            numComments = db.FeedBacks.Where(f => f.GameID == parameter1 && f.Description != null).Count();
            
            return BuildSuccesResult(HttpStatusCode.OK, numComments);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlaceCommentsPagination(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*placeId*/)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            List<FeedBack> resultFeedBackList = db.FeedBacks.Where(f => f.PlaceID == parameter3 && f.Description != null)
                                 .OrderBy(f => f.FeedBackDate)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();

            if (resultFeedBackList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no FeedBacks on this page");
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultFeedBackList);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlayerCommentsPagination(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*playerId*/)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            List<FeedBack> resultFeedBackList = db.FeedBacks.Where(f => f.PlayerID == parameter3 && f.Description != null)
                                 .OrderBy(f => f.FeedBackDate)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();

            if (resultFeedBackList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no FeedBacks on this page");
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultFeedBackList);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetGameCommentsPagination(int parameter1/*page*/, int parameter2/*rows*/, int parameter3/*gameId*/)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            List<FeedBack> resultFeedBackList = db.FeedBacks.Where(f => f.GameID == parameter3 && f.Description != null)
                                 .OrderBy(f => f.FeedBackDate)
                                 .Skip(parameter1 * parameter2)
                                 .Take(parameter2)
                                 .ToList();

            if (resultFeedBackList.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There are no FeedBacks on this page");
            }
            return BuildSuccesResult(HttpStatusCode.OK, resultFeedBackList);
        }


    }
}