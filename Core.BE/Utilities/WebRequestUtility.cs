using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Emeint.Core.BE.Utilities
{
    public class WebRequestUtility : IWebRequestUtility
    {
        #region OLD Code
        //public static T GetRequest<T>(string url)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            client.BaseAddress = new Uri(url);
        //            var response = client.GetAsync(url);
        //            var stringResult = response.Result.Content.ReadAsStringAsync();

        //            var deseralizedObject = JsonConvert.DeserializeObject<T>(stringResult.Result);
        //            return deseralizedObject;
        //        }
        //        catch
        //        {
        //            throw new ConnectionFailedException();
        //        }
        //    }
        //}
        #endregion
        private INetworkCommunicator Communicator { set; get; }
        private IHttpContextAccessor _httpContextAccessor { set; get; }

        private readonly IConfiguration _configuration;
        private readonly ILogger<WebRequestUtility> _logger;

        public WebRequestUtility(INetworkCommunicator communicator, IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
            ILogger<WebRequestUtility> logger)
        {
            Communicator = communicator;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<T> Get<T>(HttpGetRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.Get(request, namingStrategy);
            var result = await HandleResponse<T>(httpResponseMessage, namingStrategy);
            return result;
        }

        public async Task<HttpResponseMessage> Post(HttpPostRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.Post(request, namingStrategy);
            return httpResponseMessage;
        }


        public async Task<T> Put<T>(HttpPutRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.Put(request, namingStrategy);
            var result = await HandleResponse<T>(httpResponseMessage, namingStrategy);
            return result;
        }
        public async Task<T> Post<T>(HttpPostRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.Post(request, namingStrategy);
            var result = await HandleResponse<T>(httpResponseMessage, namingStrategy);
            return result;
        }

        public async Task<T> Delete<T>(HttpDeleteRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.Delete(request, namingStrategy);
            var result = await HandleResponse<T>(httpResponseMessage, namingStrategy);
            return result;
        }

        public async Task<T> PostFile<T>(HttpPostFileRequest request, JsonNamingStrategy namingStrategy)
        {
            var httpResponseMessage = await Communicator.PostFile(request, namingStrategy);
            var result = await HandleResponse<T>(httpResponseMessage, namingStrategy);
            return result;
        }

        public async Task<bool> IsDeserialized<T>(HttpResponseMessage httpResponseMessage, JsonNamingStrategy namingStrategy, MissingMemberHandling? missingMemeberHandleing)
        {
            try
            {
                var result = await DeserializeContent<T>(httpResponseMessage, namingStrategy, missingMemeberHandleing);
                return true;
            }
            catch
            {
                return false;
            }



        }

        public async Task<T> DeserializeRepsonse<T>(HttpResponseMessage httpResponseMessage, JsonNamingStrategy namingStrategy)
        {
            return await HandleResponse<T>(httpResponseMessage, namingStrategy);
        }


        #region Helpers

        public async Task<T> DeserializeContent<T>(HttpResponseMessage responseMessage, JsonNamingStrategy namingStrategy, MissingMemberHandling? missingMemberHandling)
        {
            FixInvalidCharset(responseMessage);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (!missingMemberHandling.HasValue)
            {
                missingMemberHandling = MissingMemberHandling.Error;
                if (_configuration["SerializerMissingMemberHandling"] == MissingMemberHandling.Ignore.ToString())
                    missingMemberHandling = MissingMemberHandling.Ignore;
            }

            var result = Deserialization.JsonStringToObject<T>(response, namingStrategy, missingMemberHandling.Value);
            return result;

        }
        /// <summary>
        ///     Fix invalid charset returned by some web sites.
        /// </summary>
        /// <param name="response">HttpResponseMessage instance.</param>
        private void FixInvalidCharset(HttpResponseMessage response)
        {
            if (response?.Content?.Headers?.ContentType?.CharSet != null)
            {
                // Fix invalid charset returned by some web sites.
                var charset = response.Content.Headers.ContentType.CharSet;
                if (charset.Contains("\""))
                    response.Content.Headers.ContentType.CharSet = charset.Replace("\"", string.Empty);
            }
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage httpResponseMessage, JsonNamingStrategy namingStrategy)
        {
            //if (mappedStatus != (int)ErrorCodes.Success)
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK || httpResponseMessage.StatusCode == HttpStatusCode.Created)
            {
                var result = await DeserializeContent<T>(httpResponseMessage, namingStrategy, null);
                return result;
            }
            else
            {
                string errorMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;

                _logger.LogInformation($"response status code is not indicating success, status code:{httpResponseMessage.StatusCode}, URL:{httpResponseMessage.RequestMessage.RequestUri} ,error message:{errorMessage}");

                if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();
                else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenException();
                else
                    throw new RequestToProviderFailedException(httpResponseMessage.StatusCode, errorMessage);
            }
        }

        public void AddCurrentRequestHeaders(Domain.SeedWork.HttpRequest httpRequest)
        {
            if (httpRequest.Headers == null)
                httpRequest.Headers = new List<KeyValuePair<string, string>>();

            foreach (var key in _httpContextAccessor.HttpContext.Request.Headers.Keys)
            {
                if (!httpRequest.Headers.Any(h => h.Key.Trim() == key.ToString()))
                {

                    if (key.ToString().Trim() == "Content-Type" || key.ToString().Trim() == "Content-Length" || key.ToString().Trim() == "Accept")
                        continue;

                    if (key.ToString().Trim() == "User-Agent" || key.ToString().Trim() == "Via")
                    {
                        _logger.LogInformation($"The {key.ToString().Trim()} of the requist: {_httpContextAccessor.HttpContext.Request.Headers[key.ToString()]}");
                    }

                    else
                        httpRequest.Headers.Add(new KeyValuePair<string, string>(key.ToString(), _httpContextAccessor.HttpContext.Request.Headers[key.ToString()]));
                }
            }
        }
        #endregion

    }
}