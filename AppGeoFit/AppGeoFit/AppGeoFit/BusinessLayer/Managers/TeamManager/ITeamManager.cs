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
        Team GetTeam(int teamId);
        int CreateTeam(Team team, Player player);
        bool DeleteTeam(int teamId);
        bool UpdateTeam(Team team, Player player);
        bool AddPlayer(int teamId, int playerId);
        bool RemovePlayer(int teamId, int playerId);
        List<Sport> GetSports();
        int FindTeamByNameOnSports(string teamName, int sportId);
        Player GetCaptainAsync(int teamId);
        Player UpdateCaptain(int captainId, int newCaptainId, int teamId);
        int SendNoticeAddPlayer(string playerNick, Team team);
        ICollection<Player> GetAllPlayersPendingToAdd(int messengerId, int sportId, string type);
        bool IsOnTeam(int teamId, int playerId);

    }
}
