using Emeint.Core.BE.Media.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Configurations
{
    public interface IConfigurationManager
    {
        string GetExportFolderPath();
        int GetImageMaxSize();
        int GetVideoMaxSize();
        string GetImagesRootFolder();
        bool CacheImagesInMemory();
        bool SaveImageBinaryDataToDataBase();

        #region Azure Storage Setting
        int GetAzureStorageSharedAccessKeyExpiry();

        //string GetAzureStorageBasePath();
        //string GetAzureAccountName();
        //string GetAzureAccountKey();
        //string GetAzureImageContainer();
        //string GetAzureVideoContainer();
        //string GetAzureDocumentContainer();
        #endregion

    }
}
