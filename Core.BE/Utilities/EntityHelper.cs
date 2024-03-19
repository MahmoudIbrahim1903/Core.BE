using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Emeint.Core.BE.Utilities
{
    public static class EntityHelper
    {
        private static int MaxDepth = 1;

        public static List<string> GetPropertiesNames(Type type)
        {
            var result = new List<string>();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                result.Add(property.Name);
            }
            return result;
        }

        public static List<KeyValuePair<string, object>> GetPropertiesWithValues(object entity, int depth = 0)
        {
            var result = new List<KeyValuePair<string, object>>();
            var properties = TypeDescriptor.GetProperties(entity);
            foreach (PropertyDescriptor property in properties)
            {
                if (property.PropertyType.Namespace.ToLower().Equals("system") || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    result.Add(new KeyValuePair<string, object>(property.Name, property.GetValue(entity)));
                }
                else if (property.PropertyType.BaseType == typeof(System.Enum))
                {
                    var value = property.GetValue(entity);
                    result.Add(new KeyValuePair<string, object>(property.Name, Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()))));
                }
                else
                {
                    if (depth < MaxDepth)
                    {
                        var subEntityProps = GetPropertiesWithValues(property.GetValue(entity), depth + 1);
                        string typeNamePrefix = property.PropertyType.Name;
                        subEntityProps.ForEach(p =>
                        {
                            result.Add(new KeyValuePair<string, object>($"{typeNamePrefix}_{p.Key}", p.Value));
                        });
                    }
                }
            }
            return result;
        }

        public static string GetJsonPropertyName(Type type, string propertyName)
        {
            var jsonOptions = new JsonSerializerOptions();
            var propertyInfo = type.GetProperty(propertyName);
            var jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;

            return jsonPropertyName;
        }
    }
}

