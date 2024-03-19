using Emeint.Core.BE.Configurations.Application.ViewModels;
using Emeint.Core.BE.Configurations.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Application.Mappers
{
    public class SettingMapper
    {
        public static BaseSettingViewModel MapFromSetting(Setting setting)
        {
            return new BaseSettingViewModel
            {
                Group = setting.Group,
                SettingType = setting.SettingType,
                Key = setting.Key,
                Value = setting.Value,
                IsRequired = setting.IsRequired,
            };
        }
        public static AdminSettingViewModel MapFromAdminSetting(Setting setting)
        {
            var vm = new AdminSettingViewModel
            {
                Group = setting.Group,
                Key = setting.Key,
                User = setting.User,
                Value = setting.Value,
                IsRequired = setting.IsRequired,
                SettingType = setting.SettingType,
                DisplayName = setting.DisplayName,
                Description = setting.Description,
                UnitOfMeasure = setting.UnitOfMeasure
            };
            if (setting.SettingType == Domain.Enums.SettingType.Enum && !string.IsNullOrEmpty(setting.EnumTypeName))
            {
                vm.EnumTypeDetails = new List<EnumType>();
                var enumType = GetEnumType(setting.EnumTypeName);
                if (enumType != null)
                {
                    var enumNames = Enum.GetNames(enumType).ToList();
                    var enumValues = Enum.GetValues(enumType);
                    for (int i = 0; i < enumValues.Length; i++)
                    {
                        vm.EnumTypeDetails.Add(new EnumType { Name = enumNames[i], Value = (int)enumValues.GetValue(i) });
                    }
                }
            }
            return vm;
        }
        public static Type GetEnumType(string enumName)
        {
            return Type.GetType(enumName);
        }
        public static List<BaseSettingViewModel> MapFromSettingList(List<Setting> settings)
        {
            List<BaseSettingViewModel> settingsViewModel = new List<BaseSettingViewModel>();
            if (settings != null && settings.Count > 0)
            {
                foreach (var setting in settings)
                {
                    settingsViewModel.Add(
                        MapFromSetting(setting)
                    );
                }
            }
            return settingsViewModel;
        }
        public static List<AdminSettingViewModel> MapFromAdminSettingList(List<Setting> settings)
        {
            List<AdminSettingViewModel> settingsViewModel = new List<AdminSettingViewModel>();
            if (settings != null && settings.Count > 0)
            {
                foreach (var setting in settings)
                {
                    settingsViewModel.Add(
                        MapFromAdminSetting(setting)
                    );
                }
            }
            return settingsViewModel;
        }
        public static async Task<BaseSettingViewModel> MapFromSettingAsync(Setting setting)
        {
            return await Task.Run(() => MapFromSetting(setting));
        }
        public static async Task<AdminSettingViewModel> MapFromAdminSettingAsync(Setting setting)
        {
            return await Task.Run(() => MapFromAdminSetting(setting));
        }
        public static async Task<List<BaseSettingViewModel>> MapFromSettingListAsync(List<Setting> settings)
        {
            return await Task.Run(() => MapFromSettingList(settings));
        }
        public static async Task<List<AdminSettingViewModel>> MapFromAdminSettingListAsync(List<Setting> settings)
        {
            return await Task.Run(() => MapFromAdminSettingList(settings));
        }
    }
}
