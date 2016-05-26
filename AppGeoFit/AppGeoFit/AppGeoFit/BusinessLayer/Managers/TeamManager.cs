using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.TeamRestService;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                playerRecived = playerRestService.FindCaptainOnSportsAsync(player.PlayerId, team.SportID).Result;
                //throw new DuplicateCaptainException("");
                throw new AlreadyCaptainOnSport("The player: " + player.PlayerNick + " is already a captian on "+team.Sport.SportName+".");
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

        public Task<Boolean> UpdateTeam(Team team, Player player)
        {
            int teamRecivedId = 0;
            int playerRecived = 0;
            bool teamUpdate = false;
            bool sameCaptain = false;
            Player actualCaptain = teamRestService.GetCaptainAsync(team.TeamID).Result;
            try
            {
                teamRecivedId = teamRestService.FindTeamByNameOnSports(team.TeamName, team.SportID).Result;
                if (team.TeamID != teamRecivedId)
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
                playerRecived = playerRestService.FindCaptainOnSportsAsync(player.PlayerId, team.SportID).Result;
                if (player.PlayerId != actualCaptain.PlayerId)
                    throw new AlreadyCaptainOnSport("The player: " + player.PlayerNick + " is already a captian on " + team.Sport.SportName + ".");
                else
                    sameCaptain = true;
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
            //Si mandamos le entidad Team con los Joineds actuales
            //El dbContext intentará agregarlos. Por lo que hay que
            //"Limpiarlos"
            team.Joineds.Clear();
            try
            {
                teamUpdate = teamRestService.UpdateTeamAsync(team).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is Exception)
                        throw new Exception(ex.Message);
                }
            }
            if (!sameCaptain)
                UpdateCaptain(actualCaptain.PlayerId, player.PlayerId, team.TeamID);              
            return new Task<Boolean>(() => true);
        }

        public Task<Boolean> AddPlayer(string playerNick, Team team)
        {
            Player playerToAdd = new Player();
            //Comprobamos que el equipo no está completo ya.
            int playerIdFind = 0;
            if (team.Joineds.Count >= team.Sport.NumPlayers)
            {
                throw new MaxPlayerOnTeamException("The Team: " + team.TeamName + " is full.");
            }
            //Comprobamos que exista el jugador.
            try
            {
                playerIdFind = playerRestService.FindPlayerByNickAsync(playerNick).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException)
                    {
                        throw new PlayerNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            //Buscamos el jugador en el equipo, si existe lanzamos una excepción.
            try
            {
                playerToAdd = playerRestService.FindPlayerOnTeamAsync(playerNick, team.TeamID).Result;
                throw new DuplicatePlayerOnTeamException("The player: "+playerNick+" is already joined at this team.");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NotFoundPlayerOnTeamException)
                    {
                        //TODO ARREGLAR ESTO
                        playerToAdd = playerRestService.GetPlayerAsync(playerIdFind).Result;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return teamRestService.AddPlayer(team.TeamID, playerToAdd.PlayerId, false);
        }

        public Task<Boolean> RemovePlayer(int teamId, int playerId)
        {
            Player playerToRemove = new Player();
            Player captain = teamRestService.GetCaptainAsync(teamId).Result;
            Team team = teamRestService.GetTeamAsync(teamId).Result;
            // Comprobación capitán, anzamos excepcion, pues deberemos elegir otro.
            // Comprobación ultimo jugador, borraremos el equipo.
            if(playerId == captain.PlayerId)
            {
                if (team.Joineds.Count == 1)
                    throw new OnlyOnePlayerException("You are the last player on the team: " + team.TeamName + ". You want to remove it?");
                else
                    throw new CaptainRemoveException("You are the captain. You need to chose other captain.");
            }
            //Eliminamos el jugador del equipo.
            return teamRestService.RemovePlayer(teamId, playerId, false);

            
        }
        public Task<ICollection<Sport>> GetSports()
        {
            return teamRestService.GetSports();
        }

        public Task<int> FindTeamByNameOnSports(string teamId, int sportId)
        {
            return teamRestService.FindTeamByNameOnSports(teamId, sportId);
        }

        public Task<Player> GetCaptainAsync(int teamId)
        {
            return teamRestService.GetCaptainAsync(teamId);
        }

        public Player UpdateCaptain(int captainId, int newCaptainId, int teamId)
        {
            List<Joined> joineds = new List<Joined>();
            Team team = new Team();
            try
            {
                team = teamRestService.GetTeamAsync(teamId).Result;
                joineds = team.Joineds.ToList();
                var id = playerRestService.FindCaptainOnSportsAsync(newCaptainId, team.SportID).Result;                
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is CaptainNotFoundException)
                    {
                        Player captian = joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == captainId)).Player;
                        teamRestService.RemovePlayer(team.TeamID, captian.PlayerId, true);
                        Player newCaptian = joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == newCaptainId)).Player;
                        teamRestService.RemovePlayer(team.TeamID, newCaptian.PlayerId, false);
                        teamRestService.AddPlayer(team.TeamID, captian.PlayerId, false);
                        teamRestService.AddPlayer(team.TeamID, newCaptian.PlayerId, true);
                        return joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == newCaptainId)).Player;
                    }
                    if (ex is Exception)
                        throw new Exception(ex.Message);
                }
            }
            throw new AlreadyCaptainOnSport("The player: " + joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == captainId)).Player.PlayerNick + " is already a captian on this sport.");
        }
    }
}

