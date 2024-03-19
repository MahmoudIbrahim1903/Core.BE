using Emeint.Core.BE.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public class Deserialization
    {
        public static T JsonStringToObject<T>(string jsonString, JsonNamingStrategy namingStrategy, MissingMemberHandling missingMemberHandling)
        {
            //TODO: Handle more case stratigies 
            JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.MissingMemberHandling = missingMemberHandling;

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

            var result = JsonConvert.DeserializeObject<T>(jsonString, _serializerSettings);
            return result;
        }

        public static T JsonFileToObject<T>(string filePath, JsonNamingStrategy namingStrategy, MissingMemberHandling missingMemberHandling)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                var jsonString = r.ReadToEnd();
                return JsonStringToObject<T>(jsonString, namingStrategy, missingMemberHandling);
            }
        }
    }
}
