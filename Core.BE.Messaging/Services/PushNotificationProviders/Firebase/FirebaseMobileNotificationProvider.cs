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
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Notifications.Services.PushNotificationProviders.FireBase
{
    public class FirebaseMobileNotificationProvider : FirebaseBasePushProvider, IMobilePushNotificationProvider
    {
        #region Fields 
        private readonly ILogger<FirebaseMobileNotificationProvider> _logger;
        private readonly string _typeName;

        #endregion

        #region CTOR
        public FirebaseMobileNotificationProvider(ILogger<FirebaseMobileNotificationProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _typeName = GetType().Name;

            //Initialize firebase app with firebase app credentials
            if (FirebaseApp.DefaultInstance == null)
            {
                //var mobileCredentialsPath = (_configurationManager.GetFirebaseMobileCredentialsFilePath());
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(configuration["NotificationsFirebaseCredentials"])
                });
            }
        }

        #endregion

        #region Methods 
        public async Task<bool> CreateOrUpdatePushNotificationRegistration(DeviceInstallationViewModel installation)
        {
            // FireBase doesn`t require to register the user, they can notify him using his token.
            return true;
        }

        public async Task<bool> DeleteRegistration(DeviceInstallationViewModel installation)
        {
            // FireBase doesn`t require to UnRegister the user.
            return true;
        }

        public async Task<PushMessageResponse> PushMessage(PushNotificationDto pushNotificationDto)
        {
            var res = await base.PushMessage(pushNotificationDto);
            return res;
        }

        #endregion

    }
}
