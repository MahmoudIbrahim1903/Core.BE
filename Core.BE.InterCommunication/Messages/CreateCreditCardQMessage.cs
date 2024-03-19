using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Messages
{
    public class CreateCreditCardQMessage
    {
        public string CardToken { set; get; }
        public string LastFourDigits { set; get; }
        public string AccountNumber { set; get; }
        public string CountryCode { set; get; }
        public string MerchantCode { set; get; }
        public string CardBrand { get; set; }
        public string ExpiryDate { set; get; }
    }
}
