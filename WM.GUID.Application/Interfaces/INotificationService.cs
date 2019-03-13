using WM.GUID.Application.Notifications;
using System.Threading.Tasks;

namespace WM.GUID.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(Message message);
    }
}
