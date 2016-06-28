using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data.GameRestService
{
    public interface IGameRestService
    {
        string url { get; set; }
        Task<Game> GetGameAsync(int gameId);
        Task<int> CreateGameAsync(Game game);
        Task<bool> DeleteGameAsync(int gameId);
        Task<bool> UpdateGameAsync(Game game);
        Task<bool> FindOnTime(int playerId, string startDate, string endDate);
        Task<bool> FindOnTimeAndPlace(int placeId, DateTime startDate, DateTime endDate);
        Task<bool> FindOnTimeAndPlace(double latitude, double longitude, DateTime startDate, DateTime endDate);

    }
}
