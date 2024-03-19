using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Media.Domain.Enums;
using System;

namespace Emeint.Core.BE.Media.Domain.Configurations
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingRepository _settingRepository;
        public ConfigurationManager(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public bool CacheImagesInMemory()
        {
            string cachInMemory = _settingRepository.GetSettingByKey("CacheImagesInMemory");
            var parsedSuccessfully = bool.TryParse(cachInMemory, out bool value);
            if (parsedSuccessfully)
                return value;
            return false;
        }

        public string GetExportFolderPath()
        {
            return _settingRepository.GetSettingByKey("ExportRootFolder");
        }

        public int GetImageMaxSize()
        {
            return Convert.ToInt32(_settingRepository.GetSettingByKey("ImageMaxSize"));
        }

        public string GetImagesRootFolder()
        {
            return _settingRepository.GetSettingByKey("ImagesRootFolder");
        }

        #region Azure Storage Settings
        //public string GetAzureAccountName()
        //{
        //    return _settingRepository.GetSettingByKey("AzureAccountName"); //imagesazurestorage
        //}

        //public string GetAzureImageContainer()
        //{
        //    return _settingRepository.GetSettingByKey("AzureImageContainer"); //myimagescontainer
        //}


        //public string GetAzureAccountKey()
        //{
        //    return _settingRepository.GetSettingByKey("AzureAccountKey"); //6P6Go0JkcFxRFLyJ1bNE3/7Qvg4wgNpko0cZTw3aM1Zw5ZaO1lw6PIM2NRYNDyt3eO6oJSBuyr3zS3m4HVdXRQ==
        //}

        //public string GetAzureStorageBasePath()
        //{
        //    return _settingRepository.GetSettingByKey("AzureStorageBasePath");
        //}

        //public string GetAzureVideoContainer()
        //{
        //    return _settingRepository.GetSettingByKey("AzureVideoContainer"); 

        //}

        //public string GetAzureDocumentContainer()
        //{
        //    return _settingRepository.GetSettingByKey("AzureDocumentContainer");
        //}
        #endregion

        #region Saving Image Locations Settings

        public bool SaveImageBinaryDataToDataBase()
        {
            string saveImageBinaryDataToDataBase = _settingRepository.GetSettingByKey("SaveImageBinaryDataToDataBase");
            if (string.IsNullOrEmpty(saveImageBinaryDataToDataBase))
                saveImageBinaryDataToDataBase = "true";
            return Convert.ToBoolean(saveImageBinaryDataToDataBase);
        }

        public int GetAzureStorageSharedAccessKeyExpiry()
        {
            return Convert.ToInt32( _settingRepository.GetSettingByKey("AzureStorageSharedAccessKeyExpiry"));
        }

        public int GetVideoMaxSize()
        {
            return Convert.ToInt32(_settingRepository.GetSettingByKey("VideosMaxSizeInMegaBytes"));
        }
        #endregion
    }
}
