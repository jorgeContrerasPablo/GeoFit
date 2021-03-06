﻿using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGeoFit.BusinessLayer.Managers.PlayerManager
{
    public interface IPlayerManager
    {
        IPlayerManager InitiateServices(bool test);
        Player GetPlayer(int playerId);
        ICollection<Player> GetAll();
        int CreatePlayer(Player player);
        bool DeletePlayer(int playerId);
        bool UpdatePlayer(Player player);
        Player Authentication(string nickOrMail, string password);
        int FindPlayerByMail(string nickOrMail, string post);
        int FindPlayerByNick(string nickOrMail);
        ICollection<Team> FindTeamsJoined(int playerId, int sportId);
        Team FindTeamCaptainOnSport(int playerId, int SportId);
        List<Player> FindAllPlayersOnOurTeams(int playerId, int sportId);
        List<Game> GetActualGames(int page, int rows, int playerId, int sportId);
        int TotalGamesCount(int playerId, int sportId);
        void Session(int playerId);
        void OutSession(int playerId);
    }
}
