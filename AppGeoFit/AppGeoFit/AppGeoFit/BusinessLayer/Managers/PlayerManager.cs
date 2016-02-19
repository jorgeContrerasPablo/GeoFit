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
        IRestService restService;
        public PlayerManager()
        {
            restService = DependencyService.Get<RestService>();

        }

        public Task<Player> GetPlayer(int PlayerId)
        {
            return restService.GetPlayerAsync(PlayerId);
        }

       /* public Player GetPlayerSync(int PlayerId)
        {
            return restService.GetPlayerSync(PlayerId);
        }*/

    }
}
