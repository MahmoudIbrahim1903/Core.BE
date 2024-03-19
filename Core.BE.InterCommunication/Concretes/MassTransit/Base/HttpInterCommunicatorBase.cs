using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.InterCommunication.Concretes.Base
{
    public abstract class HttpInterCommunicatorBase
    {
        protected string _baseUrl;
        protected IWebRequestUtility _webRequestUtility;
        public HttpInterCommunicatorBase(IWebRequestUtility webRequestUtility, string baseUrl)
        {
            _webRequestUtility = webRequestUtility;
            _baseUrl = baseUrl;
        }


    }
}
