/*
 * 
 * 
 * https://stackoverflow.com/questions/48393429/get-hub-context-in-signalr-core-from-within-another-object
 * 
 * */

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace IridiumLive.Hubs
{
    public class NotificationHub : Hub
    {

    }

    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
