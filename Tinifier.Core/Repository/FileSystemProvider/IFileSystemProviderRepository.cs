using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Repository.FileSystemProvider
{
    public interface IFileSystemProviderRepository
    {
        TFileSystemProviderSettings GetFileSystem();

        void Delete();

        void Create(string type);
    }
}
