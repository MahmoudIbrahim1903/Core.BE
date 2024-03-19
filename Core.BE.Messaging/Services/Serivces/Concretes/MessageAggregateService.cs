using Castle.Compatibility;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Exceptions;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders;
using Emeint.Core.BE.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Manager.Concretes
{
    public class MessageAggregateService : IMessageAggregateService
    {
        #region Fields 
        private const string MessageTypesKey = "message_types";
        private const string MessageTemplatesKey = "message_templates";

        private readonly IMessageStatusRepository _messageStatusRepository;
        private readonly IMessageTypeRepository _messageTypeRepository;
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IIdentityService _identityService;
        private readonly IUserService _userManager;
        private readonly ICacheService _cacheService;
        private readonly IMessageDestinationRepository _messageDestinationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MessageAggregateService> _logger;
        private readonly string _typeName;
        private readonly IMobilePushNotificationProvider _mobilePushNotificationProvider;
        private readonly IWebBrowserPushNotificationProvider _webBrowserPushNotificationProvider;
        private readonly IConfigurationManager _configurationManager;
        #endregion

        #region CTOR
        public MessageAggregateService(IMessageStatusRepository messageStatusRepository, IMessageDestinationRepository messageDestinationRepository,
                                       IMessageTypeRepository messageTypeRepository, IMessageTemplateRepository messageTemplateRepository,
                                       IMessageRepository messageRepository, IUserService userManager, IIdentityService identityService,
                                       ICacheService cacheService, IUserRepository userRepository, ILogger<MessageAggregateService> logger,
                                       IMobilePushNotificationProvider mobilePushNotificationProvider,
                                       IWebBrowserPushNotificationProvider webBrowserPushNotificationProvider, IConfiguration configuration, IConfigurationManager configurationManager)
        {
            _messageRepository = messageRepository;
            _messageStatusRepository = messageStatusRepository;
            _messageTemplateRepository = messageTemplateRepository;
            _messageTypeRepository = messageTypeRepository;
            _userManager = userManager;
            _identityService = identityService;
            _cacheService = cacheService;
            _messageDestinationRepository = messageDestinationRepository;
            _userRepository = userRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _mobilePushNotificationProvider = mobilePushNotificationProvider;
            _webBrowserPushNotificationProvider = webBrowserPushNotificationProvider;
            _configurationManager = configurationManager;
        }
        #endregion
        #region Methods

        #region Send Notification Methods
        public async Task<Response<SystemEventNotificationDto>> Send(SendNotificationRequestDto message)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Get Message Type");
            MessageTypeDTO messageType = GetMessageTypeByCode(message.MessageTypeCode);

            //Prepare message Body according to template
            PrepareMessageTitleAndBody(message, messageType, out string messageContentEn, out string messageTitleEn, out string messageContentAr, out string messageTitleAr, out string messageContentSw, out string messageTitleSw);

            // Save request in database
            var messageId = SaveMessage(message, messageTitleAr, messageTitleEn, messageTitleSw, messageContentAr, messageContentEn, messageContentSw, messageType.TypeId);


            // if caller wants to send the message as push, else it will only be saved in DB.
            if (message.SendAsPush)
            {
                var pushMessageViewModel = PrepareMessageForPush(message, messageId, messageContentEn, messageTitleEn,
                    messageContentAr, messageTitleAr, messageContentSw, messageTitleSw);

                // this call is used in debugging only
                var response = await PushMessage(pushMessageViewModel);

                //var response = Task.Run<PushMessageResponse>(async () =>
                //{
                //    return await PushMessage(pushMessageViewModel);
                //}).Result;

                if (response != null)
                {
                    if (response.SuccessfulSentIds != null && response.SuccessfulSentIds.Count > 0)
                    {
                        //string userId;
                        _logger.LogInformation("push message is succeeded");

                        response.SuccessfulSentIds
                            .Where(userId => message.MessageSource != MessageSource.AdminMessage || (message.MessageSource == MessageSource.AdminMessage && _configurationManager.GetTrackAdminNotifications()))
                            .ForEach(userId =>
                                UpdateMessageStatus(messageId, userId.Split('_').ElementAt(0), Status.Sent, null));

                        //foreach (var userId in response.SuccessfulSentIds)
                        //{
                        //    UpdateMessageStatus(messageId, userId.Split('_').ElementAt(0), Status.Sent, null);
                        //    //Task.Run(async () => { await UpdateMessageStatus(pushMessageViewModel.Code, userId.Split('_').ElementAt(0), Status.Sent, pushMessageExceptionValue); });
                        //}
                    }

                    if (response.FailureSentIds != null && response.FailureSentIds.Count > 0)
                    {
                        _logger.LogInformation("push message is failed");

                        response.FailureSentIds
                           .Where(userId => message.MessageSource != MessageSource.AdminMessage || (message.MessageSource == MessageSource.AdminMessage && _configurationManager.GetTrackAdminNotifications()))
                           .ForEach(userId =>
                           {
                               var exceptionMessageKeyValue = response.FailedUsersIdsExceptionsMessages?.FirstOrDefault(u => u.Key == userId);
                               UpdateMessageStatus(messageId, userId.Split('_').ElementAt(0), Status.Failed, exceptionMessageKeyValue?.Value);
                               //Task.Run(async () => { await UpdateMessageStatus(pushMessageViewModel.Code, userId.Split('_').ElementAt(0), Status.Failed, pushMessageExceptionValue); });    
                           });

                        //foreach (var userId in response.FailureSentIds)
                        //{
                        //    var exceptionMessageKeyValue = response.FailedUsersIdsExceptionsMessages?.FirstOrDefault(u => u.Key == userId);
                        //    UpdateMessageStatus(messageId, userId.Split('_').ElementAt(0), Status.Failed, exceptionMessageKeyValue?.Value);
                        //    //Task.Run(async () => { await UpdateMessageStatus(pushMessageViewModel.Code, userId.Split('_').ElementAt(0), Status.Failed, pushMessageExceptionValue); });
                        //}
                    }
                }
            }

            return new Response<SystemEventNotificationDto>() { ErrorCode = (int)ErrorCodes.Success };
        }
        public void PrepareMessageTitleAndBody(SendNotificationRequestDto message, MessageTypeDTO messageType, out string messageContentEn, out string messageTitleEn, out string messageContentAr, out string messageTitleAr, out string messageContentSw, out string messageTitleSw)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting message body from repository");

            MessageTemplateDTO messageTemplate = GetMessageTemplateByTemplateId(messageType.MessageTemplate.Id);
            var templateName = messageTemplate.Name;

            #region Language En
            var templateTitleEn = messageTemplate.TitleEn;
            var templateContentEn = messageTemplate.ContentEn;
            messageTitleEn = templateTitleEn;
            messageContentEn = templateContentEn;

            #region Message Body
            if (!string.IsNullOrEmpty(templateContentEn) && message.DisplayParams != null && message.DisplayParams.Any())
            {
                messageContentEn = StringUtility.TextToLtr(string.Format(templateContentEn, message.DisplayParams.ToArray())).ToString();
            }
            #endregion

            #region Message Title
            //prepare message title
            if (!string.IsNullOrEmpty(templateTitleEn) && message.TitleParams != null && message.TitleParams.Any())
            {
                messageTitleEn = StringUtility.TextToLtr(string.Format(templateTitleEn, message.TitleParams.ToArray())).ToString();
            }
            #endregion

            #endregion


            #region Language Ar

            var templateTitleAr = messageTemplate.TitleAr;
            var templateContentAr = messageTemplate.ContentAr;
            messageTitleAr = templateTitleAr;
            messageContentAr = templateContentAr;

            //prepare message body
            if (!string.IsNullOrEmpty(templateContentAr) && message.DisplayParamsAr != null && message.DisplayParamsAr.Any())
            {
                messageContentAr = StringUtility.TextToRtl(string.Format(templateContentAr, message.DisplayParamsAr.ToArray())).ToString();
            }
            if (string.IsNullOrEmpty(messageContentAr))
            {
                messageContentAr = messageContentEn;
            }
            //prepare message title
            if (!string.IsNullOrEmpty(templateTitleAr) && message.TitleParamsAr != null && message.TitleParamsAr.Any())
            {
                messageTitleAr = StringUtility.TextToRtl(string.Format(templateTitleAr, message.TitleParamsAr.ToArray())).ToString();
            }

            if (string.IsNullOrEmpty(messageTitleAr))
            {
                messageTitleAr = messageTitleEn;
            }
            #endregion




            #region Language Sw
            var templateTitleSw = messageTemplate.TitleSw;
            var templateContentSw = messageTemplate.ContentSw;
            messageTitleSw = templateTitleSw;
            messageContentSw = templateContentSw;

            #region Message Body
            if (!string.IsNullOrEmpty(templateContentSw) && message.DisplayParamsSw != null && message.DisplayParamsSw.Any())
            {
                messageContentSw = StringUtility.TextToLtr(string.Format(templateContentSw, message.DisplayParamsSw.ToArray())).ToString();
            }

            if (string.IsNullOrEmpty(messageContentSw))
            {
                messageContentSw = messageContentEn;
            }
            #endregion

            #region Message Title
            //prepare message title
            if (!string.IsNullOrEmpty(templateTitleSw) && message.TitleParamsSw != null && message.TitleParamsSw.Any())
            {
                messageTitleSw = StringUtility.TextToLtr(string.Format(templateTitleSw, message.TitleParamsSw.ToArray())).ToString();
            }

            if (string.IsNullOrEmpty(messageTitleSw))
            {
                messageTitleSw = messageTitleEn;
            }
            #endregion
            #endregion

        }
        private int SaveMessage(SendNotificationRequestDto message, string messageTitleAr, string messageTitleEn, string messageTitleSw, string messageContentAr, string messageContentEn, string messageContentSw, int messageTypeId)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Saving message in database.");

            //if the message is sent through Event or a Queue then the Author of the Message is the System
            string messageSender = message.MessageSource == MessageSource.SystemEventMessage ? "System" : _identityService.UserName;

            var messageId = AddNewMessage(messageTitleAr, messageTitleEn, messageTitleSw, messageTypeId,
                messageContentEn, messageContentAr, messageContentSw, message.MessageSource, messageSender, message.Notes);

            message.ExtraParams?.ForEach(item => AddMessageExtraParams(messageId, item.Key, item.Value, messageSender));

            //Saveing message destinations
            if (message.MessageSource != MessageSource.AdminMessage || (message.MessageSource == MessageSource.AdminMessage && _configurationManager.GetTrackAdminNotifications()))
            {
                message.Destinations.Where(m => m.DestinationType == DestinationType.User).SelectMany(d => d.Recipients).ForEach(r =>
                {
                    AddMessageDestination(messageId, DestinationType.User, r);
                    var statusId = AddMessageStatus(Status.Pending, null, null, null, null, null, null, null, r, null, messageId, messageSender);
                });
                message.Destinations.Where(m => m.DestinationType == DestinationType.All).ForEach(r =>
                {
                    AddMessageDestination(messageId, DestinationType.All, "ALL");
                    _userManager.GetAllUsers().ForEach(user =>
                    {
                        AddMessageStatus(Status.Pending, null, null, null, null, null, null, null, user.ApplicationUserId, null, messageId, messageSender);
                    });
                });
                //if (message.Destinations != null && message.Destinations.Count > 0)
                //{
                //    foreach (var messageDestination in message.Destinations)
                //    {
                //        if (messageDestination.DestinationType == DestinationType.Group)
                //        {
                //            foreach (var destination in messageDestination.Recipients)
                //                AddMessageDestination(messageId, DestinationType.Group, destination);
                //            //TODO: Load users Group and add message status 
                //        }
                //    }
                //}
            }
            return messageId;
        }
        private PushNotificationDto PrepareMessageForPush(SendNotificationRequestDto message, int messageId, string messageContentEn, string messageTitleEn, string messageContentAr, string messageTitleAr, string messageContentSw, string messageTitleSw)
        {
            MessageTypeDTO messageType = GetMessageTypeByCode(message.MessageTypeCode);

            //Preparing Push Message view model 
            PushNotificationDto pushMessageViewModel = new PushNotificationDto();
            foreach (var messageDestination in message.Destinations)
            {
                if (messageDestination.DestinationType == DestinationType.User)
                {
                    foreach (var recipient in messageDestination.Recipients)
                    {
                        PushMessageFinalDestination pushMessageDestination = new PushMessageFinalDestination();
                        var user = _userManager.GetUserByuserId(recipient);
                        if (user == null)
                        {
                            //throw new InvalidParameterException("user_id", recipient);
                            _logger.LogError($"user {recipient} is not found");
                            continue;
                        }
                        pushMessageDestination.Platform = user.Platform;
                        pushMessageDestination.DeviceId = user.DeviceId;
                        pushMessageDestination.UserId = recipient;
                        pushMessageDestination.PushToken = user.PushNotificationToken;
                        pushMessageDestination.Language = user.LanguageCode;
                        if (pushMessageViewModel.Destinations.Any(d => d.UserId == recipient))
                            continue;
                        else
                        {
                            pushMessageViewModel.Destinations.Add(pushMessageDestination);
                        }
                    }
                }
                else if (messageDestination.DestinationType == DestinationType.Group)
                {
                    //TODO:
                    //Load users in each group from identity 
                    //add loaded users to push message destinations
                }
                else if (messageDestination.DestinationType == DestinationType.All)
                {
                    var allUsers = _userManager.GetAllUsers();
                    if (allUsers != null && allUsers.Count > 0)
                    {
                        foreach (var user in allUsers)
                        {
                            PushMessageFinalDestination pushMessageDestination = new PushMessageFinalDestination();
                            pushMessageDestination.Platform = user.Platform;
                            pushMessageDestination.DeviceId = user.DeviceId;
                            pushMessageDestination.UserId = user.ApplicationUserId;
                            pushMessageDestination.PushToken = user.PushNotificationToken;
                            pushMessageDestination.Language = user.LanguageCode;
                            if (pushMessageViewModel.Destinations.Any(d => d.UserId == user.ApplicationUserId))
                                continue;
                            else
                            {
                                pushMessageViewModel.Destinations.Add(pushMessageDestination);
                            }
                        }
                    }
                }
            }

            pushMessageViewModel.ExtraParams = message.ExtraParams;
            pushMessageViewModel.MessageTypeCode = message.MessageTypeCode;
            pushMessageViewModel.Alert = message.Alert;
            pushMessageViewModel.Id = messageId;
            pushMessageViewModel.MessageContentEn = messageContentEn;
            pushMessageViewModel.MessageContentAr = messageContentAr;
            pushMessageViewModel.MessageTitleEn = messageTitleEn;
            pushMessageViewModel.MessageTitleAr = messageTitleAr;
            pushMessageViewModel.MessageTitleSw = messageTitleSw;
            pushMessageViewModel.MessageContentSw = messageContentSw;

            return pushMessageViewModel;
        }
        private async Task<PushMessageResponse> PushMessage(PushNotificationDto message)
        {
            PushMessageResponse mobileResponse = null;

            var mobileMessage = new PushNotificationDto(message);
            var webMessage = new PushNotificationDto(message);

            //removing Web users before sending to mobile push provider
            mobileMessage.Destinations = mobileMessage.Destinations.Where(d => d.Platform != Enums.Platform.WebBrowser).ToList();
            //Mobile Push Notifications
            if (mobileMessage.Destinations.Count > 0)
                mobileResponse = await _mobilePushNotificationProvider.PushMessage(mobileMessage);

            //removing Mobile users before sending to mobile push provider
            webMessage.Destinations = webMessage.Destinations.Where(d => d.Platform == Enums.Platform.WebBrowser).ToList();

            PushMessageResponse webResponse = null;
            if (webMessage.Destinations.Count > 0)
                //Web Push Notifications
                webResponse = await _webBrowserPushNotificationProvider.PushMessage(webMessage);

            // no mobile destination
            if (mobileResponse == null)
                return webResponse;

            // no web destination
            else if (webResponse == null)
                return mobileResponse;

            //aggregate response
            else
            {
                mobileResponse.SuccessfulSentIds.AddRange(webResponse.SuccessfulSentIds);
                mobileResponse.FailureSentIds.AddRange(webResponse.FailureSentIds);
                mobileResponse.FailedUsersIdsExceptionsMessages.AddRange(webResponse.FailedUsersIdsExceptionsMessages);
                mobileResponse.IsAllSucceeded = (mobileResponse.IsAllSucceeded && webResponse.IsAllSucceeded);
                return mobileResponse;
            }
        }
        #endregion

        #region Admin Methods
        public PagedList<MessageDTO> GetAdminMessages(PaginationVm pagination, SortingViewModel sorting)
        {
            PagedList<MessageDTO> messagesDTOs = new PagedList<MessageDTO>() { List = new List<MessageDTO>() };
            var messages = _messageRepository.GetAdminMessages(pagination, sorting);

            if (messages != null && messages.List.Count > 0)
            {
                foreach (var message in messages.List)
                {
                    messagesDTOs.List.Add(new MessageDTO(message));
                }
                messagesDTOs.TotalCount = messages.TotalCount;
                messagesDTOs.HasMore = messages.HasMore;
            }
            return messagesDTOs;
        }
        public List<AdminConversationResponseModel> GetAdminConversationsByCriteria(GetAdminConversationsCriteriaViewModel criteria)
        {
            List<AdminConversationResponseModel> adminConversations = new List<AdminConversationResponseModel>();
            var conversations = _messageRepository.GetAdminConversationsByCriteria(criteria);
            adminConversations = conversations.Select(m => new AdminConversationResponseModel
            {
                //PeerId = m.PeerId,
                Messages = m.Messages
            }).ToList();

            return conversations;
        }
        public List<AdminConversationMessageDetailsViewModel> GetAdminConversationDetailsByCriteria(GetAdminConversationDetailsCriteriaViewModel criteria)
        {
            var messages = _messageRepository.GetAdminConversationDetailsByCriteria(criteria);
            var conversations = messages.Select(m => new AdminConversationMessageDetailsViewModel
            {
                Body = m.ContentEn,
                SenderId = m.SenderId,
                Status = m.Statuses.FirstOrDefault().Status,
                SentDate = m.Statuses.Select(a => a.SentDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault(),
                DeliveredDate = m.Statuses.Select(a => a.DeliveredDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault()
            }).ToList();

            return conversations;
        }
        #endregion

        public List<AdminMessageStatusResponseViewModel> GetMessageDetails(string messageCode)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Getting message details from repository");

            List<AdminMessageStatusResponseViewModel> messagesDetails = new List<AdminMessageStatusResponseViewModel>();
            var messageStatues = _messageRepository.GetMessageDetails(messageCode);
            if (messageStatues != null && messageStatues.Count > 0)
            {
                messagesDetails = messageStatues.Select(m => new AdminMessageStatusResponseViewModel
                {
                    ToUserId = m.User.ApplicationUserId,
                    Status = m.Status,
                    SentDate = m.SentDate?.ToString("yyyyMMddHHmmss"),
                    DeliveredDate = m.DeliveredDate?.ToString("yyyyMMddHHmmss"),
                    Platform = (m.User != null) ? m.User.DevicePlatform : 0
                })
                .ToList();
            }
            return messagesDetails;
        }
        public List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting users from repository");

            List<AdminUserResponseViewModel> usersVm = _userManager.GetUsers(criteria);

            return usersVm;
        }
        public void UpdateMessageStatus(string messageCode, string userId, Status status, string errorReason)
        {
            var messageStatus = _messageStatusRepository.GetMessageStatus(messageCode, userId);
            if (messageStatus != null)
            {
                _logger.LogInformation($"update message ({messageCode}) status to {status.ToString()}");
                UpdateMessageStatuses(new List<MessageStatus> { messageStatus }, status, errorReason);
            }
            else
            {
                var msg = GetMessageByCode(messageCode);
                _logger.LogInformation($"Add status {status.ToString()} to message ({messageCode})");
                var msgStatus = AddMessageStatus(status, DateTime.UtcNow, null, null, null, null, null, null, userId, null, msg.MessageId, _identityService.DisplayName);
            }
        }
        public void UpdateMessageStatus(int messageId, string userId, Status status, string errorReason)
        {
            var messageStatus = _messageStatusRepository.GetMessageStatus(messageId, userId);
            if (messageStatus != null)
            {
                _logger.LogInformation($"update message ({messageId}) status to {status.ToString()}");
                UpdateMessageStatuses(new List<MessageStatus> { messageStatus }, status, errorReason);
            }
            else
            {
                _logger.LogInformation($"Add status {status.ToString()} to message ({messageId})");
                AddMessageStatus(status, DateTime.UtcNow, null, null, null, null, null, null, userId, null, messageId, _identityService.DisplayName);
            }
        }
        private bool UpdateMessageStatuses(List<MessageStatus> messageStatuses, Status status, string errorReason)
        {
            var messageStatusUpdated = false;
            messageStatuses.ForEach(messageStatus =>
            {
                switch (status)
                {
                    case Status.Pending:
                        messageStatus.Status = status;
                        messageStatusUpdated = true;
                        break;
                    case Status.Sent:
                        if (messageStatus.Status == Status.Pending || messageStatus.Status == Status.Failed)
                        {
                            messageStatus.SentDate = DateTime.UtcNow;
                            messageStatus.Status = status;
                            messageStatusUpdated = true;
                        }
                        else if (messageStatus.Status == Status.Read || messageStatus.Status == Status.Delivered)
                        {
                            break;
                        }
                        else
                            throw new ErrorUpdateMessageStatusException(messageStatus.Status, Status.Sent);

                        break;
                    case Status.Delivered:
                        if (messageStatus.Status == Status.Sent || messageStatus.Status == Status.Failed || messageStatus.Status == Status.Pending)
                        {
                            messageStatus.DeliveredDate = DateTime.UtcNow;
                            messageStatus.Status = status;
                            messageStatusUpdated = true;
                            if (messageStatus.SentDate == null)
                            {
                                messageStatus.SentDate = messageStatus.FailedDate;
                                messageStatus.FailedDate = null;
                            }
                        }
                        else if (messageStatus.Status == Status.Delivered || messageStatus.Status == Status.Read)
                        {
                            break;
                        }
                        else
                            throw new ErrorUpdateMessageStatusException(messageStatus.Status, Status.Delivered);

                        break;
                    case Status.Read:
                        if (messageStatus.Status == Status.Delivered || messageStatus.Status == Status.Sent || messageStatus.Status == Status.Pending || messageStatus.Status == Status.Failed)
                        {
                            messageStatus.ReadDate = DateTime.UtcNow;
                            messageStatus.Status = status;
                            messageStatusUpdated = true;
                            if (messageStatus.SentDate == null)
                            {
                                messageStatus.SentDate = messageStatus.FailedDate;
                                messageStatus.FailedDate = null;
                            }
                        }
                        else if (messageStatus.Status == Status.Read)
                        {
                            break;
                        }
                        else
                            throw new ErrorUpdateMessageStatusException(messageStatus.Status, Status.Read);

                        break;

                    case Status.Failed:
                        if (messageStatus.Status == Status.Pending)
                        {
                            messageStatus.FailedDate = DateTime.UtcNow;
                            messageStatus.Status = status;
                            messageStatus.ErrorReason = errorReason;
                            messageStatusUpdated = true;
                        }
                        else if (messageStatus.Status == Status.Read || messageStatus.Status == Status.Delivered)
                        {
                            break;
                        }
                        else
                            throw new ErrorUpdateMessageStatusException(messageStatus.Status, Status.Failed);

                        break;
                }
                if (messageStatusUpdated)
                {
                    if (messageStatus.Status != Status.Failed)
                    {
                        messageStatus.FailedDate = null;
                        messageStatus.ErrorReason = null;
                    }

                    _messageStatusRepository.Update(messageStatus);
                }
            });
            if (messageStatusUpdated)
                _messageStatusRepository.SaveEntities();
            return messageStatusUpdated;
        }
        //public GetNotificationsResponse GetMessagesByCriteriaWithCount(GetNotificationsCriteriaViewModel criteria, string language)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

        //    _logger.LogInformation($"{_typeName}.{methodName}: Getting user notifications from repository");

        //    return _messageRepository.GetNotificationsByCriteriaWithCount(criteria, language);
        //}
        public int GetEndUserUnreadMessagesCount()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting user notifications unread count from repository");
            return _messageRepository.GetEndUserUnreadMessagesCount(_identityService.UserId);
        }
        public async Task<Response<bool>> RegisterForPush(string userId, string platformHeader, string pushToken, string deviceId, string languageCode = "en")
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Saving user " + userId);
            // create new user or update if exist 
            await _userManager.CreateOrUpdateUser(userId, platformHeader, pushToken, deviceId, languageCode);

            var platform = GetPlatform(platformHeader);
            // if device is mobile :
            if (platform != Enums.Platform.WebBrowser && !string.IsNullOrEmpty(deviceId) && !string.IsNullOrEmpty(pushToken))
            {
                //register at provider if needed 
                // web doesn`t need to register at firebase
                var deviceInstallation = new DeviceInstallationViewModel()
                {
                    DeviceId = deviceId,
                    Language = languageCode,
                    Platform = platformHeader,
                    PushNotificationToken = pushToken,
                    UserId = userId

                    //TODO: Add Tages 
                };
                await _mobilePushNotificationProvider.CreateOrUpdatePushNotificationRegistration(deviceInstallation);
            }

            return new Response<bool>() { Data = true };
        }

        private Enums.Platform GetPlatform(string platformHeader)
        {
            //Maping platforms 
            Enums.Platform platform =
                !string.IsNullOrEmpty(platformHeader) ?
                platformHeader switch
                {
                    "android" => Enums.Platform.Android,
                    "ios" => Enums.Platform.Ios,
                    "web" => Enums.Platform.WebBrowser,
                    _ => throw new InvalidParameterException("Platform", platformHeader),
                }
                : throw new MissingParameterException("platform");
            return platform;
        }

        public async Task<BaseResponse> UnRegisterFromPush(string userId)
        {
            //loading user 
            var user = _userManager.GetUserByuserId(userId);
            if (user != null)
            {

                //unregister from user token in  core.messaging db 
                _userManager.UnRegisterUserFromPushToken(userId);

                var deviceInstallation = new DeviceInstallationViewModel()
                {
                    UserId = userId,
                    DeviceId = user.DeviceId,
                    Language = user.LanguageCode,
                    Platform = user.Platform.ToString(),
                    PushNotificationToken = user.PushNotificationToken
                };

                // if device is mobile :
                if (user.Platform != Enums.Platform.WebBrowser)
                    await _mobilePushNotificationProvider.CreateOrUpdatePushNotificationRegistration(deviceInstallation);
                // web doesn`t need to regestir at firebase
            }
            else
            {
                _logger.LogInformation($"User :{userId} not found");
            }
            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }

        public MessageTypeDTO GetMessageTypeByCode(string typeCode)
        {
            var messageType = GetMessageTypes().Where(t => t.Code == typeCode).FirstOrDefault();
            return new MessageTypeDTO(messageType);
        }
        public void AddMessageExtraParams(int messageId, string key, string value, string createdBy)
        {
            var message = _messageRepository.GetMessageByMessageId(messageId);
            var code = message.AddMessageExtraParams(key, value, createdBy);
            _messageRepository.SaveEntities();
        }
        public int AddNewMessage(string messageTitleAr, string messageTitleEn, string messageTitleSw, int messageTypeId, string messageContentEn, string messageContentAr, string messageContentSw, MessageSource messageSource, string createdBy, string notes, bool isDeleted = false)
        {
            var messageType = _messageTypeRepository.GetMessageTypeById(messageTypeId);
            if (messageType == null)
                throw new MessageTypeNotFoundException(messageTypeId);

            Message msg = new Message(messageTitleAr, messageTitleEn, messageTitleSw, messageType, messageContentEn, messageContentAr, messageContentSw, messageSource, createdBy, notes);
            _messageRepository.Add(msg);
            _messageRepository.SaveEntities();

            return msg.Id;
        }
        public int AddMessageStatus(Status name, DateTime? sentDate, DateTime? failedDate, DateTime? deliveredDate, DateTime? readDate, int? devicePlatformId, string errorCode, string errorReason, string userId, int? userDeviceId, int messageId, string createdBy)
        {
            var user = _userRepository.GetUserByUserRegisterId(userId);

            var message = _messageRepository.GetMessageByMessageId(messageId);

            var messagestatus = new MessageStatus(Status.Pending, null, null, null, null, null, null, null, user, null, message, createdBy);

            _messageStatusRepository.Add(messagestatus);
            _messageStatusRepository.SaveEntities();

            return messagestatus.Id;
        }
        public MessageDTO GetMessageByMessageId(int messageId)
        {
            var message = _messageRepository.GetMessageByMessageId(messageId);

            return new MessageDTO(message);
        }

        public MessageStatusDTO GetMessageStatus(string messageCode, string userId)
        {
            var messageStatus = _messageStatusRepository.GetMessageStatus(messageCode, userId);

            return new MessageStatusDTO(messageStatus);
        }

        public MessageDTO GetMessageByCode(string messageCode)
        {
            return new MessageDTO(_messageRepository.GetMessageByCode(messageCode));
        }

        public MessageTemplateDTO GetMessageTemplateByTemplateId(int templateId)
        {
            var messageTemplate = GetMessageTemplates().FirstOrDefault(t => t.Id == templateId);
            return new MessageTemplateDTO(messageTemplate);
        }

        public PagedList<EndUserNotificationResponseModel> GetMessagesForEndUser(GetNotificationsCriteriaViewModel criteria, Language language)
        {
            var messagesDb = _messageRepository.GetEndUserMessagesByCriteria(criteria);

            PaginationVm pagination = (criteria.Pagination == null || criteria.Pagination.PageNumber == null || criteria.Pagination.PageSize == null || criteria.Pagination.PageNumber == 0 || criteria.Pagination.PageSize == 0) ? null : new PaginationVm { PageNumber = criteria.Pagination.PageNumber.Value, PageSize = criteria.Pagination.PageSize.Value };


            PagedList<EndUserNotificationResponseModel> messagesVm = new PagedList<EndUserNotificationResponseModel>(messagesDb.TotalCount, pagination);

            messagesVm.List = new List<EndUserNotificationResponseModel>();

            List<int> sentMessagesIdsToBeUpdated = new List<int>();

            if (messagesDb is { TotalCount: > 0 })
            {
                foreach (var messageDb in messagesDb.List)
                {
                    EndUserNotificationResponseModel messageVm = new EndUserNotificationResponseModel
                    {
                        Title = LocalizationUtility.GetLocalizedText(messageDb.TitleEn, messageDb.TitleAr, language, messageDb.TitleSw),
                        MessageTypeCode = messageDb.Type.Code,
                        Body = LocalizationUtility.GetLocalizedText(messageDb.ContentEn, messageDb.ContentAr, language, messageDb.ContentSw),
                        Code = messageDb.Code,
                        Id = messageDb.Id
                    };
                    MessageStatus messageStatusDb = messageDb.Statuses.FirstOrDefault(u => u.UserApplicationUserId == _identityService.UserId);

                    if (messageStatusDb == null)
                        throw new InternalServerErrorException($"No message status found for user {_identityService.UserId} and message {messageDb.Id}");

                    messageVm.SentDate = messageStatusDb.CreationDate.ToString("yyyyMMddHHmmss");
                    messageVm.ExtraParams = messageDb.MessageExtraParams.GroupBy(x => x.Key).ToDictionary(g => g.Key, g => g.Select(x => x.Value).FirstOrDefault());


                    messagesVm.List.Add(messageVm);
                }
            }

            UpdateMessagesStatus(sentMessagesIdsToBeUpdated, _identityService.UserId, Status.Sent, null);

            return messagesVm;
        }
        public async Task UpdateMessagesStatus(List<int> messageIds, string userId, Status status, string errorReason)
        {
            foreach (var messageId in messageIds)
            {
                UpdateMessageStatus(messageId, userId, status, errorReason);
            }
        }

        public List<EndUserConversationMessageDetailsResponseModel> GetEndUserConversationDetailsByCriteria(GetEndUserConversationDetailsCriteriaViewModel criteria, string userId)
        {
            List<EndUserConversationMessageDetailsResponseModel> endUserConversations = new List<EndUserConversationMessageDetailsResponseModel>();
            var messages = _messageRepository.GetEndUserConversationDetailsByCriteria(criteria, userId);
            if (messages is { Count: > 0 })
            {
                foreach (var message in messages)
                {
                    var userMessageStatus = message.Statuses.FirstOrDefault(u => u.User.ApplicationUserId == _identityService.UserId);
                    endUserConversations.Add(new EndUserConversationMessageDetailsResponseModel()
                    {
                        //Title = message.TitleEn,
                        Body = message.ContentEn,
                        SenderId = message.SenderId,
                        Status = userMessageStatus.Status,
                        SentDate = userMessageStatus.SentDate != null ? ((DateTime)userMessageStatus.SentDate).ToString("yyyyMMddHHmmss") : "",
                        DeliveredDate = userMessageStatus.DeliveredDate != null ? ((DateTime)userMessageStatus.DeliveredDate).ToString("yyyyMMddHHmmss") : ""
                    });
                }
            }
            return endUserConversations;
        }

        public List<EndUserConversationsResponseModel> GetEndUserConversationsByCriteria(GetEndUserConversationsCriteriaViewModel criteria, string userId)
        {
            return _messageRepository.GetEndUserConversationsByCriteria(criteria, userId);
        }

        public void AddMessageDestination(int messageId, DestinationType destinationType, string destination)
        {
            var message = _messageRepository.GetMessageByMessageId(messageId);

            _messageDestinationRepository.Add(new MessageDestination(message, destinationType, message.CreatedBy, destination));
            _messageDestinationRepository.SaveEntities();
        }


        public List<MessageType> GetMessageTypes()
        {
            List<MessageType> messageTypes = null;
            // try to get message types from memory with key "message_types"
            messageTypes = (List<MessageType>)_cacheService.GetItemFromCache<List<MessageType>>(MessageTypesKey);

            if (messageTypes != null && messageTypes.Count > 0)
                return messageTypes;
            else
            {
                messageTypes = _messageTypeRepository.GetMessageTypes();
                // store message types from DB into memory with  key "message_types"
                _cacheService.SaveItemToCache(MessageTypesKey, messageTypes);
                return messageTypes;
            }
        }
        public List<MessageTemplate> GetMessageTemplates()
        {
            List<MessageTemplate> messageTemplates = null;
            // try to get message types from memory with key "message_templates"
            messageTemplates = (List<MessageTemplate>)_cacheService.GetItemFromCache<List<MessageTemplate>>(MessageTemplatesKey);

            if (messageTemplates != null)
                return messageTemplates;
            else
            {
                messageTemplates = _messageTemplateRepository.GetAll().ToList();
                // store message types from DB into memory with  key "message_templates"
                _cacheService.SaveItemToCache(MessageTemplatesKey, messageTemplates);
                return messageTemplates;
            }
        }

        public async void UpdateMessagesStatusToRead(string userId)
        {
            var messageStatuses = _messageStatusRepository.GetMessagesStatusToMarkAsRead(userId);
            UpdateMessageStatuses(messageStatuses, Status.Read, null);
        }

        public void DeleteNotification(int notificationId, string userId)
        {
            var messageStatus = _messageStatusRepository.GetMessageStatus(notificationId, userId);
            if (messageStatus != null && !messageStatus.IsDeleted)
            {
                messageStatus.IsDeleted = true;
                _messageStatusRepository.Update(messageStatus);
                _messageStatusRepository.SaveEntities();
            }
        }
        #endregion
    }
}
