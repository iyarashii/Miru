using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiruLibrary;
using MiruLibrary.Models;
using MyInternetConnectionLibrary;
using Newtonsoft.Json;

namespace MiruLibrary
{
    public class FileSystemService : IFileSystemService
    {
        public FileSystemService(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            ImageCacheFolder = FileSystem.DirectoryInfo.FromDirectoryName(Constants.ImageCacheFolderPath);

            if (!ImageCacheFolder.Exists)
            {
                ImageCacheFolder.Create();
            }
            // if there is no local senpai data file get the JSON from senpai.moe
            GetSenpaiData();
        }
        public IFileSystem FileSystem { get; }
        public IDirectoryInfo ImageCacheFolder { get; }

        public void ClearImageCache()
        {
            foreach (IFileInfo file in ImageCacheFolder.EnumerateFiles())
            {
                file.Delete();
            }
        }

        // updates senpai.json data file which is used as a backup for anime airing time
        public void UpdateSenpaiData()
        {
            if (FileSystem.File.Exists(Constants.SenpaiFilePath))
            {
                FileSystem.File.Delete(Constants.SenpaiFilePath);
                GetSenpaiData();
            }
            else
            {
                GetSenpaiData();
            }
        }

        // gets the data from senpai.moe in a JSON form
        public void GetSenpaiData()
        {
            if (FileSystem.File.Exists(Constants.SenpaiFilePath))
            {
                return;
            }

            // serialize JSON directly to a file
            using (StreamWriter file = FileSystem.File.CreateText(Constants.SenpaiFilePath))
            {
                // get only MALID and airing_date json properties
                var deserializedSenpaiData = JsonConvert.DeserializeObject<SenpaiEntryModel>(InternetConnection.client.GetStringAsync(Constants.SenpaiDataSourceURL).Result);
                file.Write(JsonConvert.SerializeObject(deserializedSenpaiData, Formatting.Indented));
            }
        }

        public void DownloadFile(IWebClientWrapper client, string fileLocation, string url)
        {
            if (!FileSystem.File.Exists(fileLocation))
            {
                client.DownloadFile(url, fileLocation);
            }
        }
    }
}
