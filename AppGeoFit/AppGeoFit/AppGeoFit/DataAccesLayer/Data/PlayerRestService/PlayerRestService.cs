using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using AppGeoFit.DataAccesLayer.Data.PlayerRestService.Exceptions;
using System.Net.Http;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.RestService))]
namespace AppGeoFit.DataAccesLayer.Data
{
    class RestService : IRestService
    {
        public string url  { get; set; }
        readonly HttpClient client;

        public RestService()
            {
                client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
            }

        public async Task<Player> GetPlayerAsync(int PlayerId)

        {
            Player responseAsPlayer = new Player();
            var uri = new Uri(string.Format(url + "Players/GetPlayer/{0}", PlayerId));
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

        public async Task<int> CreatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(url + "Players/CreatePlayer"));
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

        public async Task<Boolean> DeletePlayerAsync(int PlayerId)
        {
            var uri = new Uri(string.Format(url + "Players/DeletePlayer/{0}", PlayerId));
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

        public async Task<Boolean> UpdatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(url + "Players/UpdatePlayer"));
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

                responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<int> FindPlayerByMailAsync(string nickOrMail, string post)
        {
            int responseSucced = 0;

            Uri uri = new Uri(string.Format(url + "Players/FindPlayerByMail/{0}/{1}", nickOrMail, post));
            
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
            Uri uri = new Uri(string.Format(url + "Players/FindPlayerByNick/{0}", nickOrMail));

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

        public void Session(int playerId)
        {
            var uri = new Uri(string.Format(url + "Players/Session/{0}", playerId));
           
            try
            {
                var json = JsonConvert.SerializeObject("");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PutAsync(uri, content).Result;
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void OutSession(int playerId)
        {
            var uri = new Uri(string.Format(url + "Players/OutSession/{0}", playerId));

            try
            {
                var json = JsonConvert.SerializeObject("");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PutAsync(uri, content).Result;
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
