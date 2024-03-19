using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Domain.Contracts
{
    public interface IConfigurationsProxy
    {
        Task<Response<bool>> UpdateConfiguration(string key, string value);
    }
}
