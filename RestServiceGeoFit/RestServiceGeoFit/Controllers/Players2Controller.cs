﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using RestServiceGeoFit.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using RestServiceGeoFit.Models.Managers.Player.Exceptions;
using RestServiceGeoFit.Models2;
using System.Web.Mvc;

namespace RestServiceGeoFit.Controllers
{
    public class Players2Controller : BaseApiController
    {
        static readonly bool test = true;
        //PlayerManager playerManager = new PlayerManager(!test);
        private AppGeoFitDBContext db = new AppGeoFitDBContext();

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPlayer(int parameter1)
        {
            Player player = new Player();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //playerManager = new PlayerManager(test);
            }

            player = db.Players.Find(parameter1);

            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id : "+parameter1+" don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, player);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreatePlayer([Bind(Include = "Password,PlayerNick,PlayerName,LastName,PhoneNum,PlayerMail,PhotoID,Level,MedOnTime,FavoriteSportID,PlayerSesion")] Player player)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //playerManager = new PlayerManager(test);
            }
            if (ModelState.IsValid)
            {
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
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //   playerManager = new PlayerManager(test);
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
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //   playerManager = new PlayerManager(test);
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
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //  playerManager = new PlayerManager(test);
            }
            parameter1 = parameter1 + "." + parameter2;
            Player player = db.Players.Where(p => p.PlayerMail == parameter1).FirstOrDefault<Player>();
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with email: "+parameter1+" don't exists.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerID);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindPlayerByNick(string parameter1)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //  playerManager = new PlayerManager(test);
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
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //  playerManager = new PlayerManager(test);
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }
            player.PlayerSesion = true;
            db.Entry(player).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage OutSession(int parameter1)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //  playerManager = new PlayerManager(test);
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }
            player.PlayerSesion = false;
            db.Entry(player).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, "");
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsOnSession(int parameter1)
        {
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                //  playerManager = new PlayerManager(test);
            }

            Player player = db.Players.Find(parameter1);
            if (player == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with id: " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, player.PlayerSesion);
        }
    }
}