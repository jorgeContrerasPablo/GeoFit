using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppGeoFit.DataAccesLayer.Models;
using Xamarin.Forms;
using System.Net;
using Newtonsoft.Json;
using AppGeoFit.DataAccesLayer.Data.TeamRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using System.Collections;
using System.Collections.ObjectModel;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.TeamRestService.TeamRestService))]

namespace AppGeoFit.DataAccesLayer.Data.TeamRestService
{
    class TeamRestService : ITeamRestService
    {
        public string url { get; set; }
        readonly HttpClient client;

        public TeamRestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<Team> GetTeamAsync(int teamId)
        {
            Team responseAsTeam = new Team();
            var uri = new Uri(string.Format(url + "Team/GetTeam/{0}", teamId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new TeamNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseAsTeam = JsonConvert.DeserializeObject<Team>(responseAsString);
            }

            return responseAsTeam;
        }

        public async Task<int> CreateTeamAsync(Team team)
        {
            var uri = new Uri(string.Format(url + "Team/CreateTeam"));
            int responseSucced = 0;
            HttpResponseMessage response = new HttpResponseMessage();


            var json = JsonConvert.SerializeObject(team);
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

        public async Task<Boolean> DeleteTeamAsync(int teamId)
        {
            var uri = new Uri(string.Format(url + "Team/DeleteTeam/{0}", teamId));
            Boolean responseSucced = false;

            HttpResponseMessage response = client.DeleteAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<Boolean> UpdateTeamAsync(Team team)
        {
            var uri = new Uri(string.Format(url + "Team/UpdateTeam"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(team);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<Boolean> AddPlayer(int teamId, int playerId, bool captain)
        {
            var uri = new Uri(string.Format(url + "Team/AddPlayer"));
            Boolean responseSucced = false;

            Joined joined = new Joined();
            joined.TeamID = teamId;
            joined.PlayerID = playerId;
            joined.Captain = captain;

            var json = JsonConvert.SerializeObject(joined);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (response.ReasonPhrase.Contains("Player"))
                    throw new PlayerNotFoundException(response.ReasonPhrase);
                else
                    throw new TeamNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
            }
            return responseSucced;
        }

        public async Task<Boolean> RemovePlayer(int teamId, int playerId, bool captain)
        {
            var uri = new Uri(string.Format(url + "Team/RemovePlayer"));
            Boolean responseSucced = false;

            Joined joined = new Joined();
            joined.TeamID = teamId;
            joined.PlayerID = playerId;
            joined.Captain = captain;

            var json = JsonConvert.SerializeObject(joined);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {               
                throw new NotJoinedException(response.ReasonPhrase);            
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
            }
            return responseSucced;
        }

        public async Task<ICollection<Sport>> GetSports()
        {
            var uri = new Uri(string.Format(url + "Team/GetSports"));
            ICollection<Sport> responseSports = new Collection<Sport>();

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
               throw new SportsNotFoundException(response.ReasonPhrase);            
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSports = JsonConvert.DeserializeObject<ICollection<Sport>>(responseAsString);
            }
            return responseSports;

        }

        public async Task<int> FindTeamByNameOnSports(string teamName, int sportId)
        {
            int responseSucced = 0;
            Uri uri = new Uri(string.Format(url + "Team/FindTeamByNameOnSports/{0}/{1}", teamName, sportId));
            
            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new TeamNotFoundException(response.ReasonPhrase);
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

        public async Task<Player> GetCaptainAsync(int teamId)
        {
            Player responseAsPlayer = new Player();
            var uri = new Uri(string.Format(url + "Team/GetCaptain/{0}", teamId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new TeamNotFoundException(response.ReasonPhrase);
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

        public async Task<ICollection<Player>> GetAllPlayersPendingToAdd(int messengerId, int sportId, string type)
        {
            ICollection<Player> responseListPlayers = new Collection<Player>();
            var uri = new Uri(string.Format(url + "Team/GetAllPlayersPendingToAdd/{0}/{1}/{2}", messengerId, sportId, type));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotPendingPlayersToAddException(response.ReasonPhrase);
            }
            if(response.StatusCode == HttpStatusCode.NotFound)
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
    }
}
