﻿using System;
using System.Text;
using System.Threading.Tasks;
using AppGeoFit.DataAccesLayer.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using AppGeoFit.BusinessLayer.Exceptions;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.GameRestService.GameRestService))]
namespace AppGeoFit.DataAccesLayer.Data.GameRestService
{
    class GameRestService : IGameRestService
    {
        public string url { get; set; }
        readonly HttpClient client;

        public GameRestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<int> CreateGameAsync(Game game)
        {
            var uri = new Uri(string.Format(url + "Game/CreateGame"));
            int responseSucced = 0;
            HttpResponseMessage response = new HttpResponseMessage();

            var json = JsonConvert.SerializeObject(game);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            response = client.PostAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            var uri = new Uri(string.Format(url + "Game/DeleteGame/{0}", gameId));
            Boolean responseSucced = false;

            HttpResponseMessage response = client.DeleteAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<Game> GetGameAsync(int gameId)
        {
            Game responseAsGame= new Game();
            var uri = new Uri(string.Format(url + "Game/GetGame/{0}", gameId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseAsGame = JsonConvert.DeserializeObject<Game>(responseAsString);
            }

            return responseAsGame;
        }

        public async Task<bool> UpdateGameAsync(Game game)
        {
            var uri = new Uri(string.Format(url + "Game/UpdateGame"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(game);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<int> FindOnTime(int playerId, string startDate, string endDate)
        {
            int responseId = 0;
            Uri uri = new Uri(string.Format(url + "Game/FindOnTime/{0}/{1}/{2}", playerId, startDate, endDate));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseId = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return responseId;
        }
        //TODO PLACE
        public async Task<bool> FindOnTimeAndPlace(int placeId, DateTime startDate, DateTime endDate)
        {
            bool responseSucced = false;
            Uri uri = new Uri(string.Format(url + "Game/FindOnTimeAndPlace/{0}/{1}/{2}", placeId, startDate, endDate));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }
        //TODO Coordinates
        public async Task<bool> FindOnTimeAndPlace(double latitude, double longitude, DateTime startDate, DateTime endDate)
        {
            bool responseSucced = false;
            Uri uri = new Uri(string.Format(url + "Game/FindOnTimeAndPlace/{0}/{1}/{2}/{4}", latitude, longitude, startDate, endDate));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<ICollection<Game>> GetAllPagination(int pages, int rows, int sportId)
        {
            ICollection<Game> responseListGames = new Collection<Game>();
            var uri = new Uri(string.Format(url + "Game/GetAllPagination/{0}/{1}/{2}", pages, rows, sportId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseListGames = JsonConvert.DeserializeObject<ICollection<Game>>(responseAsString);
            }

            return responseListGames;
        }

        public async Task<int> TotalGamesCount(int sportId)
        {
            int numGames = 0;
            var uri = new Uri(string.Format(url + "Game/TotalGamesCount/{0}", sportId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                numGames = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return numGames;

        }

        public async Task<bool> IsPlayerOnGame(int gameId, int playerId)
        {
            bool isOnGame = false;
            var uri = new Uri(string.Format(url + "Game/IsPlayerOnGame/{0}/{1}", gameId, playerId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                isOnGame = JsonConvert.DeserializeObject<bool>(responseAsString);
            }
            if (isOnGame)
            {
                throw new PlayerOnGameException(response.ReasonPhrase);
            }
            return isOnGame;
        }

        public async Task<bool> AddPlayer(int gameId, int playerId)
        {
            bool isOnGame = false;
            var uri = new Uri(string.Format(url + "Game/AddPlayer/{0}/{1}", gameId, playerId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                isOnGame = JsonConvert.DeserializeObject<bool>(responseAsString);
            }
            return isOnGame;
        }

        public async Task<bool> AddPlayers(Game game)
        {
            var uri = new Uri(string.Format(url + "Game/AddPlayers"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(game);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<bool> RemovePlayers(Game game)
        {
            var uri = new Uri(string.Format(url + "Game/RemovePlayers"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(game);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<int> FindTeamOnTime(int teamId, string startDate, string endDate)
        {
            int responseId = 0;
            Uri uri = new Uri(string.Format(url + "Game/FindTeamOnTime/{0}/{1}/{2}", teamId, startDate, endDate));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new GameNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseId = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return responseId;
        }

        public async Task<ICollection<Player>> GetParticipatePlayers(int gameId)
        {
            ICollection<Player> responsePlayerList = new Collection<Player>();
            var uri = new Uri(string.Format(url + "Game/GetParticipatePlayers/{0}", gameId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayerNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responsePlayerList = JsonConvert.DeserializeObject<ICollection<Player>>(responseAsString);
            }

            return responsePlayerList;
        }
    }
}