using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace Emeint.Core.BE.Domain.SeedWork
{
    class Serialization
    {
        public static string ObjectToJsonString(object obj, JsonNamingStrategy? namingStrategy)
        {
            JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

            switch (namingStrategy)
            {
                case JsonNamingStrategy.PascalCase:
                    _serializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new DefaultNamingStrategy()
                    };
                    break;
                case JsonNamingStrategy.SnakeCase:
                    _serializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    break;
                case JsonNamingStrategy.CamelCase:
                    _serializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };
                    break;
                default:
                    break;
            }

            return obj == null ? string.Empty : JsonConvert.SerializeObject(obj, _serializerSettings);
        }

        public static string ObjectToQueryString(object obj)
        {
            var queryString = string.Empty;

            if (obj == null) return queryString;

            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            queryString = string.Join("&", properties.ToArray());

            return queryString;
        }

    }
}
