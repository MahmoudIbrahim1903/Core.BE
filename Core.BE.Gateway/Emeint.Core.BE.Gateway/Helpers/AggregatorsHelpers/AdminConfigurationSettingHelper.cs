using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Emeint.Core.BE.Gateway.Helpers.AggregatorsHelpers
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]

    public class AdminConfigurationSettingHelper: ConfigurationSettingHelper
    {
        public List<ConfigurationSettingEnumType> EnumTypeDetails { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public int? SettingService { get; set; }
    }
}
