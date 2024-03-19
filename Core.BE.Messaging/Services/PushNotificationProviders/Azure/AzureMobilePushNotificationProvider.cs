using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Microsoft.Extensions.Logging;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Microsoft.Azure.NotificationHubs;
using System.Text;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Exceptions;

namespace Emeint.Core.BE.Notifications.Services.PushNotificationProviders.Azure
{
    public class AzureMobilePushNotificationProvider : IMobilePushNotificationProvider
    {

        #region Fields
        private readonly ILogger<AzureMobilePushNotificationProvider> _logger;
        private readonly string _typeName;
        private readonly IConfigurationManager _configurationManager;
        private readonly NotificationHubClient _hub;
        #endregion

        #region Constuctor
        public AzureMobilePushNotificationProvider(ILogger<AzureMobilePushNotificationProvider> logger, IConfigurationManager configurationManager/*, string type*/)
        {
            _logger = logger;
            _configurationManager = configurationManager;
            _typeName = GetType().Name;
            _hub = NotificationHubClient.CreateClientFromConnectionString(configurationManager.GetNotificationHubConnectionString(), configurationManager.GetNotificationHubPath());
        }
        #endregion

        public async Task<BaseResponse> SendToAndroidDevices(string message, List<string> usersTag)
        {
            var result = await _hub.SendGcmNativeNotificationAsync(message, usersTag);
            return new Response<NotificationOutcome>() { Data = result };
        }

        public async Task<BaseResponse> SendToIOSDevices(string message, List<string> userIds)
        {
            var result = await _hub.SendAppleNativeNotificationAsync(message, userIds);
            return new Response<NotificationOutcome>() { Data = result };
        }

        public async Task<bool> CreateOrUpdatePushNotificationRegistration(DeviceInstallationViewModel installation)
        {
            string installationId = string.Format("{0}_{1}", installation.UserId, installation.DeviceId);

            // Add installationId as the main tag to reach the device
            var finalTags = new List<string> { installationId };
            if (installation.Tags != null && installation.Tags.Count >= 0)
            {
                finalTags.AddRange(installation.Tags);
            }

            var azureInstallation = new Installation();
            azureInstallation.InstallationId = installationId;
            azureInstallation.Tags = finalTags;
            azureInstallation.PushChannel = installation.PushNotificationToken;

            if (installation.Platform.ToLower() == "android")
                azureInstallation.Platform = NotificationPlatform.Gcm;
            else if (installation.Platform.ToLower() == "ios")
                azureInstallation.Platform = NotificationPlatform.Apns;
            else
                throw new BE.Domain.Exceptions.InvalidParameterException("Platform", installation.Platform);

            try
            {
                await _hub.CreateOrUpdateInstallationAsync(azureInstallation);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"create or update azure installation failed {ex.Message}");
                throw new UpdatePushNotificationRegistrationFailedException();
            }
            return true;
        }
        public async Task<bool> DeleteRegistration(DeviceInstallationViewModel installation)
        {
            var installationId = $"{installation.UserId}_{installation.DeviceId}";

            Installation userInstallation = await _hub.GetInstallationAsync(installationId);

            if (userInstallation != null)
                await _hub.DeleteInstallationAsync(installationId);

            else throw new Exception("This device is not registered on Hub");

            return true;
        }

