﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data
{
    public interface IRestService
    {
        Task <Player> GetPlayerAsync(int PlayerId);
        Task <int> CreatePlayerAsync(Player player);
        Task <Boolean> DeletePlayerAsync(int PlayerId);
        Task <Boolean> UpdatePlayerAsync(Player player);
    }
}
