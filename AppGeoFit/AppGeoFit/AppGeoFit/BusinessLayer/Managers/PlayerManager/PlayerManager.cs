using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.TeamRestService;
using AppGeoFit.DataAccesLayer.Models;
using DevOne.Security.Cryptography.BCrypt;
using System;
using System.Collections.Generic;
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

        public PlayerManager(){}

        public IPlayerManager InitiateServices(bool test)
        {
            teamRestService = DependencyService.Get<ITeamRestService>();
            teamRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            playerRestService = DependencyService.Get<IPlayerRestService>();
            playerRestService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }

        public Task<Player> GetPlayer(int playerId)
        {
            return playerRestService.GetPlayerAsync(playerId);
        }

        public Task<ICollection<Player>> GetAll()
        {
            return playerRestService.GetAllAsync();
        }

        public Task<int> CreatePlayer(Player player)
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

            return playerRestService.CreatePlayerAsync(player);
        }

        public Task<Boolean> DeletePlayer(int playerId)
        {        
            return playerRestService.DeletePlayerAsync(playerId);
        }

        public Task<Boolean> UpdatePlayer(Player player)
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

            return playerRestService.UpdatePlayerAsync(player);

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

        public Task<int> FindPlayerByMail(string nickOrMail, string post)
        {
            return playerRestService.FindPlayerByMailAsync(nickOrMail, post);
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

        public Task<ICollection<Team>> FindTeamsJoined(int playerId, int sportId)
        {
            return playerRestService.FindTeamsJoinedAsync(playerId, sportId);
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
            List<Team> teamsJoined = (List<Team>)playerRestService.FindTeamsJoinedAsync(playerId, sportId).Result;
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
                    //finalEmail[0].Insert(0,emailParts[n]);
                    finalEmail[0] = emailParts[n];
                else
                {
                    //finalEmail[0].Insert(emailParts[n - 1].Length, "." + emailParts[n]);
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
