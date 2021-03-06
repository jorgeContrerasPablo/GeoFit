﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using System.Net.Http;
using AppGeoFit.DataAccesLayer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using System.Net.Http.Headers;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.PlayerRestService.PlayerRestService))]
namespace AppGeoFit.DataAccesLayer.Data.PlayerRestService
{
    class PlayerRestService : IPlayerRestService
    {
        public string url  { get; set; }
        readonly HttpClient client;
        
        public PlayerRestService()
        {           
            var authData = string.Format("{0}:{1}", Constants.Username, Constants.Password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        }

        public async Task<Player> GetPlayerAsync(int playerId)

        {
            Player responseAsPlayer = new Player();
            var uri = new Uri(string.Format(url + "Players2/GetPlayer/{0}/", playerId));
            HttpResponseMessage response;

         
            response =  client.GetAsync(uri).Result;
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

                responseAsPlayer = JsonConvert.DeserializeObject<Player>(responseAsString);
            }
            
            return responseAsPlayer;
        }

        public async Task<ICollection<Player>> GetAllAsync()
        {
            ICollection<Player> responseListPlayers = new Collection<Player>();
            var uri = new Uri(string.Format(url + "Players2/GetAll/"));
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

                responseListPlayers = JsonConvert.DeserializeObject<ICollection<Player>>(responseAsString);
            }

            return responseListPlayers;
        }

        public async Task<int> CreatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(url + "Players2/CreatePlayer/"));
            int responseSucced = 0;
            HttpResponseMessage response = new HttpResponseMessage();


            var json = JsonConvert.SerializeObject(player);
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

        public async Task<bool> DeletePlayerAsync(int PlayerId)
        {
            var uri = new Uri(string.Format(url + "Players2/DeletePlayer/{0}/", PlayerId));
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

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(url + "Players2/UpdatePlayer/"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(player);
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

        public async Task<int> FindPlayerByMailAsync(string nickOrMail, string post)
        {
            int responseSucced = 0;

            Uri uri = new Uri(string.Format(url + "Players2/FindPlayerByMail/{0}/{1}/", nickOrMail, post));
            
            HttpResponseMessage response = client.GetAsync(uri).Result;

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
                responseSucced = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return responseSucced;

        }

        public async Task<int> FindPlayerByNickAsync(string nickOrMail)
        {
            int responseSucced = 0;
            Uri uri = new Uri(string.Format(url + "Players2/FindPlayerByNick/{0}/", nickOrMail));

            HttpResponseMessage response = client.GetAsync(uri).Result;

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
                responseSucced = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return responseSucced;

        }

        public async void Session(int playerId)
        {
            var uri = new Uri(string.Format(url + "Players2/Session/{0}/", playerId));

            var json = JsonConvert.SerializeObject("");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }

        }

        public async void OutSession(int playerId)
        {
            var uri = new Uri(string.Format(url + "Players2/OutSession/{0}/", playerId));

            var json = JsonConvert.SerializeObject("");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<int> FindCaptainOnSportsAsync(int playerId, int sportId)
        {
            int responseSucced = 0;
            Uri uri = new Uri(string.Format(url + "Players2/FindCaptainOnSports/{0}/{1}/", playerId, sportId));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new CaptainNotFoundException(response.ReasonPhrase);
            }
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

        public async Task<ICollection<Team>> FindTeamsJoinedAsync(int playerId, int sportId)
        {
            ICollection<Team> responseTeams = new Collection<Team>();
            Uri uri = new Uri(string.Format(url + "Players2/FindTeamsJoined/{0}/{1}/", playerId, sportId));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotTeamJoinedOnSportException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseTeams = JsonConvert.DeserializeObject<ICollection<Team>>(responseAsString);
            }

            return responseTeams;

        }

        public async Task<Player> FindPlayerOnTeamAsync(string playerNick, int teamId)
        {
            Player responsePlayer = new Player();
            Uri uri = new Uri(string.Format(url + "Players2/FindPlayerOnTeam/{0}/{1}/", playerNick, teamId));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundPlayerOnTeamException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responsePlayer = JsonConvert.DeserializeObject<Player>(responseAsString);
            }

            return responsePlayer;
        }

        public async Task<ICollection<Game>> GetActualGames(int page, int rows, int playerId, int sportId)
        {
            ICollection<Game> responseGames = new Collection<Game>();
            Uri uri = new Uri(string.Format(url + "Players2/GetActualGames/{0}/{1}/{2}/{3}/", page, rows, playerId, sportId));

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
                responseGames = JsonConvert.DeserializeObject<ICollection<Game>>(responseAsString);
            }

            return responseGames;
        }

        public async Task<int> TotalGamesCount(int playerId, int sportId)
        {
            int totalGames = 0;
            Uri uri = new Uri(string.Format(url + "Players2/TotalGamesCount/{0}/{1}/", playerId, sportId));

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
                totalGames = JsonConvert.DeserializeObject<int>(responseAsString);
            }
            return totalGames;
        }
    }
}
