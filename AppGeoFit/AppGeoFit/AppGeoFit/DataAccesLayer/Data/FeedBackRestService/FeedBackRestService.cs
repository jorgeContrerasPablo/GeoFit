using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppGeoFit.DataAccesLayer.Models;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Collections.ObjectModel;
using AppGeoFit.DataAccesLayer.Data.FeedBackRestService.Exceptions;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.FeedBackRestService.FeedBackRestService))]
namespace AppGeoFit.DataAccesLayer.Data.FeedBackRestService
{
    class FeedBackRestService : IFeedBackRestService
    {
        public string url { get; set; }
        readonly HttpClient client;

        public FeedBackRestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<int> CreateFeedBackAsync(FeedBack feedBack)
        {
            var uri = new Uri(string.Format(url + "FeedBack/CreateFeedBack"));
            int responseSucced = 0;
            HttpResponseMessage response = new HttpResponseMessage();

            var json = JsonConvert.SerializeObject(feedBack);
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

        public void UpdateLvlAndOnTimePlayer(int playerId)
        {
            var uri = new Uri(string.Format(url + "FeedBack/UpdateLvlAndOnTimePlayer/{0}", playerId));

            var json = JsonConvert.SerializeObject(playerId);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public void UpdateLvlTeam(int teamId)
        {
            var uri = new Uri(string.Format(url + "FeedBack/UpdateLvlTeam/{0}", teamId));

            var json = JsonConvert.SerializeObject(teamId);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

        }

        public void UpdateLvlPlace(int placeId)
        {
            var uri = new Uri(string.Format(url + "FeedBack/UpdateLvlPlace/{0}", placeId));

            var json = JsonConvert.SerializeObject(placeId);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<FeedBack> GetFeedBackAsync(int feedBackId)
        {
            FeedBack responseAsFeedBack = new FeedBack();
            var uri = new Uri(string.Format(url + "FeedBack/GetFeedBack/{0}", feedBackId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseAsFeedBack = JsonConvert.DeserializeObject<FeedBack>(responseAsString);
            }

            return responseAsFeedBack;
        }

        public async Task<bool> UpdateFeedBackAsync(FeedBack feedBack)
        {
            var uri = new Uri(string.Format(url + "FeedBack/UpdateFeedBack"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(feedBack);
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

        public async Task<bool> DeleteFeedBackAsync(int feedBackId)
        {
            var uri = new Uri(string.Format(url + "FeedBack/DeleteFeedBack/{0}", feedBackId));
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

        public async Task<int> TotalPlayerCommentsCount(int playerId)
        {
            int numComments = 0;
            var uri = new Uri(string.Format(url + "FeedBack/TotalPlayerCommentsCount/{0}", playerId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                numComments = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return numComments;
        }

        public async Task<int> TotalPlaceCommentsCount(int placeId)
        {
            int numComments = 0;
            var uri = new Uri(string.Format(url + "FeedBack/TotalPlaceCommentsCount/{0}", placeId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                numComments = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return numComments;
        }

        public async Task<int> TotalGameCommentsCount(int gameId)
        {
            int numComments = 0;
            var uri = new Uri(string.Format(url + "FeedBack/TotalGameCommentsCount/{0}", gameId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                numComments = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return numComments;
        }

        public async Task<ICollection<FeedBack>> GetPlaceCommentsPagination(int pages, int rows, int placeId)
        {
            ICollection<FeedBack> responseListComments = new Collection<FeedBack>();
        
            var uri = new Uri(string.Format(url + "FeedBack/GetPlaceCommentsPagination/{0}/{1}/{2}", pages, rows, placeId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseListComments = JsonConvert.DeserializeObject<ICollection<FeedBack>>(responseAsString);
            }

            return responseListComments;
        }

        public async Task<ICollection<FeedBack>> GetPlayerCommentsPagination(int pages, int rows, int playerId)
        {
            ICollection<FeedBack> responseListComments = new Collection<FeedBack>();

            var uri = new Uri(string.Format(url + "FeedBack/GetPlayerCommentsPagination/{0}/{1}/{2}", pages, rows, playerId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseListComments = JsonConvert.DeserializeObject<ICollection<FeedBack>>(responseAsString);
            }

            return responseListComments;
        }

        public async Task<ICollection<FeedBack>> GetGameCommentsPagination(int pages, int rows, int gameId)
        {
            ICollection<FeedBack> responseListComments = new Collection<FeedBack>();

            var uri = new Uri(string.Format(url + "FeedBack/GetGameCommentsPagination/{0}/{1}/{2}", pages, rows, gameId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FeedBackNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseListComments = JsonConvert.DeserializeObject<ICollection<FeedBack>>(responseAsString);
            }

            return responseListComments;
        }
    }
}
