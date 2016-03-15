using AppGeoFit.DataAccesLayer.Data;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
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
            if (test)
                restService.url = Constants.RestUrlTest;
            else
                restService.url = Constants.RestUrl;

        }

        public Task<Player> GetPlayer(int playerId)
        {
            return restService.GetPlayerAsync(playerId);
        }

        public Task<int> CreatePlayer(Player player)
        {
            //TODO comprobación nick y usuario único con findPlayerByNickOrMail
            //, string nick, string mail
            return restService.CreatePlayerAsync(player);
        }

        public Task<Boolean> DeletePlayer(int playerId)
        {
            return restService.DeletePlayerAsync(playerId);
        }

        public Task<Boolean> UpdatePlayer(Player player)
        {
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
    }
}
