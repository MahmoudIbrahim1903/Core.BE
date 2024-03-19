using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public class HttpRequest
    {
        public HttpRequest(string url, List<KeyValuePair<string, string>> headers)
        {
            Headers = headers;
            Url = url;
        }

        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
    }

    public class HttpGetRequest : HttpRequest
    {
        public HttpGetRequest(string url, object querryParameters, List<KeyValuePair<string, string>> headers)
            : base(url, headers)
        {
            Method = HttpMethod.Get;
            QueryString = Serialization.ObjectToQueryString(querryParameters); // TODO: need to make sure the object is snake case resolved
        }

        public string QueryString { get; set; }
    }

    public class HttpPutRequest : HttpRequest
    {
        public HttpPutRequest(string url, object querryParameters, List<KeyValuePair<string, string>> headers, object body = null, string contentType = null, JsonNamingStrategy namingStrategy = JsonNamingStrategy.SnakeCase)
            : base(url, headers)
        {
            Method = HttpMethod.Put;
            QueryString = Serialization.ObjectToQueryString(querryParameters); // TODO: need to make sure the object is snake case resolved
            // default ContentType is Json
            ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;

            if (ContentType.Contains("form")) // TODO: Better to have a ContentType Enum to avoid hardcoded strings.
            {
                Body = Serialization.ObjectToQueryString(body); // query string format is similar to Forms variables, TODO: rename the method to be sutable for both
            }
            else // Json
            {
                Body = Serialization.ObjectToJsonString(body, namingStrategy);
            }
        }

        public string Body { get; set; }
        public string ContentType { get; set; }
        public string QueryString { get; set; }
    }
    public class HttpPostRequest : HttpRequest
    {
        public HttpPostRequest(string url, object body, string contentType, List<KeyValuePair<string, string>> headers, JsonNamingStrategy? namingStrategy) : base(url, headers)
        {
            Method = HttpMethod.Post;
            // default ContentType is Json
            ContentType = string.IsNullOrEmpty(contentType) ? "application/json" : contentType;

            // if form encoded
            if (ContentType.Contains("form")) // TODO: Better to have a ContentType Enum to avoid hardcoded strings.
            {
                Body = Serialization.ObjectToQueryString(body); // query string format is similar to Forms variables, TODO: rename the method to be sutable for both
            }
            else // Json
            {
                Body = Serialization.ObjectToJsonString(body, namingStrategy);
            }
        }

        public string Body { get; set; }
        public string ContentType { get; set; }

    }

    public class HttpDeleteRequest : HttpRequest
    {
        public HttpDeleteRequest(string url, object querryParameters, List<KeyValuePair<string, string>> headers)
            : base(url, headers)
        {
            Method = HttpMethod.Delete;
            QueryString = Serialization.ObjectToQueryString(querryParameters);
        }

        public string QueryString { get; set; }
    }

    public class HttpPostFileRequest : HttpRequest
    {
        public HttpPostFileRequest(string url, List<KeyValuePair<string, string>> formParameters, File file, ContentType? contentType, List<KeyValuePair<string, string>> headers
            , JsonNamingStrategy namingStrategy)
            : base(url, headers)
        {
            Method = HttpMethod.Post;
            ContentType = Enums.ContentType.Multipart.ToDescriptionString();
            FormParameters = formParameters;
            File = file;
        }
        public File File { get; set; }
        public List<KeyValuePair<string, string>> FormParameters { get; set; }
    }

}
