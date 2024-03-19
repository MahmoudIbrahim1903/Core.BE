using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Emeint.Core.BE.Utilities
{
    public static class LocalizationUtility
    {
        private static ResourceManager _arResourceManager;
        private static ResourceManager _enResourceManager;
        private static ResourceManager _swResourceManager;

        static LocalizationUtility()
        {
            Initialize();
        }

        private static void Initialize()
        {
            Assembly _assembly = Assembly.GetEntryAssembly();
            var arResourceName = _assembly?.GetManifestResourceNames()?.ToList()?.SingleOrDefault(e => e.Contains("-" + Language.ar))?.Replace(".resources", string.Empty);
            var enResourceName = _assembly?.GetManifestResourceNames()?.ToList()?.SingleOrDefault(e => e.Contains("-" + Language.en))?.Replace(".resources", string.Empty);
            var swResourceName = _assembly?.GetManifestResourceNames()?.ToList()?.SingleOrDefault(e => e.Contains("-" + Language.sw))?.Replace(".resources", string.Empty);

            _arResourceManager = new ResourceManager(arResourceName, _assembly);
            _enResourceManager = new ResourceManager(enResourceName, _assembly);
            _swResourceManager = new ResourceManager(swResourceName, _assembly);
        }

        public static string GetLocalizedText(string textEn, string textAr, Language userLanguage, string textSw = null)
        {
            //search for his requested Language , if found return it, else return other Language value
            if (userLanguage == Language.ar)
            {
                if (!string.IsNullOrEmpty(textAr))
                    return textAr;
                else
                    return textEn;
            }
            else if (userLanguage == Language.sw)
            {
                if (!string.IsNullOrEmpty(textSw))
                    return textSw;
                else
                    return textEn;
            }
            else
            {
                if (!string.IsNullOrEmpty(textEn))
                    return textEn;
                else
                {
                    if (!string.IsNullOrEmpty(textAr))
                        return textAr;
                    return textSw;
                }
            }
        }
     
        public static string GetLocalizedResourceText(string resourceCode, List<string> messageParameters = null, Language userLanguage = Language.en)
        {
            string messageDescription = string.Empty;

            if (messageParameters != null && messageParameters.Count > 0)
            {
                string messageText = string.Empty;
                if (userLanguage == Language.ar)
                    messageText = _arResourceManager.GetString(resourceCode);

                else if (userLanguage == Language.sw)
                {
                    messageText = _swResourceManager.GetString(resourceCode);
                    if(string.IsNullOrEmpty(messageText))
                      messageText = _enResourceManager.GetString(resourceCode);
                }

                else
                    messageText = _enResourceManager.GetString(resourceCode);

               messageDescription = messageText != null ? (string.Format(messageText, messageParameters.ToArray())) : messageText;
            }
            else
            {
                if (userLanguage == Language.ar)
                    messageDescription = _arResourceManager.GetString(resourceCode);
                else if (userLanguage == Language.sw)
                {
                    messageDescription = _swResourceManager.GetString(resourceCode);
                    if (string.IsNullOrEmpty(messageDescription))
                        messageDescription = _enResourceManager.GetString(resourceCode);
                }
                else
                    messageDescription = _enResourceManager.GetString(resourceCode);
            }

            return messageDescription;
        }


      


    }
}
