using AppGeoFit.DataAccesLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.DataAccesLayer.Data.NoticeRestService
{
    public interface INoticeRestService
    {
        string url { get; set; }
        Task<Notice> GetNoticeAsync(int noticeId);
        //Task<ICollection<Notice>> GetAllAsync();
        Task<int> CreateNoticeAsync(Notice notice);
        Task<Boolean> DeleteNoticeAsync(int noticeId);
        Task<Boolean> UpdateNoticeAsync(Notice notice);
        Task<int> FindNoticeAsync(int receiverId, int messengerId, int sportId, string type);
    }
}
