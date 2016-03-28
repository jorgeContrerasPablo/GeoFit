using AppGeoFit.BusinessLayer.Exceptions;
using AppGeoFit.DataAccesLayer.Data;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppGeoFit.BusinessLayer.Managers
{
    public class PlayerManager
    {
        readonly IRestService restService;
        public PlayerManager(bool test)
        {
            restService = DependencyService.Get<RestService>();
            restService.url = test ? Constants.RestUrlTest : Constants.RestUrl;

        }

        public Task<Player> GetPlayer(int playerId)
        {
            return restService.GetPlayerAsync(playerId);
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
            //TODO Encriptación contraseña

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
            bool okMail = false;
            bool okNick = false;

            // Comprobamos mail duplicado
            try
            {
                id_responseMail = restService.FindPlayerByMailAsync(finalEmail[0], finalEmail[1]).Result;
                okMail = false;

            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException) {
                        okMail = true;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }

            // Comprobamos nick duplicado
            try
            {
                id_responseNick = restService.FindPlayerByNickAsync(player.PlayerNick).Result;
                okNick = false;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is PlayerNotFoundException) {
                        okNick = true;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }

            if (!okNick)
            {
                if (id_responseNick != player.PlayerId)
                    throw new DuplicatePlayerNickException("Player with nick: " + player.PlayerNick + " already exists.");
            }
            if (!okMail)
            {
                if (id_responseMail != player.PlayerId)
                    throw new DuplicatePlayerMailException("Player with mail: " + player.PlayerMail + " already exists.");
            }

            return restService.UpdatePlayerAsync(player);

        }

        public Task<int> FindPlayerByMail(string nickOrMail, string post)
        {
            return restService.FindPlayerByMailAsync(nickOrMail, post);
        }

        public Task<int> FindPlayerByNick(string nickOrMail)
        {
            return restService.FindPlayerByNickAsync(nickOrMail);
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
            finalEmail[1] = emailParts[emailParts.Length - 1];

            return finalEmail;
        }
        
    }
}
