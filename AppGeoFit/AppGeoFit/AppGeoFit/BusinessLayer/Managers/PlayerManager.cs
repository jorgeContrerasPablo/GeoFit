using AppGeoFit.DataAccesLayer.Data;
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
        public PlayerManager()
        {
            restService = DependencyService.Get<RestService>();

        }

        public Task<Player> GetPlayer(int PlayerId)
        {
            return restService.GetPlayerAsync(PlayerId);
        }

        public Task<int> CreatePlayer(Player player)
        {
            return restService.CreatePlayerAsync(player);
        }

        public Task<Boolean> DeletePlayer(int PlayerId)
        {
            return restService.DeletePlayerAsync(PlayerId);
        }

        public Task<Boolean> UpdatePlayer(Player player)
        {
            return restService.UpdatePlayerAsync(player);
        }
    }
}
