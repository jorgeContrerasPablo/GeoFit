using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.BusinessLayer.Managers.GameManager;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.TeamRestService;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using DevOne.Security.Cryptography.BCrypt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.PlayerManager.PlayerManager))]
namespace AppGeoFit.BusinessLayer.Managers.PlayerManager
{    
    public class PlayerManager : IPlayerManager
    {
        IPlayerRestService playerRestService;
        ITeamRestService teamRestService;
        IGameManager gameManager;
        INoticeRestService noticeRestService;

        public PlayerManager(){}

        public IPlayerManager InitiateServices(bool test)
        {
            teamRestService = DependencyService.Get<ITeamRestService>();
            teamRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<IPlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            noticeRestService = DependencyService.Get<INoticeRestService>();
            noticeRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            gameManager = DependencyService.Get<IGameManager>().InitiateServices(test);
            return this;
        }

        public Player GetPlayer(int playerId)
        {
            Player playerReturn = new Player();
            try
            {
                playerReturn = playerRestService.GetPlayerAsync(playerId).Result;
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
            return playerReturn;
        }

        public ICollection<Player> GetAll()
        {
            ICollection<Player> returnCollection = new Collection<Player>();
            try {
                returnCollection= playerRestService.GetAllAsync().Result;
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
            return returnCollection;
        }

        public int CreatePlayer(Player player)
        {
            string[] finalEmail = splitFunction( player.PlayerMail );
            int reciveIdEmail = 0;
            int reciveIdNick = 0;

            // Comprobamos mail duplicado
            try
            {
                reciveIdEmail = playerRestService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
                throw new DuplicatePlayerMailException("Player with mail: " + player.PlayerMail + " already exists.");

            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException){}
                    else
                        throw new Exception(ex.Message);              
                }
            }

            // Comprobamos nick duplicado
            try
            {
                reciveIdNick = playerRestService.FindPlayerByNickAsync(player.PlayerNick).Result;            
                throw new DuplicatePlayerNickException("Player with nick: " + player.PlayerNick + " already exists.");

            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException){}
                    else
                        throw new Exception(ex.Message);
                }
            }
            //Encriptacion de la contraseña
            player.Password = BCryptHelper.HashPassword(player.Password, BCryptHelper.GenerateSalt());

