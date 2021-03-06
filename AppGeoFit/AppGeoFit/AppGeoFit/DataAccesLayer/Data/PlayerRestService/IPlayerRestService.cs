﻿using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data.PlayerRestService
{
    public interface IPlayerRestService
    {
        string url { get; set; }
        Task <Player> GetPlayerAsync(int playerId);
        Task<ICollection<Player>> GetAllAsync();
        Task <int> CreatePlayerAsync(Player player);
        Task <Boolean> DeletePlayerAsync(int playerId);
        Task <Boolean> UpdatePlayerAsync(Player player);
        Task <int> FindPlayerByMailAsync(string nickOrMail, string post);
        Task<int> FindPlayerByNickAsync(string nickOrMail);
        void Session(int playerId);
        void OutSession(int playerId);
        Task<int> FindCaptainOnSportsAsync(int playerId, int sportId);
        Task<ICollection<Team>> FindTeamsJoinedAsync(int playerId, int sportId);
        Task<Player> FindPlayerOnTeamAsync(string playerNick, int teamId);
        Task<ICollection<Game>> GetActualGames(int page, int rows, int playerId, int sportId);
        Task<int> TotalGamesCount(int playerId, int sportId);     
    }
}
