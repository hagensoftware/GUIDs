using WM.GUID.Application.Interfaces;
using WM.GUID.Application.Notifications;
using System.Threading.Tasks;

namespace WM.GUID.Infrastructure
{
    public class NotificationService : INotificationService
    {
        public Task SendAsync(Message message)
        {
            return Task.CompletedTask;
        }
    }
}
