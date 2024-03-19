using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Utilities
{
    public class NetworkCommunicator : INetworkCommunicator
    {
        private const int DefaultPoolSize = 10;
        private readonly Queue<HttpClient> _httpClientQueue;
        private readonly HttpMessageHandler _httpFilter;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogger<NetworkCommunicator> _logger;
        private readonly IConfiguration _configuration;

        public NetworkCommunicator(ILogger<NetworkCommunicator> logger, IConfiguration configuration)
        {
            _httpFilter = GetDefaultFilter();
            _semaphore = new SemaphoreSlim(DefaultPoolSize);
            _httpClientQueue = new Queue<HttpClient>();
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> Put(HttpPutRequest request, JsonNamingStrategy namingStrategy)
        {
            HttpClient client = null;
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            Exception exception = null;

            try
            {
                client = await GetHttpClientInstance();

                //Add Control Fields To Request Headers
                request.Headers.ForEach(kv =>
                {
                    client.DefaultRequestHeaders.Remove(kv.Key); // keep headers updated
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                });

                httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{request.Url }?{request.QueryString}")
                {
                    Content = new StringContent(request.Body)
                };
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);

                httpResponseMessage = await client.SendAsync(httpRequestMessage);
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ex;
            }
            finally
            {
                // Add the HttpClient instance back to the queue.
                if (client != null)
                    _httpClientQueue.Enqueue(client);
                //.. Logging ..//
                LogRequestAndResponse(httpRequestMessage, exception, httpResponseMessage?.StatusCode.ToString());
            }
        }

        public async Task<HttpResponseMessage> Get(HttpGetRequest request, JsonNamingStrategy namingStrategy)
        {
            HttpClient client = null;
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            Exception exception = null;

            try
            {
                client = await GetHttpClientInstance();

                //Add Control Fields To Request Headers
                request.Headers.ForEach(kv =>
                {
                    client.DefaultRequestHeaders.Remove(kv.Key); // keep headers updated
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                });

                if (!string.IsNullOrEmpty(request.QueryString))
                    httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{request.Url }?{request.QueryString}");
                else if (string.IsNullOrEmpty(request.QueryString))
                    httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{request.Url }");

                httpResponseMessage = await client.SendAsync(httpRequestMessage);
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ex;
            }
            finally
            {
                // Add the HttpClient instance back to the queue.
                if (client != null)
                    _httpClientQueue.Enqueue(client);
                //.. Logging ..//
                LogRequestAndResponse(httpRequestMessage, exception, httpResponseMessage?.StatusCode.ToString());
            }
        }

        public async Task<HttpResponseMessage> Post(HttpPostRequest request, JsonNamingStrategy namingStrategy)
        {
            HttpClient client = null;
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            Exception exception = null;

            try
            {
                client = await GetHttpClientInstance();
                //Add Control Fields To Request Headers

                request.Headers.ForEach(kv =>
                {
                    client.DefaultRequestHeaders.Remove(kv.Key); // keep headers updated
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                });
                client.DefaultRequestHeaders.Remove("Accept"); // keep headers updated
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, request.Url)
                {
                    Content = new StringContent(request.Body)
                };
                

                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
                httpResponseMessage = await client.SendAsync(httpRequestMessage);

                // TODO: LogResponse
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ex;
            }
            finally
            {
                // Add the HttpClient instance back to the queue.
                if (client != null)
                    _httpClientQueue.Enqueue(client);
                //.. Logging ..//
                LogRequestAndResponse(httpRequestMessage, exception, httpResponseMessage?.StatusCode.ToString());
            }
        }

        public async Task<HttpResponseMessage> Delete(HttpDeleteRequest request, JsonNamingStrategy namingStrategy)
        {
            HttpClient client = null;
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            Exception exception = null;

            try
            {
                client = await GetHttpClientInstance();

                //Add Control Fields To Request Headers
                request.Headers.ForEach(kv =>
                {
                    client.DefaultRequestHeaders.Remove(kv.Key); // keep headers updated
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                });

                httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{request.Url }?{request.QueryString}");
                httpResponseMessage = await client.SendAsync(httpRequestMessage);
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ex;
            }
            finally
            {
                // Add the HttpClient instance back to the queue.
                if (client != null)
                    _httpClientQueue.Enqueue(client);
                //.. Logging ..//
                LogRequestAndResponse(httpRequestMessage, exception, httpResponseMessage?.StatusCode.ToString());
            }
        }

        public async Task<HttpResponseMessage> PostFile(HttpPostFileRequest request, JsonNamingStrategy namingStrategy)
        {
            HttpClient client = null;
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            Exception exception = null;

            try
            {
                client = await GetHttpClientInstance();
                client.DefaultRequestHeaders.Clear();
                request.Headers.ForEach(kv =>
                {
                    client.DefaultRequestHeaders.Remove(kv.Key); // keep headers updated
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                });
                var multipartForm = new MultipartFormDataContent();
                // File is the key in Request Body path is the Extension of file
                multipartForm.Add(request.File.Content, request.File.Key, request.File.NameAndExtension);
                foreach (var item in request.FormParameters)
                {
                    multipartForm.Add(new StringContent(item.Value), item.Key);
                }

                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{request.Url}")
                {
                    Content = multipartForm
                };
                // httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
                httpResponseMessage = await client.SendAsync(httpRequestMessage);

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw ex;
            }
            finally
            {
                // Add the HttpClient instance back to the queue.
                if (client != null)
                    _httpClientQueue.Enqueue(client);
                //.. Logging ..//
                LogRequestAndResponse(httpRequestMessage, exception, httpResponseMessage?.StatusCode.ToString());
            }
        }

        #region Helpers
        private static HttpMessageHandler GetDefaultFilter()
        {
            return new HttpClientHandler();
        }
        private async Task<HttpClient> GetHttpClientInstance()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            HttpClient client = null;

            // Try and get HttpClient from the queue
            if (_httpClientQueue.Any())
            {
                client = _httpClientQueue.Dequeue();
            }
            if (client == null)
            {
                client = new HttpClient(_httpFilter);
                var hasTimeOut = int.TryParse(_configuration["DefaultRequestTimeOutInSec"], out int defaultTimeOut);
                if (hasTimeOut)
                    client.Timeout = TimeSpan.FromSeconds(defaultTimeOut);
            }
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            //client.DefaultRequestHeaders.UserAgent.Add(
            //    new ProductInfoHeaderValue(new ProductHeaderValue("Xamarin_HttpClient", "1.0")));
            _semaphore.Release();
            return client;
        }

        // TODO: Move from here
        private void LogRequestAndResponse(HttpRequestMessage httpRequestMessage, Exception exception, string responseStatusCode)
        {
            string requestStr = GetRequestStr(httpRequestMessage);
            if (exception != null)
                _logger.LogError($"{requestStr}  + RECEIVED RESPONSE: {exception.Message} + StatusCode: {responseStatusCode}");
            else
                _logger.LogInformation($"{requestStr}  + RECEIVED RESPONSE: SUCCESS + StatusCode: {responseStatusCode}");
        }

        private string GetRequestStr(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage != null)
            {
                var requestStr = $"SENT REQUEST: {httpRequestMessage.Method} {httpRequestMessage.RequestUri}{Environment.NewLine}";
                var content = httpRequestMessage.Content == null ? string.Empty : httpRequestMessage.Content.ReadAsStringAsync().Result;
                requestStr += $"BODY: {Environment.NewLine}{content} {Environment.NewLine}";
                return requestStr;
            }
            else
            {
                return "request message is null";
            }
        }

        #endregion
    }
}
