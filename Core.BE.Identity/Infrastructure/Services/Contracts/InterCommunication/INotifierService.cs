using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts.InterCommunication
{
    public interface INotifierService<T> where T : class
    {
        Task NotifyAsync(T message);
    }
}
