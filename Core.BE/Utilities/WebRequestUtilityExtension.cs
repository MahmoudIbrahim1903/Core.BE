using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Utilities
{
    public static class WebRequestUtilityExtension
    {
        public static async Task<Response<T>> PostExtended<T>(this IWebRequestUtility webRequestUtility
             , HttpPostRequest request, JsonNamingStrategy namingStrategy)
        {
            var result = await webRequestUtility.Post<Response<T>>(request, namingStrategy);
            return result;
        }
        public static async Task<Response<T>> GetExtended<T>(this IWebRequestUtility webRequestUtility
          , HttpGetRequest request, JsonNamingStrategy namingStrategy)
        {
            var result = await webRequestUtility.Get<Response<T>>(request, namingStrategy);
            return result;
        }

        public static async Task<Response<T>> PutExtended<T>(this IWebRequestUtility webRequestUtility
         , HttpPutRequest request, JsonNamingStrategy namingStrategy)
        {
            var result = await webRequestUtility.Put<Response<T>>(request, namingStrategy);
            return result;
        }
    }
}
