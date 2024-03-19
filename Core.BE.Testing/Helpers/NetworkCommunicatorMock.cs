using Emeint.Core.BE.Utilities;
using Moq;
using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using Emeint.Core.BE.Domain.Enums;
using System.Net.Http;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Testing.Helpers
{
    public static class NetworkCommunicatorMock
    {
        public static INetworkCommunicator GetInstance(string expectedResponse)
        {
            var responseMessage = new HttpResponseMessage()
            {
                Content = new StringContent(expectedResponse),
                StatusCode = System.Net.HttpStatusCode.OK
            };

            var mock = new Mock<INetworkCommunicator>();

            mock.Setup(c => c.Get(It.IsAny<HttpGetRequest>(), It.IsAny<JsonNamingStrategy>()))
                .Returns(Task.FromResult(responseMessage));

            mock.Setup(c => c.Post(It.IsAny<HttpPostRequest>(), It.IsAny<JsonNamingStrategy>()))
                .Returns(Task.FromResult(responseMessage));

            return mock.Object;
        }
    }
}


