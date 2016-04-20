﻿using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService
{
    public interface ITeamRestService
    {
        string url { get; set; }
        Task<Team> GetTeamAsync(int teamId);
        Task<int> CreateTeamAsync(Team team);
        Task<Boolean> DeleteTeamAsync(int teamId);
        Task<Boolean> UpdateTeamAsync(Team team);
        Task<Boolean> AddPlayer(int teamId, int playerId, bool captain);
        Task<ICollection<Sport>> GetSports();
        Task<int> FindTeamByNameOnSports(string teamName, int sportId);
    }
}