using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Utilities;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.PushNotificationProviders.Firebase
{
    public abstract class FirebaseBasePushProvider
    {
        public async Task<PushMessageResponse> PushMessage(PushNotificationDto pushNotificationDto)
        {
            PushMessageResponse pushResponse = new PushMessageResponse();
            var data = new Dictionary<string, string>();
            List<string> tokensEn = pushNotificationDto.Destinations.Where(d => d.Language.ToLower() == "en" && !string.IsNullOrWhiteSpace(d.PushToken)).Select(d => d.PushToken).ToList();
            List<string> tokensAr = pushNotificationDto.Destinations.Where(d => d.Language.ToLower() == "ar" && !string.IsNullOrWhiteSpace(d.PushToken)).Select(d => d.PushToken).ToList();
            List<string> tokensSw = pushNotificationDto.Destinations.Where(d => d.Language.ToLower() == "sw" && !string.IsNullOrWhiteSpace(d.PushToken)).Select(d => d.PushToken).ToList();

            //Validating title & body and alert extra params
            //if exists will double check its value 
            //if not exists will be added 
            MulticastMessage payloadEn = null;
            MulticastMessage payloadAr = null;
            MulticastMessage payloadSw = null;
            if (tokensEn != null && tokensEn.Count > 0)
            {
                data = FillDataDictionary(pushNotificationDto, "en");
                //prepareing message en 
                payloadEn = new MulticastMessage()
                {
                    //Notification = new Notification()
                    //{
                    //    Body = pushMessageViewModel.MessageContentAr,
                    //    Title = pushMessageViewModel.MessageTitleAr,
                    //},
                    Data = data,
                    Tokens = tokensEn
                };
            }
            if (tokensAr != null && tokensAr.Count > 0)
            {
                data = FillDataDictionary(pushNotificationDto, "ar");
                //prepareing message ar 
                payloadAr = new MulticastMessage()
                {
                    //    Notification = new Notification()
                    //    {
                    //        Body = pushMessageViewModel.MessageContentAr,
                    //        Title = pushMessageViewModel.MessageTitleAr,
                    //    },
                    Data = data,
                    Tokens = tokensAr
                };
            }
            if (tokensSw != null && tokensSw.Count > 0)
            {
                data = FillDataDictionary(pushNotificationDto, "sw");
                //prepareing message Sw 
                payloadSw = new MulticastMessage()
                {
                    Data = data,
                    Tokens = tokensSw
                };
            }
            BatchResponse responseEn = null;
            BatchResponse responseAr = null;
            BatchResponse responseSw = null;

            //calling fire base to send message En 
            if (payloadEn != null)
                responseEn = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(payloadEn);

            //calling fire base to send message Ar
            if (payloadAr != null)
                responseAr = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(payloadAr);
            //calling fire base to send message Sw
            if (payloadSw != null)
                responseSw = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(payloadSw);

            //detecting failure tokens if exist
            var failedTokensAr = ExtractFailedMessagesTokens(responseAr, tokensAr);
            var failedTokensEn = ExtractFailedMessagesTokens(responseEn, tokensEn);
            var failedTokensSw = ExtractFailedMessagesTokens(responseSw, tokensSw);

            //return push response to manager 
            if ((failedTokensAr != null && failedTokensAr.Count > 0)
                || (failedTokensEn != null && failedTokensAr.Count > 0)
                || (failedTokensSw != null && failedTokensSw.Count > 0))
            {
                pushResponse.IsAllSucceeded = false;
                if (failedTokensAr?.Count > 0)
                    pushResponse.FailedUsersIdsExceptionsMessages.AddRange(failedTokensAr);
                if (failedTokensEn?.Count > 0)
                    pushResponse.FailedUsersIdsExceptionsMessages.AddRange(failedTokensEn);
                if (failedTokensSw?.Count > 0)
                    pushResponse.FailedUsersIdsExceptionsMessages.AddRange(failedTokensSw);

                if (pushResponse.FailedUsersIdsExceptionsMessages?.Count > 0)
                    foreach (var failedId in pushResponse.FailedUsersIdsExceptionsMessages)
                        pushResponse.FailureSentIds.Add(failedId.Key);

                pushResponse.SuccessfulSentIds = pushNotificationDto.Destinations.Select(d => d.UserId).Except(pushResponse.FailureSentIds).ToList();
            }
            else
            {
                pushResponse.IsAllSucceeded = true;
                pushResponse.SuccessfulSentIds = pushNotificationDto.Destinations.Select(d => d.UserId).ToList();
            }
            return pushResponse;
        }

        private Dictionary<string, string> FillDataDictionary(PushNotificationDto pushMessageViewModel, string language)
        {
            var data = new Dictionary<string, string>();
            var languageEnum = language == Language.en.ToString() ? Language.en : language == Language.ar.ToString() ? Language.ar : Language.sw;
            //set title  
            data.Add("title", LocalizationUtility.GetLocalizedText(pushMessageViewModel.MessageTitleEn, pushMessageViewModel.MessageTitleAr, languageEnum, pushMessageViewModel.MessageTitleSw));

            //set body 
            data.Add("body", LocalizationUtility.GetLocalizedText(pushMessageViewModel.MessageContentEn, pushMessageViewModel.MessageContentAr, languageEnum, pushMessageViewModel.MessageContentSw));

            //set message type 
            data.Add("type", pushMessageViewModel.MessageTypeCode);

            //set message type 
            data.Add("id", pushMessageViewModel.Id.ToString());

            //set alert 
            data.Add("alert", pushMessageViewModel.Alert.ToString());

            //checking if there are extra params 
            StringBuilder extraParametersStringBuilder = new StringBuilder();
            if (pushMessageViewModel.ExtraParams != null && pushMessageViewModel.ExtraParams.Count > 0)
            {
                //construct extra param object 
                extraParametersStringBuilder.Append("{");

                for (int i = 0; i < pushMessageViewModel.ExtraParams.Count; i++)
                {
                    var item = pushMessageViewModel.ExtraParams.ElementAt(i);
                    extraParametersStringBuilder.Append($"\"{item.Key}\" : \"{item.Value}\"");

                    if (i != pushMessageViewModel.ExtraParams.Count - 1)
                        extraParametersStringBuilder.Append(",");
                }

                extraParametersStringBuilder.Append("}");

                //set extra param key in data 
                data.Add("extra_params", extraParametersStringBuilder.ToString());
            }
            return data;
        }

        private List<KeyValuePair<string, string>> ExtractFailedMessagesTokens(BatchResponse response, List<string> tokens)
        {
            var failedTokens = new List<KeyValuePair<string, string>>();
            if (response != null && response.FailureCount > 0)
            {
                for (var i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        // The order of responses corresponds to the order of the registration tokens.
                        failedTokens.Add(new KeyValuePair<string, string>(tokens[i], response.Responses[i]?.Exception?.Message));
                    }
                }

                Console.WriteLine($"List of tokens that caused failures: {failedTokens}");
            }
            return failedTokens;
        }
    }
}
