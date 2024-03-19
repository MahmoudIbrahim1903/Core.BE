using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Utilities
{
    public interface IWebRequestUtility
    {
        Task<T> Get<T>(HttpGetRequest request, JsonNamingStrategy namingStrategy);
        Task<T> Put<T>(HttpPutRequest request, JsonNamingStrategy namingStrategy);
        Task<T> Post<T>(HttpPostRequest request, JsonNamingStrategy namingStrategy);
        Task<T> PostFile<T>(HttpPostFileRequest request, JsonNamingStrategy namingStrategy);
        Task<T> Delete<T>(HttpDeleteRequest request, JsonNamingStrategy namingStrategy);
        void AddCurrentRequestHeaders(Domain.SeedWork.HttpRequest httpRequest);
        Task<HttpResponseMessage> Post(HttpPostRequest request, JsonNamingStrategy namingStrategy);
        Task<bool> IsDeserialized<T>(HttpResponseMessage httpResponseMessage, JsonNamingStrategy namingStrategy, MissingMemberHandling? missingMemeberHandleing);
        Task<T> DeserializeRepsonse<T>(HttpResponseMessage httpResponseMessage, JsonNamingStrategy namingStrategy);



    }
}

