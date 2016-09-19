using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppGeoFit.DataAccesLayer.Models;
using Xamarin.Forms;
using System.Net;
using Newtonsoft.Json;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

[assembly: Dependency(typeof(AppGeoFit.DataAccesLayer.Data.NoticeRestService.NoticeRestService))]
namespace AppGeoFit.DataAccesLayer.Data.NoticeRestService
{
    public class NoticeRestService : INoticeRestService
    {
        public string url { get; set; }
        readonly HttpClient client;

        public NoticeRestService()
        {
            var authData = string.Format("{0}:{1}", Constants.Username, Constants.Password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            client = new HttpClient();            //2147483647
            client.MaxResponseContentBufferSize = 25600000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        }

        public async Task<Notice> GetNoticeAsync(int noticeId)
        {
            Notice responseAsNotice = new Notice();
            var uri = new Uri(string.Format(url + "Notice/GetNotice/{0}/", noticeId));
            HttpResponseMessage response;


            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NoticeNotFoundException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseAsNotice = JsonConvert.DeserializeObject<Notice>(responseAsString);
            }

            return responseAsNotice;
        }

        public async Task<int> CreateNoticeAsync(Notice notice)
        {
            var uri = new Uri(string.Format(url + "Notice/CreateNotice/"));
            int responseSucced = 0;
            HttpResponseMessage response = new HttpResponseMessage();


            var json = JsonConvert.SerializeObject(notice);
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

        public async Task<bool> DeleteNoticeAsync(int noticeId)
        {
            var uri = new Uri(string.Format(url + "Notice/DeleteNotice/{0}/", noticeId));
            Boolean responseSucced = false;

            HttpResponseMessage response = client.DeleteAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NoticeNotFoundException(response.ReasonPhrase);
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

        public async Task<bool> DeleteAllNoticeByTypeMessengerAndSport(string type, int messengerId, int sportId)
        {
            var uri = new Uri(string.Format(url + "Notice/DeleteAllNoticeByTypeMessengerAndSport/{0}/{1}/{2}/", type, messengerId, sportId));
            bool responseSucced = false;

            HttpResponseMessage response = client.DeleteAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NoticeNotFoundException(response.ReasonPhrase);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                responseSucced = JsonConvert.DeserializeObject<bool>(responseAsString);
            }

            return responseSucced;
        }

        public async Task<bool> UpdateNoticeAsync(Notice notice)
        {
            var uri = new Uri(string.Format(url + "Notice/UpdateNotice/"));
            Boolean responseSucced = false;

            var json = JsonConvert.SerializeObject(notice);
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

        public async Task<int> FindNoticeAsync(int receiverId, int messengerId, int sportId, string type)
        {
            int responseSucced = 0;
            Uri uri = new Uri(string.Format(url + "Notice/FindNotice/{0}/{1}/{2}/{3}/", receiverId, messengerId, sportId, type));

            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NoticeNotFoundException(response.ReasonPhrase);
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

        public async Task<bool> NoticeIsPending(int receiverId, int messengerId, int sportId, string type)
        {
            bool responseSucced = false;
            Uri uri = new Uri(string.Format(url + "Notice/NoticeIsPending/{0}/{1}/{2}/{3}/", receiverId, messengerId, sportId, type));

            HttpResponseMessage response = client.GetAsync(uri).Result;

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

        public async Task<ICollection<Notice>> GetAllPendingNotice(int playerId)
        {
            ICollection<Notice> responseListNotice = new Collection<Notice>();
            var uri = new Uri(string.Format(url + "Notice/GetAllPendingNotice/{0}/", playerId));
            HttpResponseMessage response = client.GetAsync(uri).Result;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotPendingNoticeException(response.ReasonPhrase);
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();
                responseListNotice = JsonConvert.DeserializeObject<ICollection<Notice>>(responseAsString);
            }

            return responseListNotice;
        }

        public async Task<int> TotalNoticesCount(int playerId)
        {
            int numNotices = 0;
            var uri = new Uri(string.Format(url + "Notice/TotalNoticesCount/{0}/", playerId));
            HttpResponseMessage response;

            response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                string responseAsString = await response.Content.ReadAsStringAsync();

                numNotices = JsonConvert.DeserializeObject<int>(responseAsString);
            }

            return numNotices;
        }
    }
}
