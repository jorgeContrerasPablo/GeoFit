using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.BusinessLayer.Managers.NoticeManager
{
    public interface INoticeManager
    {
        INoticeManager InitiateServices(bool test);
        bool NoticeIsPending(int receiverId, int messengerId, int sportId, string type);
        ICollection<Notice>  GetAllPendingNotice(int PlayerId);
        Task<int> CreateNotice(Notice notice);
        Task<Boolean> DeleteNotice(int noticeId);
        bool UpdateNotice(Notice notice);
        Notice GetNotice(int noticeId);
        int TotalNoticesCount(int playerId);
    }
}
