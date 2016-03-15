using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data
{
    public interface IRestService
    {
        string url { get; set; }
        Task <Player> GetPlayerAsync(int PlayerId);
        Task <int> CreatePlayerAsync(Player player);
        Task <Boolean> DeletePlayerAsync(int PlayerId);
        Task <Boolean> UpdatePlayerAsync(Player player);
        Task <int> FindPlayerByMailAsync(string nickOrMail, string post);
        Task<int> FindPlayerByNickAsync(string nickOrMail);
        void Session(int playerId);
        void OutSession(int playerId);
    }
}
