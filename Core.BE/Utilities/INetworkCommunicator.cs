using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using System.Net.Http;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Utilities
{
    public interface INetworkCommunicator
    {
        Task<HttpResponseMessage> Get(HttpGetRequest request, JsonNamingStrategy namingStrategy);
        Task<HttpResponseMessage> Put(HttpPutRequest request, JsonNamingStrategy namingStrategy);

        Task<HttpResponseMessage> Post(HttpPostRequest request, JsonNamingStrategy namingStrategy);
        Task<HttpResponseMessage> PostFile(HttpPostFileRequest request, JsonNamingStrategy namingStrategy);
        Task<HttpResponseMessage> Delete(HttpDeleteRequest request, JsonNamingStrategy namingStrategy);
    }
}