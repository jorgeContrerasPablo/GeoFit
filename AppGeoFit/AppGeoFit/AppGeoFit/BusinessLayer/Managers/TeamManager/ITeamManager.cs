using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.BusinessLayer.Managers.TeamManager
{
    public interface ITeamManager
    {
        ITeamManager InitiateServices(bool test);
        Task<Team> GetTeam(int teamId);
        Task<bool> CreateTeam(Team team, Player player);
        Task<Boolean> DeleteTeam(int teamId);
        Task<Boolean> UpdateTeam(Team team, Player player);
        Task<Boolean> AddPlayer(int teamId, int playerId);
        Task<Boolean> RemovePlayer(int teamId, int playerId);
        Task<ICollection<Sport>> GetSports();
        Task<int> FindTeamByNameOnSports(string teamId, int sportId);
        Task<Player> GetCaptainAsync(int teamId);
        Player UpdateCaptain(int captainId, int newCaptainId, int teamId);
        Task<int> SendNoticeAddPlayer(string playerNick, Team team);
        Task<ICollection<Player>> GetAllPlayersPendingToAdd(int messengerId, int sportId, string type);

    }
}
