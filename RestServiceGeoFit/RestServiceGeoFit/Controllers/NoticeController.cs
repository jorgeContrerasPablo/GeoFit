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
    public class NoticeController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetNotice(int parameter1)
        {
            Notice notice = new Notice();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }

            notice = db.Notices.Find(parameter1);

            if (notice == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id : " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, notice);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateNotice([Bind(Include = "Type, MessengerID, ReceiverID, SportID")] Notice notice)
        {
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                notice.Messenger = db.Players.Find(notice.MessengerID);
                notice.Receiver = db.Players.Find(notice.ReceiverID);
                notice.Sport = db.Sports.Find(notice.SportID);
                db.Notices.Add(notice);
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, notice.NoticeID);
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteNotice(int parameter1)
        {
            bool response = false;
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Notice notice = db.Notices.Find(parameter1);
            if (notice == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Notice with id: " + parameter1 + " don't exists.");
            }
            db.Notices.Remove(notice);
            db.SaveChanges();

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateNotice([Bind(Include = "Type, MessengerID, ReceiverID, SportID")] Notice notice)
        {
            // Acces Data Base Test according to request
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            if (ModelState.IsValid)
            {
                db.Entry(notice).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                return BuildErrorResult(HttpStatusCode.BadRequest, "Not valid parameter.");

            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage FindNotice(int parameter1, int parameter2, int parameter3, string parameter4)
        {
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Notice notice = db.Notices.Where(n => n.ReceiverID == parameter1 && n.MessengerID == parameter2
            && n.SportID == parameter3 && n.Type == parameter4).FirstOrDefault<Notice>();
            if (notice == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Player with nick: " + parameter1 + " don't exists.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, notice.NoticeID);
        }
    }
}