using AppGeoFit.DataAccesLayer.Data.FeedBackRestService;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using Xamarin.Forms;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.TeamRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.FeedBackRestService.Exceptions;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.FeedBackManager.FeedBackManager))]
namespace AppGeoFit.BusinessLayer.Managers.FeedBackManager
{
    public class FeedBackManager : IFeedBackManager
    {
        IFeedBackRestService feedBackRestService;
        IPlayerRestService playerRestService;
        ITeamRestService teamRestService;
        INoticeRestService noticeRestService;

        public IFeedBackManager InitiateServices(bool test)
        {
            feedBackRestService = DependencyService.Get<IFeedBackRestService>();
            feedBackRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<IPlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            teamRestService = DependencyService.Get<ITeamRestService>();
            teamRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            noticeRestService = DependencyService.Get<INoticeRestService>();
            noticeRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }
        public void CreateFeedBacks(List<FeedBack> pendingFeedBacks, int noticeId)
        {
            foreach(FeedBack f in pendingFeedBacks)
            {
                try
                {
                    var x = feedBackRestService.CreateFeedBackAsync(f).Result;
                    if (f.PlayerID != null)
                        feedBackRestService.UpdateLvlAndOnTimePlayer((int)f.PlayerID);
                    if (f.TeamID != null)
                        feedBackRestService.UpdateLvlTeam((int)f.TeamID);
                    if (f.PlaceID != null)
                        feedBackRestService.UpdateLvlPlace((int)f.PlaceID);
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        throw new Exception(ex.Message);
                    }
                }               
            }
            Notice notice = new Notice();
            try
            {
                notice = noticeRestService.GetNoticeAsync(noticeId).Result;
                notice.Accepted = true;
                var succes = noticeRestService.UpdateNoticeAsync(notice).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NoticeNotFoundException)
                    {
                        throw new NoticeNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
        }

        public FeedBack GetFeedBack(int feedBackId)
        {
            FeedBack feedBackResult = new FeedBack();
            try
            {
                feedBackResult = feedBackRestService.GetFeedBackAsync(feedBackId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is FeedBackNotFoundException)
                    {
                        throw new FeedBackNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return feedBackResult;
        }

        public bool UpdateFeedBack(FeedBack feedBack)
        {
            bool succes = false;
            try
            {
                succes = feedBackRestService.UpdateFeedBackAsync(feedBack).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return succes;
        }

        public bool DeleteFeedBack(int feedBackId)
        {
            bool succes = false;
            try
            {
                succes = feedBackRestService.DeleteFeedBackAsync(feedBackId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is FeedBackNotFoundException)
                    {
                        throw new FeedBackNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return succes;
        }

        public int TotalPlayerCommentsCount(int playerId)
        {
            int count = 0;
            try
            {
                count = feedBackRestService.TotalPlayerCommentsCount(playerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return count;
        }

        public int TotalPlaceCommentsCount(int placeId)
        {
            int count = 0;
            try
            {
                count = feedBackRestService.TotalPlaceCommentsCount(placeId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return count;
        }

        public int TotalGameCommentsCount(int gameId)
        {
            int count = 0;
            try
            {
                count = feedBackRestService.TotalGameCommentsCount(gameId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return count;
        }

        public List<FeedBack> GetPlaceCommentsPagination(int pages, int rows, int placeId)
        {
            List<FeedBack> returnCommentList = new List<FeedBack>();
            try
            {
                returnCommentList = feedBackRestService.GetPlaceCommentsPagination(pages, rows, placeId).Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is FeedBackNotFoundException)
                    {
                        throw new FeedBackNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return returnCommentList;
        }

        public List<FeedBack> GetPlayerCommentsPagination(int pages, int rows, int playerId)
        {
            List<FeedBack> returnCommentList = new List<FeedBack>();
            try
            {
                returnCommentList = feedBackRestService.GetPlayerCommentsPagination(pages, rows, playerId).Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is FeedBackNotFoundException)
                    {
                        throw new FeedBackNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return returnCommentList;
        }

        public List<FeedBack> GetGameCommentsPagination(int pages, int rows, int gameId)
        {
            List<FeedBack> returnCommentList = new List<FeedBack>();
            try
            {
                returnCommentList = feedBackRestService.GetGameCommentsPagination(pages, rows, gameId).Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is FeedBackNotFoundException)
                    {
                        throw new FeedBackNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return returnCommentList;
        }
    }
}
