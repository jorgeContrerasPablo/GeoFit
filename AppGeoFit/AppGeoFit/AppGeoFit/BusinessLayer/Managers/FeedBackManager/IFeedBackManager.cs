using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Managers.FeedBackManager
{
    public interface IFeedBackManager
    {
        IFeedBackManager InitiateServices(bool test);
        void CreateFeedBacks(List<FeedBack> pendingFeedBacks, int noticeId);
        FeedBack GetFeedBack(int feedBackId);
        bool UpdateFeedBack(FeedBack feedBack);
        bool DeleteFeedBack(int feedBackId);
        int TotalPlayerCommentsCount(int playerId);
        int TotalPlaceCommentsCount(int placeId);
        int TotalGameCommentsCount(int gameId);
        List<FeedBack> GetPlaceCommentsPagination(int pages, int rows, int placeId);
        List<FeedBack>  GetPlayerCommentsPagination(int pages, int rows, int playerId);
        List<FeedBack>  GetGameCommentsPagination(int pages, int rows, int gameId);
    }
}
