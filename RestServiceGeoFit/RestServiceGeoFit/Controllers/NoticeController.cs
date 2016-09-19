using RestServiceGeoFit.Models2;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RestServiceGeoFit.Controllers
{
    public class NoticeController : BaseApiController
    {
        private AppGeoFitDBContext db = new AppGeoFitDBContext("name=AppGeoFitDBContext");
        string dataBase = "GeoFitDB";
        string authData = string.Format("{0}:{1}", Constants.UserName, Constants.PassWord);

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetNotice(int parameter1)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            Notice notice = new Notice();
            // Acces Data Base Test according to request
            if (this.ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            notice = db.Notices.Find(parameter1);
            if (notice.GameID != null)
            {
                notice.Game.Place.Games.Clear();
                notice.Game.Creator.Joineds.Clear();
                notice.Game.Place.Sport.Players.Clear();
                notice.Game.Creator.Sport.Places.Clear();
                notice.Game.Sport.Players.Clear();
                notice.Game.Creator.Sport.Teams.Clear();
                notice.Game.FeedBacks.Clear();
                notice.Game.Notices.Clear();
                notice.Game.Sport.Games.Clear();
                if (notice.Game.Team1ID != null)
                {
                    notice.Game.Team.Joineds.Clear();
                    notice.Game.Team.Games.Clear();
                }
                if (notice.Game.Team2ID != null)
                {
                    notice.Game.Team1.Joineds.Clear();
                    notice.Game.Team1.Games.Clear();
                }
            }
            notice.Sport.Players.Clear();
            notice.Messenger.Games.Clear();
            notice.Messenger.NoticesMessege.Clear();
            notice.Messenger.GamesCreated.Clear();
            if (notice.Messenger.FavoriteSportID != null)
                notice.Messenger.Sport.Players.Clear();
            notice.Receiver.Games.Clear();
            notice.Receiver.GamesCreated.Clear();
            notice.Receiver.NoticesRecive.Clear();
            if (notice.Receiver.FavoriteSportID != null)
                notice.Receiver.Sport.Players.Clear();
            notice.Sport.Notices.Clear();
            notice.Sport.Players.Clear();

            /* Game newGame = new Game();
             newGame.CreatorID = notice.Game.CreatorID;
             newGame.GameID = notice.Game.GameID;
             newGame.PlaceID = notice.Game.PlaceID;
             newGame.SportId = notice.Game.SportId;
             newGame.StartDate = notice.Game.StartDate;
             newGame.Team1ID = notice.Game.Team1ID;
             newGame.Team2ID = notice.Game.Team2ID;
             notice.Game = newGame;*/

            if (notice == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Team with id : " + parameter1 + " don't exists.");
            }

            return BuildSuccesResult(HttpStatusCode.OK, notice);
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateNotice([Bind(Include = "Type, MessengerID, ReceiverID, SportID")] Notice notice)
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
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteAllNoticeByTypeMessengerAndSport(string parameter1, int parameter2, int parameter3)
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
            List<Notice> notices = db.Notices.Where(n => n.Type.Equals(parameter1) && n.MessengerID == parameter2 && n.SportID == parameter3 && n.Accepted == null).ToList();
            if (notices == null)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "Notice not found.");
            }
            foreach (Notice n in notices)
            {
                db.Notices.Remove(n);
            }
            db.SaveChanges();
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateNotice([Bind(Include = "Type, Accepted, MessengerID, ReceiverID, SportID, GameID")] Notice notice)
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

            notice.Messenger = null;
            notice.Receiver = null;
            notice.Sport = null;
            notice.Game = null;
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
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
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

        [System.Web.Http.HttpGet]
        public HttpResponseMessage NoticeIsPending(int parameter1, int parameter2, int parameter3, string parameter4)
        {
            if (ControllerContext.Request.Headers.Authorization == null || !Encoding.UTF8.GetString(Convert.FromBase64String(ControllerContext.Request.Headers.Authorization.Parameter)).Equals(authData))
            {
                return BuildErrorResult(HttpStatusCode.Unauthorized, "Your username or password are incorrect.");
            }
            if (ControllerContext.RouteData.Route.RouteTemplate.Contains("apiTest"))
            {
                db = new AppGeoFitDBContext("name=AppGeoFitDBContextTest");
            }
            Notice notice = db.Notices.Where(n => n.ReceiverID == parameter1 && n.MessengerID == parameter2
            && n.SportID == parameter3 && n.Type == parameter4 && n.Accepted == null).FirstOrDefault<Notice>();

            if (notice == null)
            {
                return BuildSuccesResult(HttpStatusCode.OK, false);
            }
            return BuildSuccesResult(HttpStatusCode.OK, true);
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPendingNotice(int parameter1)
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
            var receiverId = new SqlParameter("@ReceiverId", parameter1);
            var nowDate = new SqlParameter("@NowDate", DateTime.Now);
            string nativeSQLQuery = @"SELECT *" +
                                    " FROM "+dataBase+".dbo.Notice" +
                                    " WHERE Accepted IS NULL AND ReceiverID = @ReceiverId " +
                                    "AND (GameID IN (SELECT GameID FROM "+dataBase+".dbo.Game WHERE EndDate < @NowDate) OR GameID IS NULL)";
            var noticeL = db.Notices.SqlQuery(nativeSQLQuery, receiverId, nowDate);
            List<Notice> noticeLReturn = noticeL.ToList(); 
            
            foreach(Notice notice in noticeLReturn)
            {
                if (notice.GameID != null)
                {
                    notice.Game.Place.Games.Clear();
                    notice.Game.Creator.Joineds.Clear();
                    notice.Game.Place.Sport.Players.Clear();
                    notice.Game.Creator.Sport.Places.Clear();
                    notice.Game.Sport.Players.Clear();
                    notice.Game.Creator.Sport.Teams.Clear();
                    notice.Game.FeedBacks.Clear();
                    notice.Game.Notices.Clear();
                    notice.Game.Sport.Games.Clear();
                    if (notice.Game.Team1ID != null)
                    {
                        notice.Game.Team.Joineds.Clear();
                        notice.Game.Team.Games.Clear();
                    }
                    if (notice.Game.Team2ID != null)
                    {
                        notice.Game.Team1.Joineds.Clear();
                        notice.Game.Team1.Games.Clear();
                    }
                }
                notice.Sport.Players.Clear();
                notice.Messenger.Games.Clear();
                notice.Messenger.NoticesMessege.Clear();
                notice.Messenger.GamesCreated.Clear();
                if(notice.Messenger.FavoriteSportID!= null)
                    notice.Messenger.Sport.Players.Clear();
                notice.Receiver.Games.Clear();
                notice.Receiver.GamesCreated.Clear();
                notice.Receiver.NoticesRecive.Clear();
                if (notice.Receiver.FavoriteSportID != null)
                    notice.Receiver.Sport.Players.Clear();
                notice.Sport.Notices.Clear();
                notice.Sport.Players.Clear();
            }            
            if (noticeLReturn.Count == 0)
            {
                return BuildErrorResult(HttpStatusCode.NotFound, "There aren't any pending notice to this player.");
            }
            return BuildSuccesResult(HttpStatusCode.OK, noticeLReturn);
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TotalNoticesCount(int parameter1)
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
            var receiverId = new SqlParameter("@ReceiverId", parameter1);
            var nowDate = new SqlParameter("@NowDate", DateTime.Now);
            string nativeSQLQuery = @"SELECT Count(NoticeID)" +
                                    " FROM "+dataBase+".dbo.Notice" +
                                    " WHERE Accepted IS NULL AND ReceiverID = @ReceiverId "+
                                    "AND (GameID IN (SELECT GameID FROM " + dataBase + ".dbo.Game WHERE EndDate < @NowDate) OR GameID IS NULL)";
                                    
            var totalNoticesCount = db.Database.SqlQuery<int?>(nativeSQLQuery, receiverId, nowDate).FirstOrDefault();
            if(totalNoticesCount == null)
                return BuildSuccesResult(HttpStatusCode.OK, 0);
            else
                return BuildSuccesResult(HttpStatusCode.OK, totalNoticesCount);
        }

    }
}