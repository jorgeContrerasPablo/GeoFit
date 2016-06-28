using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppGeoFit.BusinessLayer.Managers.GameManager
{
    public interface IGameManager
    {
        IGameManager InitiateServices(bool test);
        int CreateGame(Game game);
    }
}
