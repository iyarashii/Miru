// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using MyInternetConnectionLibrary;
using System.IO;
using System.IO.Abstractions;

namespace MiruLibrary
{
    public interface IFileSystemService
    {
        IDirectoryInfo ImageCacheFolder { get; }
        IFileSystem FileSystem { get; }

        void ClearImageCache();
        void DownloadFile(IWebClientWrapper client, string fileLocation, string url);
        void GetSenpaiData();
        void UpdateSenpaiData();
    }
}