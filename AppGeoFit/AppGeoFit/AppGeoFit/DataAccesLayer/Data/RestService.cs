using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.RestService))]
namespace AppGeoFit.DataAccesLayer.Data
{
    class RestService :IRestService
    {
        
        readonly HttpClient client;
        

        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;        
        }

        public async Task<Player> GetPlayerAsync(int PlayerId)

        {
           Player responseAsPlayer = new Player();
           var uri = new Uri(string.Format(Constants.RestUrl + "Players/GetPlayer/{0}", PlayerId));

            try
             {
                 HttpResponseMessage response =   client.GetAsync(uri).Result;
                 if (response.IsSuccessStatusCode)
                 {
                     string responseAsString = await response.Content.ReadAsStringAsync();

                     responseAsPlayer = JsonConvert.DeserializeObject<Player>(responseAsString);
                 }
                 else
                 {
                     Debug.WriteLine("Estado de la respuesta http : "+response.StatusCode);
                 }
             }
             catch (Exception ex)
             {
                // Debug.WriteLine(@"				ERROR {0}", ex.Message);
                throw new Exception(ex.Message);
             }
             return responseAsPlayer;
        }

        public async Task<int> CreatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(Constants.RestUrl + "Players/CreatePlayer"));
            int responseSucced = 0;

            try
            {
                var json = JsonConvert.SerializeObject(player);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(uri, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();

                    responseSucced = JsonConvert.DeserializeObject<int>(responseAsString);
                }
                else
                {
                    Debug.WriteLine("Estado de la respuesta http : " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine(@"				ERROR {0}", ex.Message);
                throw new Exception(ex.Message);
            }
            return responseSucced; 

        }

        public async Task<Boolean> DeletePlayerAsync(int PlayerId)
        {
            var uri = new Uri(string.Format(Constants.RestUrl + "Players/DeletePlayer/{0}", PlayerId));
            Boolean responseSucced = false;
            try
            {
                HttpResponseMessage response = client.DeleteAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();

                    responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
                }
                else
                {
                    Debug.WriteLine("Estado de la respuesta http : " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine(@"				ERROR {0}", ex.Message);
                throw new Exception(ex.Message);
            }
            return responseSucced;

        }

        public async Task<Boolean> UpdatePlayerAsync(Player player)
        {
            var uri = new Uri(string.Format(Constants.RestUrl + "Players/UpdatePlayer"));
            Boolean responseSucced = false;
            try
            {
                var json = JsonConvert.SerializeObject(player);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PutAsync(uri, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();

                    responseSucced = JsonConvert.DeserializeObject<Boolean>(responseAsString);
                }
                else
                {
                    Debug.WriteLine("Estado de la respuesta http : " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine(@"				ERROR {0}", ex.Message);
                throw new Exception(ex.Message);
            }
            return responseSucced;
        }
    }
}
