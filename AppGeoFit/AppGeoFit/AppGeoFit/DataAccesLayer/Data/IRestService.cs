using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data
{
    public interface IRestService
    {
        Task <Player> GetPlayerAsync(int PlayerId);
       // Player GetPlayerSync(int PlayerId);
    }
}
