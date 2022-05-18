// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.IO;
using System.IO.Abstractions;
using MiruLibrary.Models;
using MyInternetConnectionLibrary;
using Newtonsoft.Json;

namespace MiruLibrary
{
    public class FileSystemService : IFileSystemService
    {
        public FileSystemService(IFileSystem fileSystem, IWebService webService)
        {
            FileSystem = fileSystem;
            WebService = webService;
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
        public IWebService WebService { get; }

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
            }
            GetSenpaiData();
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
                var deserializedSenpaiData = JsonConvert
                    .DeserializeObject<SenpaiEntryModel>(WebService.Client.GetStringAsync(Constants.SenpaiDataSourceURL).Result);
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
