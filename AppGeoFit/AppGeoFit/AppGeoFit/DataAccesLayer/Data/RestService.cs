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
        private Player responseAsPlayer;

        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;        
        }

        public async Task<Player> GetPlayerAsync(int PlayerId)

        {
            var uri = new Uri(string.Format(Constants.RestUrl + "Players/player/{0}", PlayerId));

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
    }
}
