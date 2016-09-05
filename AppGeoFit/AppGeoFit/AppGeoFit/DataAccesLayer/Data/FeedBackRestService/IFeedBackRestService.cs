using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data.FeedBackRestService
{
    interface IFeedBackRestService
    {
        string url { get; set; }
        Task<int> CreateFeedBackAsync(FeedBack feedBack);
        Task<FeedBack> GetFeedBackAsync(int feedBackId);
        Task<bool> UpdateFeedBackAsync(FeedBack feedBack);
        Task<bool> DeleteFeedBackAsync(int feedBackId);
        void UpdateLvlAndOnTimePlayer(int playerId);
        void UpdateLvlTeam(int teamId);
        void UpdateLvlPlace(int placeId);
        Task<int> TotalPlayerCommentsCount(int playerId);
        Task<int> TotalPlaceCommentsCount(int placeId);
        Task<int> TotalGameCommentsCount(int gameId);
        Task<ICollection<FeedBack>> GetPlaceCommentsPagination(int pages, int rows, int playerId);
        Task<ICollection<FeedBack>> GetPlayerCommentsPagination(int pages, int rows, int placeId);
        Task<ICollection<FeedBack>> GetGameCommentsPagination(int pages, int rows, int gameId);
    }
}
