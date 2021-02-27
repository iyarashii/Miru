using System.IO;
using System.IO.Abstractions;

namespace MiruDatabaseLogicLayer
{
    public interface IFileSystemService
    {
        IDirectoryInfo ImageCacheFolder { get; }
        IFileSystem FileSystem { get; }

        void ClearImageCache();
        void GetSenpaiData();
        void UpdateSenpaiData();
    }
}