            return playerRestService.CreatePlayerAsync(player).Result;
        }

        public bool DeletePlayer(int playerId)
        {
            Random random = new Random();
            List<Team> teamsJoined = new List<Team>();
            List<Sport> sports = new List<Sport>();
            sports = teamRestService.GetSports().Result.ToList();
            int numTeamsJoined = 0;
            Player captain = new Player();
            foreach (Sport s in sports)
            {
                try
                {
                    teamsJoined = playerRestService.FindTeamsJoinedAsync(playerId, s.SportID).Result.ToList();
                    numTeamsJoined = teamsJoined.Count;
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        if (ex is NotTeamJoinedOnSportException)
                            numTeamsJoined = 0;
                        else
                            throw new Exception(ex.Message);
                    }
                }

                if (numTeamsJoined != 0)
                {
                    foreach(Team t in teamsJoined)
                    {
                        try
                        {
                            captain = teamRestService.GetCaptainAsync(t.TeamID).Result;
                        }
                        catch (AggregateException aex)
                        {
                            foreach (var ex in aex.Flatten().InnerExceptions)
                            {
                                if (ex is TeamNotFoundException) {
                                    throw new TeamNotFoundException(ex.Message);
                                }
                                else
                                    throw new Exception(ex.Message);
                            }
                        }

                        List<Joined> joineds = new List<Joined>();
                        joineds.AddRange(t.Joineds);
                        if (playerId == captain.PlayerId)
                        {
                            try
                            {
                                noticeRestService.DeleteAllNoticeByTypeMessengerAndSport(Constants.TEAM_ADD_PLAYER, playerId, s.SportID);
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
                            try
                            {
                                teamRestService.RemovePlayer(t.TeamID, playerId, true);
                            }
                            catch (AggregateException aex)
                            {
                                foreach (var ex in aex.Flatten().InnerExceptions)
                                {
                                    if (ex is NotJoinedException)
                                    {
                                        throw new NotJoinedException(ex.Message);
                                    }
                                    else
                                        throw new Exception(ex.Message);
                                }
                            }
                            joineds.RemoveAt(joineds.FindIndex(p => p.PlayerID == playerId));
                            if (joineds.Count != 0)
                            {
                                int rPlayer = random.Next(joineds.Count);
                                Joined newCaptain = joineds[rPlayer];
                                try
                                {
                                    teamRestService.RemovePlayer(newCaptain.TeamID, newCaptain.PlayerID, false);
                                    teamRestService.AddPlayer(newCaptain.TeamID, newCaptain.PlayerID, true);
                                }
                                catch (AggregateException aex)
                                {
                                    foreach (var ex in aex.Flatten().InnerExceptions)
                                    {
                                        if (ex is NotJoinedException)
                                        {
                                            throw new NotJoinedException(ex.Message);
                                        }
                                        else
                                        {
                                            if (ex is PlayerNotFoundException)
                                            { throw new PlayerNotFoundException(ex.Message); }
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
                                }
                            }
                            else
                            {
                                try
                                {
                                    teamRestService.DeleteTeamAsync(t.TeamID);
                                }
                                catch (AggregateException aex)
                                {
                                    foreach (var ex in aex.Flatten().InnerExceptions)
                                    {
                                            throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            teamRestService.RemovePlayer(t.TeamID, playerId, false);
                            if (joineds.Count == 1)
                            {
                                try
                                {
                                    teamRestService.DeleteTeamAsync(t.TeamID);
                                }
                                catch (AggregateException aex)
                                {
                                    foreach (var ex in aex.Flatten().InnerExceptions)
                                    {
                                        throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
               
                List<Game> gameList = new List<Game>();
                int totalGames = 0;
                try
                {
                    totalGames = playerRestService.TotalGamesCount(playerId, s.SportID).Result;
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        if (ex is GameNotFoundException)
                            totalGames = 0;
                        else
                            throw new Exception(ex.Message);
                    }
                }
                if (totalGames != 0)
                {
                    gameList.AddRange(playerRestService.GetActualGames(0, totalGames, playerId, s.SportID).Result.ToList());
                    foreach (Game g in gameList)
                    {
                        gameManager.RemovePlayer(g.GameID, playerId);
                    }
                }

            }           
            return playerRestService.DeletePlayerAsync(playerId).Result;

        }

        public bool UpdatePlayer(Player player)
        {
            string[] finalEmail = splitFunction(player.PlayerMail);
            int id_responseMail = 0;
            int id_responseNick = 0;

            // Comprobamos mail duplicado
            try
            {
                id_responseMail = playerRestService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
                if (id_responseMail != player.PlayerId)
                    throw new DuplicatePlayerMailException("Player with mail: " + player.PlayerMail + " already exists.");

            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is Exception)
                            throw new Exception(ex.Message);
                }
            }

            // Comprobamos nick duplicado
            try
            {
                id_responseNick = playerRestService.FindPlayerByNickAsync(player.PlayerNick).Result;
                if (id_responseNick != player.PlayerId)
                    throw new DuplicatePlayerNickException("Player with nick: " + player.PlayerNick + " already exists.");
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is Exception)
                        throw new Exception(ex.Message);
                }
            }

            return playerRestService.UpdatePlayerAsync(player).Result;

        }

        public Player Authentication (string nickOrMail, string password)
        {
            int response = 0;
            string[] finalEmail = splitFunction(nickOrMail);

            try
            {
                if (finalEmail[1] != null)
                {
                    response = playerRestService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
                }
                else response = playerRestService.FindPlayerByNickAsync(finalEmail[0]).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException)
                        throw new PlayerNotFoundException(ex.Message);
                    if (ex is Exception)
                        throw new Exception(ex.Message);                    
                }
            }
            Player player = playerRestService.GetPlayerAsync(response).Result;
            if (BCryptHelper.CheckPassword(password, player.Password))
            {
                if (player.PlayerSesion)
                {
                    throw new PlayerAlreadyConnectedException("User : " + player.PlayerNick + " is already connected");
                }
                else
                {
                    try
                    {
                        playerRestService.Session(player.PlayerId);
                    }
                    catch (AggregateException aex)
                    {
                        foreach (var ex in aex.Flatten().InnerExceptions)
                        {
                            if (ex is Exception)
                                throw new Exception(ex.Message);
                        }
                    }
                }

            }
            else
                throw new PasswordIncorrectException("Nick/mail or password was incorrect ");

            return player;


        }

        public int FindPlayerByMail(string nickOrMail, string post)
        {
            int playerId = 0;
            try
            {
                playerId = playerRestService.FindPlayerByMailAsync(nickOrMail, post).Result;
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
            return playerId;
        }


        public int FindPlayerByNick(string nickOrMail)
        {
            int returnId = 0;
            try
            {
                returnId = playerRestService.FindPlayerByNickAsync(nickOrMail).Result;
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
            return returnId;
        }

        public ICollection<Team> FindTeamsJoined(int playerId, int sportId)
        {
            ICollection<Team> teams = new Collection<Team>();
            try
            {
                teams = playerRestService.FindTeamsJoinedAsync(playerId, sportId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NotTeamJoinedOnSportException)
                    {
                        throw new NotTeamJoinedOnSportException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return teams;
        }

        public Team FindTeamCaptainOnSport(int playerId, int SportId)
        {
            Team teamReturn = new Team();  
            try
            {
                teamReturn = teamRestService.GetTeamAsync(playerRestService.FindCaptainOnSportsAsync(playerId, SportId).Result).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is CaptainNotFoundException)
                    {
                        throw new CaptainNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return teamReturn;
        }

        public List<Player> FindAllPlayersOnOurTeams(int playerId, int sportId)
        {
            List<Player> playersReturn = new List<Player>();
            List<Team> teamsJoined = new List<Team>();
            try
            {
                teamsJoined = playerRestService.FindTeamsJoinedAsync(playerId, sportId).Result.ToList();
            }              
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NotTeamJoinedOnSportException)
                    {
                        throw new NotTeamJoinedOnSportException(ex.Message);
                     }
                    else
                        throw new Exception(ex.Message);
                }
            }
            foreach (Team t in teamsJoined)
            {
                foreach (Joined j in t.Joineds)
                {
                    playersReturn.Add(j.Player);
                }
            }
            return playersReturn;
        }

        public List<Game> GetActualGames(int page, int rows, int playerId, int sportId)
        {
            List<Game> gameListReturn = new List<Game>();
            try
            {
                gameListReturn = playerRestService.GetActualGames(page, rows, playerId, sportId).Result.ToList();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is GameNotFoundException)
                    {
                        throw new GameNotFoundException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return gameListReturn;
        }

        public int TotalGamesCount(int playerId, int sportId)
        {
            int countGames = 0;
            try
            {
                countGames = playerRestService.TotalGamesCount(playerId, sportId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
            return countGames;
        }

        public void Session(int playerId)
        {
            try
            {
                playerRestService.Session(playerId);
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }

        }

        public void OutSession(int playerId)
        {
            try
            {
                playerRestService.OutSession(playerId);
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    throw new Exception(ex.Message);
                }
            }
        }        


        // Funcion split, necesario para el parametro mail.
        string[] splitFunction (string playerMail)
        {

            int n = 0;
            string[] finalEmail = new string [2];

            string[] emailParts = playerMail.Split('.');

            while (n <= emailParts.Length - 2)
            {
                if (n == 0)
                    finalEmail[0] = emailParts[n];
                else
                {
                    finalEmail[0] += "." + emailParts[n];
                }
                n++;
            }
            if(emailParts.Length >1)
                finalEmail[1] = emailParts[emailParts.Length - 1];
            else
                finalEmail[0] = emailParts[emailParts.Length - 1];

            return finalEmail;
        }

    }
}
