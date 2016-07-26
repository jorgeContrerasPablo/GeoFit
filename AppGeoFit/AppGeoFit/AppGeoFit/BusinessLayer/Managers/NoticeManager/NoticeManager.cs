using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppGeoFit.BusinessLayer.Managers.NoticeManager.NoticeManager))]
namespace AppGeoFit.BusinessLayer.Managers.NoticeManager
{
    public class NoticeManager : INoticeManager
    {
        INoticeRestService restService;

        public NoticeManager(){}

        public INoticeManager InitiateServices(bool test)
        {
            restService = DependencyService.Get<INoticeRestService>();
            restService.url = test ? Constants.RestUrlTest : Constants.RestUrl;
            return this;
        }

        public bool NoticeIsPending(int receiverId, int messengerId, int sportId, string type)
        {
            bool isPending = false;
            try
            {
                isPending = restService.NoticeIsPending(receiverId, messengerId, sportId, type).Result;    
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NoticeNotFoundException)
                    {
                        return false;
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return isPending;
        }

        public Task<bool> UpdateNotice(Notice notice)
        {
            notice.Messenger = null;
            notice.Receiver = null;
            notice.Sport = null;
            return restService.UpdateNoticeAsync(notice);
        }

        public Task<int> CreateNotice(Notice notice)
        {
            return restService.CreateNoticeAsync(notice);
        }

        public Task<bool> DeleteNotice(int noticeId)
        {
            return restService.DeleteNoticeAsync(noticeId);
        }

        public ICollection<Notice> GetAllPendingNotice(int PlayerId)
        {
            ICollection<Notice> response = new Collection<Notice>();
            try
            {
                response = restService.GetAllPendingNotice(PlayerId).Result;
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    if (ex is NotPendingNoticeException)
                    {
                        throw new NotPendingNoticeException(ex.Message);
                    }
                    else
                        throw new Exception(ex.Message);
                }
            }
            return response;
        }
    }
}
