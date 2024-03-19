using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using System.Text;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders.Firebase;

namespace Emeint.Core.BE.Notifications.Services.PushNotificationProviders.FireBase
{
    public class FirebaseWebBrowserNotificationProvider : FirebaseBasePushProvider, IWebBrowserPushNotificationProvider
    {
        #region Fields 
        private readonly ILogger<FirebaseWebBrowserNotificationProvider> _logger;
        private readonly string _typeName;
        private readonly IConfigurationManager _configurationManager;
        #endregion

        #region CTOR
        public FirebaseWebBrowserNotificationProvider(ILogger<FirebaseWebBrowserNotificationProvider> logger,
            IConfigurationManager configurationManager)
        {
            _logger = logger;
            _typeName = GetType().Name;
            _configurationManager = configurationManager;

            //Initialize firebase app with firebase app credentials
            var webCredentialsPath = (_configurationManager.GetFirebaseWebCredentialsFilePath());

            if (FirebaseApp.DefaultInstance == null)
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(webCredentialsPath)
                });
        }

        #endregion

        #region Methods 
        public async Task<PushMessageResponse> PushMessage(PushNotificationDto pushNotificationDto)
        {
            return await base.PushMessage(pushNotificationDto);
        }
        #endregion
    }

}
