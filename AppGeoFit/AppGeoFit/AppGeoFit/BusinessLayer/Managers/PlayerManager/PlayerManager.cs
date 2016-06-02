using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using DevOne.Security.Cryptography.BCrypt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.PlayerManager.PlayerManager))]
namespace AppGeoFit.BusinessLayer.Managers.PlayerManager
{    
    public class PlayerManager : IPlayerManager
    {
        IPlayerRestService restService;

        public PlayerManager(){}

        public IPlayerManager InitiateServices(bool test)
        {
            restService = DependencyService.Get<IPlayerRestService>();
            restService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }

        public Task<Player> GetPlayer(int playerId)
        {
            return restService.GetPlayerAsync(playerId);
        }

        public Task<ICollection<Player>> GetAll()
        {
            return restService.GetAllAsync();
        }

        public Task<int> CreatePlayer(Player player)
        {
            string[] finalEmail = splitFunction( player.PlayerMail );
            int reciveIdEmail = 0;
            int reciveIdNick = 0;

            // Comprobamos mail duplicado
            try
            {
                reciveIdEmail = restService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
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
                reciveIdNick = restService.FindPlayerByNickAsync(player.PlayerNick).Result;            
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

            return restService.CreatePlayerAsync(player);
        }

        public Task<Boolean> DeletePlayer(int playerId)
        {        
            return restService.DeletePlayerAsync(playerId);
        }

        public Task<Boolean> UpdatePlayer(Player player)
        {
            string[] finalEmail = splitFunction(player.PlayerMail);
            int id_responseMail = 0;
            int id_responseNick = 0;

            // Comprobamos mail duplicado
            try
            {
                id_responseMail = restService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
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
                id_responseNick = restService.FindPlayerByNickAsync(player.PlayerNick).Result;
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

            return restService.UpdatePlayerAsync(player);

        }

        public Player Authentication (string nickOrMail, string password)
        {
            int response = 0;
            string[] finalEmail = splitFunction(nickOrMail);

            try
            {
                if (finalEmail[1] != null)
                {
                    response = restService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
                }
                else response = restService.FindPlayerByNickAsync(finalEmail[0]).Result;
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
            Player player = restService.GetPlayerAsync(response).Result;
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
                        restService.Session(player.PlayerId);
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
            return restService.FindPlayerByMailAsync(nickOrMail, post);
        }

        public Task<int> FindPlayerByNick(string nickOrMail)
        {
            return restService.FindPlayerByNickAsync(nickOrMail);
        }

        public Task<ICollection<Team>> FindTeamsJoined(int playerId, int sportId)
        {
            return restService.FindTeamsJoinedAsync(playerId, sportId);
        }

        public void Session(int playerId)
        {
            restService.Session(playerId);
        }

        public void OutSession(int playerId)
        {
            restService.OutSession(playerId);
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
