using System;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Infrastructure.Idempotency
{
    public interface ICommandManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id, string command);
    }
}
