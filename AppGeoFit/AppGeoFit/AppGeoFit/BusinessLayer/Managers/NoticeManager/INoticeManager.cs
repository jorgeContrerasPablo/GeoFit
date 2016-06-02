using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppGeoFit.BusinessLayer.Managers.NoticeManager
{
    public interface INoticeManager
    {
        bool noticeIsPending(int receiverId, int messengerId, int sportId, string type);
        INoticeManager InitiateServices(bool test);
    }
}
