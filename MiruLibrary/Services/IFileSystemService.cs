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