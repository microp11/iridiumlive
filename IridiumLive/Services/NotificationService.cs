/*
 * 
 * 
 * https://stackoverflow.com/questions/48393429/get-hub-context-in-signalr-core-from-within-another-object
 * 
 * */
 
using IridiumLive.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace IridiumLive.Services
{
    public interface INotificationService
    {
        public Task SendNotificationAsync(string message);
    }

    public class NotificationService : INotificationService
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