        public async Task<PushMessageResponse> PushMessage(PushNotificationDto pushMessageViewModel)
        {
            PushMessageResponse responseToMessaginManager = new PushMessageResponse();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Start Push message");

            Task<BaseResponse> androidOutcomeEn = null;
            Task<BaseResponse> androidOutcomeAr = null;
            Task<BaseResponse> androidOutcomeSw = null;

            Task<BaseResponse> iosOutcomeEn = null;
            Task<BaseResponse> iosOutcomeAr = null;
            Task<BaseResponse> iosOutcomeSw = null;

            string pushMessageExceptionValue = null;
            int delayTime = _configurationManager.GetMessageDelayValue();

            _logger.LogInformation($"{_typeName}.{methodName}: Setting extra params");

            StringBuilder extraParametersStringBuilder = new StringBuilder();
            extraParametersStringBuilder.Append("{");

            if (pushMessageViewModel.ExtraParams != null)
            {
                for (int i = 0; i < pushMessageViewModel.ExtraParams.Count; i++)
                {
                    var item = pushMessageViewModel.ExtraParams.ElementAt(i);
                    extraParametersStringBuilder.Append($"\"{item.Key}\" : \"{item.Value}\"");

                    if (i != pushMessageViewModel.ExtraParams.Count - 1)
                        extraParametersStringBuilder.Append(",");
                }
            }
            extraParametersStringBuilder.Append("}");

            _logger.LogInformation($"{_typeName}.{methodName}: Setting notification body");
            var notifEn =
            "{" +
                "\"data\" : {" +
                     "\"body\":\"" + pushMessageViewModel.MessageContentEn + "\"," +
                     "\"title\":\"" + pushMessageViewModel.MessageTitleEn + "\"" + "," +
                     "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                     "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                     "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                     "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
                " }," +
                "\"notification\" : {" +
                    "\"body\" :  \"" + pushMessageViewModel.MessageContentEn + "\"," +
                    "\"title\" : \"" + pushMessageViewModel.MessageTitleEn + "\"" +
                //"," + "\"icon\" : \"home_logo\" " +
                "}" +
            "}";

            var notifAr =
           "{" +
               "\"data\" : {" +
                    "\"body\":\"" + pushMessageViewModel.MessageContentAr + "\"," +
                    "\"title\":\"" + pushMessageViewModel.MessageTitleAr + "\"" + "," +
                    "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                    "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                    "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                    "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
               " }," +
                "\"notification\" : {" +
                    "\"body\" :  \"" + pushMessageViewModel.MessageContentAr + "\"," +
                    "\"title\" : \"" + pushMessageViewModel.MessageTitleAr + "\"" +
                //"," + "\"icon\" : \"home_logo\" " +
                "}" +
            "}";

            var notifSw =
           "{" +
               "\"data\" : {" +
                    "\"body\":\"" + pushMessageViewModel.MessageContentSw + "\"," +
                    "\"title\":\"" + pushMessageViewModel.MessageTitleSw + "\"" + "," +
                    "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                    "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                    "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                    "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
               " }," +
                "\"notification\" : {" +
                    "\"body\" :  \"" + pushMessageViewModel.MessageContentSw + "\"," +
                    "\"title\" : \"" + pushMessageViewModel.MessageTitleSw + "\"" +
                //"," + "\"icon\" : \"home_logo\" " +
                "}" +
            "}";
            if (pushMessageViewModel.Destinations == null)
            {
                _logger.LogInformation($"{_typeName}.{methodName}: No Destinations");
                throw new ArgumentNullException(nameof(pushMessageViewModel.Destinations));
            }
            else
            {
                _logger.LogInformation($"{_typeName}.{methodName}: Filtering by platform");

                List<string> androidUserIdsEn = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Android && d.Language == "en").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();
                List<string> androidUserIdsAr = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Android && d.Language == "ar").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();
                List<string> androidUserIdsSw = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Android && d.Language == "sw").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();

                List<string> iosUserIdsEn = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Ios && d.Language == "en").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();
                List<string> iosUserIdsAr = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Ios && d.Language == "ar").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();
                List<string> iosUserIdsSw = pushMessageViewModel.Destinations.Where(d => d.Platform == Domain.Enums.Platform.Ios && d.Language == "sw").Select(x => string.Format("{0}_{1}", x.UserId, x.DeviceId)).ToList();

                // en language
                if (androidUserIdsEn != null && androidUserIdsEn.Count > 0)
                {
                    if (androidUserIdsEn.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android more than 20 tag (EN)");
                        int i = 0;
                        while (i < androidUserIdsEn.Count())
                        {
                            var twentyUserIds = androidUserIdsEn.Skip(i).Take(20);
                            androidOutcomeEn = SendToAndroidDevices(notifEn, twentyUserIds.ToList());
                            i += 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: First (EN): " + androidUserIdsEn.FirstOrDefault());
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android: count: " + androidUserIdsEn.Count() + "First user: " + androidUserIdsEn.FirstOrDefault());
                        androidOutcomeEn = SendToAndroidDevices(notifEn, androidUserIdsEn);
                    }
                }

                // ar language
                if (androidUserIdsAr != null && androidUserIdsAr.Count > 0)
                {
                    if (androidUserIdsAr.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android more than 20 tag (AR)");
                        int i = 0;
                        while (i < androidUserIdsAr.Count())
                        {
                            var twentyUserIds = androidUserIdsAr.Skip(i).Take(20);
                            androidOutcomeAr = SendToAndroidDevices(notifAr, twentyUserIds.ToList());
                            i = i + 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: First (AR):" + androidUserIdsAr.FirstOrDefault());
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android: count: " + androidUserIdsAr.Count() + "First user: " + androidUserIdsAr.FirstOrDefault());
                        androidOutcomeAr = SendToAndroidDevices(notifAr, androidUserIdsAr);
                    }
                }
                // sw language
                if (androidUserIdsSw != null && androidUserIdsSw.Count > 0)
                {
                    if (androidUserIdsSw.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android more than 20 tag (SW)");
                        int i = 0;
                        while (i < androidUserIdsSw.Count())
                        {
                            var twentyUserIds = androidUserIdsSw.Skip(i).Take(20);
                            androidOutcomeSw = SendToAndroidDevices(notifSw, twentyUserIds.ToList());
                            i = i + 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: First (SW):" + androidUserIdsSw.FirstOrDefault());
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To android: count: " + androidUserIdsSw.Count() + "First user: " + androidUserIdsSw.FirstOrDefault());
                        androidOutcomeSw = SendToAndroidDevices(notifSw, androidUserIdsSw);
                    }
                }

                if (androidOutcomeEn != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!androidOutcomeEn.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                    }
                    if (androidOutcomeEn.IsCompletedSuccessfully)
                    {
                        responseToMessaginManager.SuccessfulSentIds.AddRange(from string userId in androidUserIdsEn select userId);
                    }
                    else
                    {
                        pushMessageExceptionValue = androidOutcomeEn?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in androidUserIdsEn)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }
                    #region commented
                    //if (!((androidPushOutcome.State == NotificationOutcomeState.Abandoned) ||
                    //    (androidPushOutcome.State == NotificationOutcomeState.Unknown)))
                    //{
                    //    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome success");
                    //    //Update Message Status to be sent
                    //    foreach (var userId in androidUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Sent);
                    //    }
                    //}
                    //else
                    //{
                    //    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome failed");
                    //    //Update Message Status to be failed
                    //    foreach (var userId in androidUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Failed);
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome is null");
                }

                if (androidOutcomeAr != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!androidOutcomeAr.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                    }
                    if (androidOutcomeAr.IsCompletedSuccessfully)
                    {
                        //string userId;
                        foreach (var userId in androidUserIdsAr)
                        {
                            responseToMessaginManager.SuccessfulSentIds.Add(userId);
                        }
                    }
                    else
                    {
                        pushMessageExceptionValue = androidOutcomeAr?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in androidUserIdsAr)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }
                    #region commented
                    //if (!((androidPushOutcome.State == NotificationOutcomeState.Abandoned) ||
                    //    (androidPushOutcome.State == NotificationOutcomeState.Unknown)))
                    //{
                    //    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome success");
                    //    //Update Message Status to be sent
                    //    foreach (var userId in androidUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Sent);
                    //    }
                    //}
                    //else
                    //{
                    //    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome failed");
                    //    //Update Message Status to be failed
                    //    foreach (var userId in androidUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Failed);
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome is null");
                }
                if (androidOutcomeSw != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!androidOutcomeSw.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                    }
                    if (androidOutcomeSw.IsCompletedSuccessfully)
                    {
                        //string userId;
                        foreach (var userId in androidUserIdsSw)
                        {
                            responseToMessaginManager.SuccessfulSentIds.Add(userId);
                        }
                    }
                    else
                    {
                        pushMessageExceptionValue = androidOutcomeSw?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in androidUserIdsAr)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: Android push outcome is null");
                }

                #region ios
                //var alert = "{\"apns\":{\"alert\":\"" + pushMessageViewModel.MessageContent + "\"}}";

                var alertEn = "{\"aps\" : {" +
                                               "\"alert\" : {" +
                                                                "\"title\" : \"" + pushMessageViewModel.MessageTitleEn + "\"," +
                                                               //"\"subtitle\" : \"Five Card Draw\"," +
                                                               "\"body\": \"" + pushMessageViewModel.MessageContentEn +
                                                           "\"}," +
                                               "\"sound\":\"default\"" +
                                           "}," +
                                "\"data\" : {" +
                                                "\"body\":\"" + pushMessageViewModel.MessageContentEn + "\"," +
                                                    "\"title\":\"" + pushMessageViewModel.MessageTitleEn + "\"" + "," +
                                                    "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                                                    "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                                                    "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                                                    "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
                                            "}" +
                                "}";

                var alertAr = "{\"aps\" : {" +
                                               "\"alert\" : {" +
                                                                "\"title\" : \"" + pushMessageViewModel.MessageTitleAr + "\"," +
                                                               //"\"subtitle\" : \"Five Card Draw\"," +
                                                               "\"body\": \"" + pushMessageViewModel.MessageContentAr +
                                                           "\"}," +
                                               "\"sound\":\"default\"" +
                                           "}," +
                                "\"data\" : {" +
                                                "\"body\":\"" + pushMessageViewModel.MessageTitleAr + "\"," +
                                                    "\"title\":\"" + pushMessageViewModel.MessageTitleAr + "\"" + "," +
                                                    "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                                                    "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                                                    "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                                                    "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
                                            "}" +
                                "}";

                var alertSw = "{\"aps\" : {" +
                                              "\"alert\" : {" +
                                                               "\"title\" : \"" + pushMessageViewModel.MessageTitleSw + "\"," +
                                                              //"\"subtitle\" : \"Five Card Draw\"," +
                                                              "\"body\": \"" + pushMessageViewModel.MessageContentSw +
                                                          "\"}," +
                                              "\"sound\":\"default\"" +
                                          "}," +
                               "\"data\" : {" +
                                               "\"body\":\"" + pushMessageViewModel.MessageTitleSw + "\"," +
                                                   "\"title\":\"" + pushMessageViewModel.MessageTitleSw + "\"" + "," +
                                                   "\"id\":\"" + pushMessageViewModel.Id + "\"" + "," +
                                                   "\"alert\":\"" + pushMessageViewModel.Alert + "\"" + "," +
                                                   "\"extra_params\":" + extraParametersStringBuilder.ToString() + "," +
                                                   "\"type\":\"" + pushMessageViewModel.MessageTypeCode + "\"" +
                                           "}" +
                               "}";
                // en language
                if (iosUserIdsEn != null && iosUserIdsEn.Count > 0)
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS devices count: " + iosUserIdsEn.Count);
                    if (iosUserIdsEn.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios more than 20 tag");
                        int i = 0;
                        while (i < iosUserIdsEn.Count())
                        {
                            var twentyUserIds = iosUserIdsEn.Skip(i).Take(20);
                            iosOutcomeEn = SendToIOSDevices(alertEn, twentyUserIds.ToList());
                            i = i + 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios: count: " + iosUserIdsEn.Count() + "First user: " + iosUserIdsEn.FirstOrDefault());
                        iosOutcomeEn = SendToIOSDevices(alertEn, iosUserIdsEn);
                    }
                }

                // ar language
                if (iosUserIdsAr != null && iosUserIdsAr.Count > 0)
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS devices count: " + iosUserIdsAr.Count);
                    if (iosUserIdsAr.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios more than 20 tag");
                        int i = 0;
                        while (i < iosUserIdsAr.Count())
                        {
                            var twentyUserIds = iosUserIdsAr.Skip(i).Take(20);
                            iosOutcomeAr = SendToIOSDevices(alertAr, twentyUserIds.ToList());
                            i = i + 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios: count: " + iosUserIdsAr.Count() + "First user: " + iosUserIdsAr.FirstOrDefault());
                        iosOutcomeAr = SendToIOSDevices(alertAr, iosUserIdsAr);
                    }
                }
                // sw language
                if (iosUserIdsSw != null && iosUserIdsSw.Count > 0)
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS devices count: " + iosUserIdsSw.Count);
                    if (iosUserIdsSw.Count() > 20)
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios more than 20 tag");
                        int i = 0;
                        while (i < iosUserIdsSw.Count())
                        {
                            var twentyUserIds = iosUserIdsSw.Skip(i).Take(20);
                            iosOutcomeSw = SendToIOSDevices(alertSw, twentyUserIds.ToList());
                            i = i + 20;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{_typeName}.{methodName}: Send To ios: count: " + iosUserIdsSw.Count() + "First user: " + iosUserIdsSw.FirstOrDefault());
                        iosOutcomeSw = SendToIOSDevices(alertSw, iosUserIdsSw);
                    }
                }

                if (iosOutcomeEn != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!iosOutcomeEn.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                        //if (!iosOutcome.IsCompletedSuccessfully)
                        //    t.Wait();
                    }
                    if (iosOutcomeEn.IsCompletedSuccessfully)
                    {
                        foreach (var userId in iosUserIdsEn)
                        {
                            responseToMessaginManager.SuccessfulSentIds.Add(userId);
                        }
                    }
                    else
                    {
                        pushMessageExceptionValue = iosOutcomeEn?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in iosUserIdsEn)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }

                    #region commented
                    //if (!((iosPushOutcome.State == NotificationOutcomeState.Abandoned) ||
                    //    (iosPushOutcome.State == NotificationOutcomeState.Unknown)))
                    //{
                    //    //Update Message Status to be sent
                    //    foreach (var userId in iosUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Sent);
                    //    }
                    //}
                    //else
                    //{
                    //    //Update Message Status to be Failed
                    //    foreach (var userId in iosUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Failed);
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS push outcome is null");
                }


                if (iosOutcomeAr != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!iosOutcomeAr.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                        //if (!iosOutcome.IsCompletedSuccessfully)
                        //    t.Wait();
                    }
                    if (iosOutcomeAr.IsCompletedSuccessfully)
                    {
                        foreach (var userId in iosUserIdsAr)
                        {
                            responseToMessaginManager.SuccessfulSentIds.Add(userId);
                        }
                    }
                    else
                    {
                        pushMessageExceptionValue = iosOutcomeAr?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in iosUserIdsAr)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }

                    #region commented
                    //if (!((iosPushOutcome.State == NotificationOutcomeState.Abandoned) ||
                    //    (iosPushOutcome.State == NotificationOutcomeState.Unknown)))
                    //{
                    //    //Update Message Status to be sent
                    //    foreach (var userId in iosUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Sent);
                    //    }
                    //}
                    //else
                    //{
                    //    //Update Message Status to be Failed
                    //    foreach (var userId in iosUserIds)
                    //    {
                    //        UpdateMessageStatus(pushMessageViewModel.Code, userId, Status.Failed);
                    //    }
                    //}
                    #endregion
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS push outcome is null");
                }
                
                if (iosOutcomeSw != null)
                {
                    //check if the azure finished the sending, if not wait 2 seconds to finish then begin to update message status
                    if (!iosOutcomeSw.IsCompletedSuccessfully)
                    {
                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(delayTime);
                        });
                        t.Wait();
                        //if (!iosOutcome.IsCompletedSuccessfully)
                        //    t.Wait();
                    }
                    if (iosOutcomeSw.IsCompletedSuccessfully)
                    {
                        foreach (var userId in iosUserIdsSw)
                        {
                            responseToMessaginManager.SuccessfulSentIds.Add(userId);
                        }
                    }
                    else
                    {
                        pushMessageExceptionValue = iosOutcomeSw?.Exception?.InnerException?.InnerException?.Message?.ToString();
                        foreach (var userId in iosUserIdsSw)
                        {
                            responseToMessaginManager.FailureSentIds.Add(userId);
                            responseToMessaginManager.FailedUsersIdsExceptionsMessages.Add(new KeyValuePair<string, string>(userId, pushMessageExceptionValue));
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"{_typeName}.{methodName}: iOS push outcome is null");
                }
                #endregion
            }

            return responseToMessaginManager;
        }
    }
}
