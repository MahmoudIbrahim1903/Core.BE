using System;
using System.IO;

namespace Emeint.Core.BE.Media.Utilities
{
    public static class DocPathUtility
    {
        public static string CreateFolder(string exportFolderPath)
        {
            //Creating folder on server to save exported file 
            string dailyGeneratedFolder = DateTime.UtcNow.Year + "/" + DateTime.UtcNow.Month + "/" + DateTime.UtcNow.Day + "/";
            string fullPath = exportFolderPath + dailyGeneratedFolder;
            fullPath = fullPath.Replace('/', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(fullPath);
            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException();
            return fullPath;
        }
    }
}
