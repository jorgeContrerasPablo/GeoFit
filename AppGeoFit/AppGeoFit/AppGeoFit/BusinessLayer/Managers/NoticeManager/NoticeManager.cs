using AppGeoFit.DataAccesLayer.Data.NoticeRestService;
using AppGeoFit.DataAccesLayer.Data.NoticeRestService.Exceptions;
using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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

        public bool noticeIsPending(int receiverId, int messengerId, int sportId, string type)
        {
            Notice notice = new Notice();
            notice.MessengerID = messengerId;
            notice.ReceiverID = receiverId;
            notice.SportID = sportId;
            notice.Type = type;
            try
            {
                notice.NoticeID = restService.FindNoticeAsync(receiverId, messengerId, sportId, type).Result;
                notice = restService.GetNoticeAsync(notice.NoticeID).Result;
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
            return (notice.Accepted == null);
        }
    }
}
