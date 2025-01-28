using Belvoir.DAL.Models;
using Belvoir.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services
{
    public interface INotificationService
    {
        public Task<Response<IEnumerable<NotificationModel>>> Get_Notifiation(Guid userid);

    }
    public class NotificationService:INotificationService
    {
        private readonly INotificationRepository _repo;
        public NotificationService(INotificationRepository repo) {
        _repo = repo;
        }

        public async Task<Response<IEnumerable<NotificationModel>>> Get_Notifiation(Guid userid)
        {
            var notification = await _repo.GetNotification(userid);
            return new Response<IEnumerable<NotificationModel>>
            {
                Data = notification,
                StatusCode = 200,
                Message = "success"
            };
        }

    }
}
