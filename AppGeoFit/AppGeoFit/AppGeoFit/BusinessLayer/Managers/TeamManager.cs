using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.TeamRestService;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppGeoFit.BusinessLayer.Managers
{
    public class TeamManager
    {
        readonly ITeamRestService teamRestService;
        readonly IPlayerRestService playerRestService;
        public TeamManager(bool test)
        {
            teamRestService = DependencyService.Get<TeamRestService>();
            teamRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<PlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
        }

        public Task<Team> GetTeam(int teamId)
        {
            return teamRestService.GetTeamAsync(teamId);
        }

        public Task<bool> CreateTeam(Team team, Player player)
        {
            int teamRecived = 0;
            int playerRecived = 0;
            int teamCreated = 0;
            try
            {
                teamRecived = teamRestService.FindTeamByNameOnSports(team.TeamName, team.SportID).Result;
                throw new DuplicateTeamNameException("Team with name: " + team.TeamName + "already exists.");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is TeamNotFoundException) { }
                    else
                        throw new Exception(ex.Message);
                                          
                }
            }
            try
            {
                playerRecived = playerRestService.FindCaptainOnSports(player.PlayerId, team.SportID).Result;
                throw new DuplicateCaptainException("");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is CaptainNotFoundException) { }
                    else 
                        throw new Exception(ex.Message);
                    
                }
            }
            try
            {
                teamCreated = teamRestService.CreateTeamAsync(team).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is Exception)
                        throw new Exception(ex.Message);

                }
            }
            return teamRestService.AddPlayer(teamCreated, player.PlayerId, true);
        }

        public Task<Boolean> DeleteTeam(int teamId)
        {
            return teamRestService.DeleteTeamAsync(teamId);
        }

        public Task<Boolean> UpdateTeam(Team team)
        {
            return teamRestService.UpdateTeamAsync(team);
        }

        public Task<Boolean> AddPlayer(int teamId, int playerId, bool captain)
        {
            return teamRestService.AddPlayer(teamId, playerId, captain);
        }

        public Task<ICollection<Sport>> GetSports()
        {
            return teamRestService.GetSports();
        }

        public Task<int> FindTeamByNameOnSports(string teamId, int sportId)
        {
            return teamRestService.FindTeamByNameOnSports(teamId, sportId);
        }
    }
}

