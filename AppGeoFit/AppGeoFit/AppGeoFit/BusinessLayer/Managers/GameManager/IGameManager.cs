using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Managers.GameManager
{
    public interface IGameManager
    {
        IGameManager InitiateServices(bool test);
        Game GetGame(int gameId);
        bool DeleteGame(int gameId);
        int CreateGame(Game game);
        List<Game> GetAllPagination(int pages, int rows, int sportId, string selectedType, double longitude, double latitude);
        int TotalGamesCount(int sportId);
        bool AddPlayer(int gameId, int playerId);
        bool RemovePlayer(int gameId, int playerId);
        bool IsPlayerOnGame(int gameId, int playerId);
        bool AddTeam(int gameId, List<Player> playerList, int teamId);
        bool TeamVisitorOnGame(int gameId);
        int PlayerGameOnTime(int playerId, Game game);
        int TeamGameOnTime(int teamId, Game game);
        List<Player> GetParticipatePlayers(int gameId);
        bool UpdateGame(Game game);
        List<Place> GetPlaces(int SportId);
    }
}
