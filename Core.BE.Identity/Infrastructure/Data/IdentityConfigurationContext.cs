using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Data
{
    public class IdentityConfigurationContext : ConfigurationContext
    {
        public IdentityConfigurationContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService) : base(options, configuration, identityService) { }


        public override void Seed()
        {
            base.Seed();

            //if (!Settings.Any(s => s.Key == "AccountVerification:PhoneVerification"))
            //    Settings.Add(
            //    new Setting()
            //    {
            //        Key = "AccountVerification:PhoneVerification",
            //        Value = "1",
            //        Code = "ic-AccountVerification:PhoneVerification",
            //        CreatedBy = "System",
            //        CreationDate = DateTime.UtcNow,
            //        DisplayName = "Phone Number Verification",
            //        Group = "Account Verification",
            //        Description = "Specifies the mode of phone number verfication",
            //        EnumTypeName = $"{typeof(PhoneNumberVerification).FullName},{typeof(PhoneNumberVerification).Assembly.GetName().Name}",
            //        SettingType = SettingType.Enum,
            //        User = SettingUser.Server,
            //        ShowInPortal = true,
            //        UnitOfMeasure = null,
            //        ModifiedBy = null,
            //        ModificationDate = null,
            //        Minimum = null,
            //        Maximum = null,
            //        Comment = null
            //    });

            if (!Settings.Any(s => s.Key == "AccountVerification:EmailVerification"))
                Settings.Add(
                new Setting()
                {
                    Key = "AccountVerification:EmailVerification",
                    Value = "1",
                    Code = "ic-AccountVerification:EmailVerification",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Email Verification",
                    Group = "Account Verification",
                    Description = "Specifies the mode of email verfication",
                    EnumTypeName = $"{typeof(EmailVerification).FullName},{typeof(EmailVerification).Assembly.GetName().Name}",
                    SettingType = SettingType.Enum,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "DefaultCountry"))
                Settings.Add(
                new Setting()
                {
                    Key = "DefaultCountry",
                    Value = "EGY",
                    Code = "ic-DefaultCountry",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Default Country",
                    Group = "Defaults",
                    Description = "set the default country if the country not specified",
                    EnumTypeName = null,
                    SettingType = SettingType.Text,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "MaxFailedAccessAttempts"))
                Settings.Add(
                new Setting()
                {
                    Key = "MaxFailedAccessAttempts",
                    Value = "3",
                    Code = "ic-MaxFailedAccessAttempts",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Max Failed Access Attempts",
                    Group = "Defaults",
                    Description = "set the allowed number of faild login",
                    EnumTypeName = null,
                    SettingType = SettingType.Integer,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "DefaultLockoutMinutes"))
                Settings.Add(
                new Setting()
                {
                    Key = "DefaultLockoutMinutes",
                    Value = "5",
                    Code = "ic-DefaultLockoutMinutes",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Default Lockout Minutes",
                    Group = "Defaults",
                    Description = "Default Lockout Minutes",
                    EnumTypeName = null,
                    SettingType = SettingType.Integer,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "CreateAdminEmailSubject"))
                Settings.Add(
                new Setting()
                {
                    Key = "CreateAdminEmailSubject",
                    Value = "New Admin Account",
                    Code = "ic-CreateAdminEmailSubject",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Create Admin Email Subject",
                    Group = "Email",
                    Description = "Create Admin Email Subject",
                    EnumTypeName = null,
                    SettingType = SettingType.Text,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "UpdateAdminEmailSubject"))
                Settings.Add(
                new Setting()
                {
                    Key = "UpdateAdminEmailSubject",
                    Value = "Admin Account Update",
                    Code = "ic-UpdateAdminEmailSubject",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Update Admin Email Subject",
                    Group = "Email",
                    Description = "Update Admin Email Subject",
                    EnumTypeName = null,
                    SettingType = SettingType.Text,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });
            if (!Settings.Any(s => s.Key == "PortalDefaultPageSize"))
                Settings.Add(
                new Setting()
                {
                    Key = "PortalDefaultPageSize",
                    Value = "10",
                    Code = "ic-DefaultPageSize",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Portal Default Page Size",
                    Group = "Defaults",
                    Description = "Portal Default Page Size",
                    EnumTypeName = $"{typeof(PortalListsPageSize).FullName},{typeof(PortalListsPageSize).Assembly.GetName().Name}",
                    SettingType = SettingType.Enum,
                    User = SettingUser.Admin,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = 1,
                    Maximum = 1000,
                    Comment = null
                });

            if (!Settings.Any(s => s.Key == "IsConcurrentSessionsAllowed"))
                Settings.Add(
                new Setting()
                {
                    Key = "IsConcurrentSessionsAllowed",
                    Value = "false",
                    Code = "ic-IsConcurrentSessionsAllowed",
                    CreatedBy = "System",
                    CreationDate = DateTime.UtcNow,
                    DisplayName = "Allow concurrent sessions for user",
                    Group = "Account",
                    Description = "Allow concurrent sessions for user",
                    EnumTypeName = null,
                    SettingType = SettingType.Boolean,
                    User = SettingUser.Server,
                    ShowInPortal = true,
                    UnitOfMeasure = null,
                    ModifiedBy = null,
                    ModificationDate = null,
                    Minimum = null,
                    Maximum = null,
                    Comment = null
                });

            SaveChanges();
        }
    }
}
