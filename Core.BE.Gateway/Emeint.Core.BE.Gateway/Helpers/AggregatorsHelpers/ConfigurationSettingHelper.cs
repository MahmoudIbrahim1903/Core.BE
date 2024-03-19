using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Helpers.AggregatorsHelpers
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ConfigurationSettingHelper
    {
        public string key { set; get; }
        public string Value { set; get; }
        public string Group { set; get; }
        public bool IsRequired { get; set; }
        public int SettingType { get; set; }
    }
}
