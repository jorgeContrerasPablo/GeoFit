﻿using AppGeoFit.BusinessLayer.Exceptions;
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

        public Team GetTeam(int teamId)
        {
            Team returnTeam = new Team();
            try
            {
                returnTeam = teamRestService.GetTeamAsync(teamId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is TeamNotFoundException)
                    {
                        throw new TeamNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);

                }
            }
            return returnTeam;
        }

        public int CreateTeam(Team team, Player player)
        {
            int teamRecived = 0;
            int teamCaptainRecived = 0;
            int teamCreatedId = 0;
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
                throw new AlreadyCaptainOnSportException("The player: " + player.PlayerNick + " is already a captian on this sport.");
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
                teamCreatedId = teamRestService.CreateTeamAsync(team).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is Exception)
                        throw new Exception(ex.Message);

                }
            }
            teamRestService.AddPlayer(teamCreatedId, player.PlayerId, true);

            return teamCreatedId;
        }

        public bool DeleteTeam(int teamId)        {            

            bool isDelete = false;
            Team team = teamRestService.GetTeamAsync(teamId).Result;
            Player teamCaptain = teamRestService.GetCaptainAsync(teamId).Result;
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
            noticeRestService.DeleteAllNoticeByTypeMessengerAndSport(Constants.TEAM_ADD_PLAYER, teamCaptain.PlayerId, team.SportID);
            Notice notice = new Notice();
            foreach (Joined j in team.Joineds)
            {
                if (j.PlayerID != teamCaptain.PlayerId)
                {
                    notice.MessengerID = teamCaptain.PlayerId;
                    notice.ReceiverID = j.PlayerID;
                    notice.SportID = team.SportID;
                    notice.Type = Constants.TEAM_DELETED;
                    noticeRestService.CreateNoticeAsync(notice);
                }
            }
            return isDelete;

        }

        public bool UpdateTeam(Team team, Player player)
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
                    throw new AlreadyCaptainOnSportException("The player: " + player.PlayerNick + " is already a captian on " + team.Sport.SportName + ".");
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
            return true;
        }

        public bool AddPlayer(int playerId, int teamId)
        {
            bool succes = false;
            try
            {
                succes = teamRestService.AddPlayer(teamId, playerId, false).Result;
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
                    {
                        if (ex is TeamNotFoundException)
                        {
                            throw new TeamNotFoundException(ex.Message);
                        }
                        else
                            throw new Exception(ex.Message);
                    }
                }
            }
            return succes;
        }

        public int SendNoticeAddPlayer(string playerNick, Team team)
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
                        playerToAdd = playerRestService.GetPlayerAsync(playerIdFind).Result;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            //Comprobamos que el jugador no este ya pendiente de una petición nuestra para ser agregado
            try
            {
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
            
            notice.MessengerID = captainTeamId;
            notice.ReceiverID = playerIdFind;
            notice.Type = Constants.TEAM_ADD_PLAYER;
            notice.SportID = team.SportID;
            notice.Accepted = null;
            return noticeRestService.CreateNoticeAsync(notice).Result;
        }

        public bool RemovePlayer(int teamId, int playerId)
        {
            Player playerToRemove = new Player();
            Player captain = teamRestService.GetCaptainAsync(teamId).Result;
            Team team = teamRestService.GetTeamAsync(teamId).Result;
            // Comprobación capitán, lanzamos excepcion, pues deberemos elegir otro.
            // Comprobación ultimo jugador, borraremos el equipo.
            if(playerId == captain.PlayerId)
            {
                if (team.Joineds.Count == 1)
                    throw new OnlyOnePlayerException("You are the last player on the team: " + team.TeamName + ". You want to remove it?");
                else
                    throw new CaptainRemoveException("You are the captain. You need to chose other captain.");
            }
            //Eliminamos el jugador del equipo.
            return teamRestService.RemovePlayer(teamId, playerId, false).Result;

            
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

        public int FindTeamByNameOnSports(string teamName, int sportId)
        {
            int teamId = 0;
            try
            {
                teamId = teamRestService.FindTeamByNameOnSports(teamName, sportId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is TeamNotFoundException)
                    {
                        throw new TeamNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return teamId;
        }

        public Player GetCaptainAsync(int teamId)
        {
            Player returnPlayer = null;
            try
            {
                returnPlayer = teamRestService.GetCaptainAsync(teamId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is TeamNotFoundException)
                    {
                        throw new TeamNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return returnPlayer;
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
            throw new AlreadyCaptainOnSportException("The player: " + joineds.ElementAt(joineds.FindIndex(j => j.PlayerID == captainId)).Player.PlayerNick + " is already a captian for this sport.");
        }

        public ICollection<Player> GetAllPlayersPendingToAdd(int messengerId, int sportId, string type)
        {
            ICollection<Player> listPlayers = null;
            try
            {
                listPlayers = teamRestService.GetAllPlayersPendingToAdd(messengerId, sportId, type).Result;
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

        public bool IsOnTeam(int teamId, int playerId)
        {

            bool isOnTeam = false;
            try
            {
                isOnTeam = teamRestService.IsOnTeam(teamId, playerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                  throw new Exception(ex.Message);
                }
            }
            return isOnTeam;
        }
    }
}

