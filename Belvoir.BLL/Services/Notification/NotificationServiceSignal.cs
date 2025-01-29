using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belvoir.Bll.Services.Notification
{
    public interface INotificationServiceSignal
    {
        public Task TriggerNotification(string user, string message);

    }
    public class NotificationServiceSignal:INotificationServiceSignal
    {
        
        public delegate Task MessageSentEventHandler(string user, string message);

        public event MessageSentEventHandler Notification_event;
        public async Task TriggerNotification(string user, string message)
        {
            
          await Notification_event.Invoke(user, message);
            
        }
    }
}
