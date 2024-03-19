using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Testing.Helpers;
using Emeint.Core.BE.Testing.Helpers.Dtos;
using Emeint.Core.BE.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Emeint.Core.BE.Testing.Utilities
{
    public class UtilitiesUnitTest
    {
        [Fact]
        public void PostExtendedDeserializationError_Test()
        {
            var response = "{\"Name\":\"Sayed Khalad\",\"Mobile\":\"+20102628760\",\"Address\":\"Cairo, Helwan\"}";
            IWebRequestUtility utility = new WebRequestUtility(NetworkCommunicatorMock.GetInstance(response), null, null);

            string url = "http://localhost/";
            string body = "{\"Message\":\"Testing Post Extended\"}";
            string contentType = "application/json";
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Signature", "SOME_KEY_VALUE"));

            var httpPostRequest = new HttpPostRequest(url, body, contentType, headers, JsonNamingStrategy.SnakeCase);
            //var rest = utility.PostExtended<Response<SamplePersonDto>>(httpPostRequest, JsonNamingStrategy.SnakeCase).Result;

            var ex = Assert.Throws<AggregateException>(
                () => utility.PostExtended<Response<SamplePersonVm>>(httpPostRequest, JsonNamingStrategy.SnakeCase).Result);

            Assert.NotNull(ex);
            Assert.Contains("could not find member", ex.Message.ToLower());
        }

        [Fact]
        public void IsValidAge()
        {
            int minimumAge = 21, maximumAge = 50;

            int wrongMinimumYear = DateTime.UtcNow.Year - 20;
            int validMinimumYear = DateTime.UtcNow.Year - 21;

            int wrongMaximumYear = DateTime.UtcNow.Year - 51;
            int validMaximumYear = DateTime.UtcNow.Year - 50;


            DateTime validMinimumBirthDate = new DateTime(validMinimumYear, 4, 1);
            DateTime wrongMinimumBirthDate = new DateTime(wrongMinimumYear, 4, 1);

            DateTime validMaximumBirthDate = new DateTime(validMaximumYear, 4, 1);
            DateTime wrongMaximumBirthDate = new DateTime(wrongMaximumYear, 4, 1);

            //validate minimum age
            Assert.True(AgeUtility.IsValidAge(validMinimumBirthDate, minimumAge));
            Assert.False(AgeUtility.IsValidAge(wrongMinimumBirthDate, minimumAge));

            //validate maximum age
            Assert.True(AgeUtility.IsValidAge(validMaximumBirthDate, null, maximumAge));
            Assert.False(AgeUtility.IsValidAge(wrongMaximumBirthDate, null, maximumAge));

            //validate minimum and maximum age
            Assert.True(AgeUtility.IsValidAge(validMaximumBirthDate, minimumAge, maximumAge));
            Assert.True(AgeUtility.IsValidAge(validMinimumBirthDate, minimumAge, maximumAge));
            Assert.False(AgeUtility.IsValidAge(wrongMinimumBirthDate, minimumAge, maximumAge));
            Assert.False(AgeUtility.IsValidAge(wrongMaximumBirthDate, minimumAge, maximumAge));
        }
    }
}
