using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class LocalizedMappingBehavior : IMappingBehavior
    {
        public string[] Languages = { "En", "Ar" };

        /// <summary>
        /// This method maps propreties with name "Prop" of type LocalizedString to the strings of name "Prop"+"Xy" where "Xy" is one of the languages in the array
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void MapProperties<TSource, TDest>(TSource source, TDest dest)
        {

            //get all localized string props in destination
            foreach (
                var destProp in
                    typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.PropertyType.Name == "LocalizedString" && p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            {
                //get all string props in source that contain the destination property name
                //TODO optimize this refelection
                var sourceStringProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.Name == "String" && p.Name.Contains(destProp.Name));

                //init the localized string
                var localizedString = new LocalizedString();

                // loop on each language and find matching string
                for (int i = 0; i < Languages.Length; i++)
                {
                    foreach (var sourceStringProp in sourceStringProps)
                    {
                        if (sourceStringProp != null && source != null && sourceStringProp.Name == destProp.Name + Languages[i])
                        {
                            //set localized string values
                            string strValue = null;
                            var value = sourceStringProp.GetValue(source, null);
                            if (value != null)
                            {
                                strValue = value.ToString();
                            }
                            localizedString.Values.Add(new Value { Lang = Languages[i], Text = strValue });
                            //destProp.SetValue(dest, sourceStringProp.GetValue(source, null), null);
                        }
                    }
                }


                //set the destination localized string to the one mapped
                destProp.SetValue(dest, localizedString, null);

                //if (sourceProp != null && source != null)
                //{
                //    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                //}
            }
        }
    }
}
