using Belvoir.Bll.Services;
using Belvoir.Bll.Services.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Belvoir.Bll.Hubs
{
    public class NotificationHub : Hub { 

        private readonly NotificationServiceSignal _service;
        public NotificationHub(NotificationServiceSignal service)
        {
            _service = service; 
            _service.Notification_event += SendIndividual;
        }
    
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendIndividual(string userid,string message)
        {
            await Clients.User(userid).SendAsync("ReceiveMessage", message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Notification_event -= SendIndividual;
            }
            base.Dispose(disposing);
        }
    }
}
