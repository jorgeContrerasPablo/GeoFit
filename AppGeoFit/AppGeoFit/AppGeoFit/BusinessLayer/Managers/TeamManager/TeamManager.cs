using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
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

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.TeamManager.TeamManager))]
namespace AppGeoFit.BusinessLayer.Managers.TeamManager
{
    
    public class TeamManager : ITeamManager
    {
        ITeamRestService teamRestService;
        IPlayerRestService playerRestService;
        INoticeRestService noticeRestService;

        public TeamManager(){}

        public ITeamManager InitiateServices(bool test)
        {
            teamRestService = DependencyService.Get<ITeamRestService>();
            teamRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<IPlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            noticeRestService = DependencyService.Get<INoticeRestService>();
            noticeRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }

        public Task<Team> GetTeam(int teamId)
        {
            return teamRestService.GetTeamAsync(teamId);
        }

        public Task<bool> CreateTeam(Team team, Player player)
        {
            int teamRecived = 0;
            int teamCaptainRecived = 0;
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
                teamCaptainRecived = playerRestService.FindCaptainOnSportsAsync(player.PlayerId, team.SportID).Result;
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

        public bool DeleteTeam(int teamId)        {            

            bool isDelete = false;
            try
            {
                isDelete = teamRestService.DeleteTeamAsync(teamId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return isDelete;

        }

        public Task<Boolean> UpdateTeam(Team team, Player player)
        {
            int teamRecivedId = 0;
            int teamCaptainRecived = 0;
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
                teamCaptainRecived = playerRestService.FindCaptainOnSportsAsync(player.PlayerId, team.SportID).Result;
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
            //TODO CAPA SEPARADA???
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

        public Task<Boolean> AddPlayer(int playerId, int teamId)
        {
            return teamRestService.AddPlayer(teamId, playerId, false);
        }

        public Task<int> SendNoticeAddPlayer(string playerNick, Team team)
        {
            Player playerToAdd = new Player();
            Notice notice = new Notice();
            int captainTeamId = teamRestService.GetCaptainAsync(team.TeamID).Result.PlayerId;
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
                throw new DuplicatePlayerOnTeamException("The player: " + playerNick + " is already joined at this team.");
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
            //Comprobamos que el jugador no este ya pendiente de una petición nuestra para ser agregado
            try
            {
                //int noticeId = noticeRestService.FindNoticeAsync(playerIdFind, captainTeamId, team.SportID, Constants.TEAM_ADD_PLAYER).Result;
                notice.NoticeID = noticeRestService.FindNoticeAsync(playerIdFind, captainTeamId, team.SportID, Constants.TEAM_ADD_PLAYER).Result;
                notice = noticeRestService.GetNoticeAsync(notice.NoticeID).Result;
                if (notice.Accepted == null)
                    throw new DuplicateNoticeException("You have already send a petition to this player");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NoticeNotFoundException) { }
                    else
                        throw new Exception(ex.Message);
                }
            }

            //notice.Messenger = 
            notice.MessengerID = captainTeamId;
            notice.ReceiverID = playerIdFind;
            notice.Type = Constants.TEAM_ADD_PLAYER;
            notice.SportID = team.SportID;
            notice.Accepted = null;
            return noticeRestService.CreateNoticeAsync(notice);
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
        public List<Sport> GetSports()
        {
            List<Sport> sportList = new List<Sport>();
            try
            {
                sportList = teamRestService.GetSports().Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is SportsNotFoundException)
                    {
                        throw new SportsNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return sportList;
        }

        public Task<int> FindTeamByNameOnSports(string teamName, int sportId)
        {
            return teamRestService.FindTeamByNameOnSports(teamName, sportId);
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
                var teamCaptainRecived = playerRestService.FindCaptainOnSportsAsync(newCaptainId, team.SportID).Result;                
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
            throw new AlreadyCaptainOnSport("The player: " + joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == captainId)).Player.PlayerNick + " is already a captian for this sport.");
        }

        public Task<ICollection<Player>> GetAllPlayersPendingToAdd(int messengerId, int sportId, string type)
        {
            Task<ICollection<Player>> listPlayers = null;
            try
            {
                listPlayers = teamRestService.GetAllPlayersPendingToAdd(messengerId, sportId, type);
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NotPendingPlayersToAddException)
                    {
                        throw new NotPendingPlayersToAddException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return listPlayers;       
        }
    }
}

