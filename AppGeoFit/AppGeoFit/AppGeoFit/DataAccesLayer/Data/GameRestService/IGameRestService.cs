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
        Task<int> FindOnTime(int playerId, string startDate, string endDate);
        Task<bool> FindOnTimeAndPlace(int placeId, DateTime startDate, DateTime endDate);
        Task<bool> FindOnTimeAndPlace(double latitude, double longitude, DateTime startDate, DateTime endDate);
        Task<ICollection<Game>> GetAllPaginationByDistance(int pages, int rows, int sportId, double longitude, double latitude);
        Task<ICollection<Game>> GetPaginationByTime(int pages, int rows, int sportId);
        Task<ICollection<Game>> GetAllPaginationByNumPlayers(int pages, int rows, int sportId);
        Task<int> TotalGamesCount(int sportId);
        Task<bool> IsPlayerOnGame(int gameId, int playerId);
        Task<bool> AddPlayer(int gameId, int playerId);
        Task<bool> AddPlayers(Game game);
        Task<bool> RemovePlayers(Game game); 
        Task<int> FindTeamOnTime(int teamId, string startDate, string endDate);
        Task<ICollection<Player>> GetParticipatePlayers(int gameId);
        Task<ICollection<Place>> GetPlacesBySport(int sportId);
        Task<ICollection<Place>> GetPlacesWithOutSport();
    }

}

