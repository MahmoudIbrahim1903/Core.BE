
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Domain.Contracts
{
    public interface IAggregationService
    {
        Task<Response<bool>> UpdateSettings(string key, string value);
    }
}
