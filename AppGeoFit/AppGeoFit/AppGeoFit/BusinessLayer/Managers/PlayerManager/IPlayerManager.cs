using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppGeoFit.BusinessLayer.Managers.PlayerManager
{
    public interface IPlayerManager
    {
        IPlayerManager InitiateServices(bool test);
        Task<Player> GetPlayer(int playerId);
        Task<ICollection<Player>> GetAll();
        Task<int> CreatePlayer(Player player);
        Task<Boolean> DeletePlayer(int playerId);
        Task<Boolean> UpdatePlayer(Player player);
        Player Authentication(string nickOrMail, string password);
        Task<int> FindPlayerByMail(string nickOrMail, string post);
        Task<int> FindPlayerByNick(string nickOrMail);
        Task<ICollection<Team>> FindTeamsJoined(int playerId, int sportId);
        Task<Team> FindTeamCaptainOnSport(int playerId, int SportId);
        void Session(int playerId);
        void OutSession(int playerId);
    }
}
