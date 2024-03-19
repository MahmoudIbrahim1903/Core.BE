using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Exceptions;
using EZagelServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.SmsProviders
{
    public class EZagelSmsProvider : ISmsSender
    {
        private readonly IConfigurationManager _configurationManager;

        public EZagelSmsProvider(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }
        async Task<int> ISmsSender.SendSmsAsync(string number, string message)
        {
            //Plug in your SMS service here to send a text message.
            string smsUserName = _configurationManager.GetEzagelSMSUserName();
            string smsPassword = _configurationManager.GetEzagelSMSPassword();
            string smsSender = _configurationManager.GetEZagelSMSSender(); //"IPMagix";

            var client = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
            var result = client.Send_SMSAsync(Guid.NewGuid().ToString(), number, message, "", "", smsSender, smsUserName, smsPassword, "Confirmation").Result;

            return await GetResultCode(result);
        }

        private async Task<int> GetResultCode(string result)
        {
            switch (result)
            {
                case "Success":
                    return 0;
                case "Error Fire":
                    throw new SendSmsFailedException("-1", "Internal error happened.", -1);
                case "Msg ID Empty":
                    throw new SendSmsFailedException("-2", "You send empty message ID.", -2);
                case "Mobile !Digit":
                    throw new SendSmsFailedException("-3", "The mobile character not digit.", -3);
                case "Body Empty":
                    throw new SendSmsFailedException("-4", "Message body is empty.", -4);
                case "Valdity length":
                    throw new SendSmsFailedException("-5", "Validty format should be yyyyMMddHHmmss.", -5);
                case "StartTime length":
                    throw new SendSmsFailedException("-6", "Start time format should be yyyyMMddHHmmss.", -6);
                case "Valdity !Digit":
                    throw new SendSmsFailedException("-7", "Validty value should be digits only.", -7);
                case "StartTime !Digit":
                    throw new SendSmsFailedException("-8", "Start time value should be digits only.", -8);
                case "Sender more than 11 character":
                    throw new SendSmsFailedException("-9", "Sender  alpha numeric value should be 11 digits only.", -9);
                case "Sender length":
                    throw new SendSmsFailedException("-10", "Sender value length shouldn't be moe than 20 characters.", -10);
                case "Sender Arabic":
                    throw new SendSmsFailedException("-11", "Sender doesn't support arabic", -11);
                case "Sender Spechial Char":
                    throw new SendSmsFailedException("-12", "Sender doesn't support special characters.", -12);
                case "Invalid User":
                    throw new SendSmsFailedException("-13", "Username or password is incorrect.", -13);
                case "User !Active":
                    throw new SendSmsFailedException("-14", "User is not active.", -14);
                case "Low Credit":
                    throw new SendSmsFailedException("-14", "User don't have credit.", -14);
                default:
                    throw new SendSmsFailedException("-15", "Undefined error.", -15);
            }
        }
    }
}
