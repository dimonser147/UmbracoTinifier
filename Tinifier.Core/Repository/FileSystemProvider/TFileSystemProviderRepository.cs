using Tinifier.Core.Models.Db;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.FileSystemProvider
{
    public class TFileSystemProviderRepository : IFileSystemProviderRepository
    {
        private readonly UmbracoDatabase _database;

        public TFileSystemProviderRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public void Create(string type)
        {
            var settings = new TFileSystemProviderSettings
            {
                Type = type
            };

            _database.Insert(settings);
        }

        public TFileSystemProviderSettings GetFileSystem()
        {
            var query = new Sql("SELECT * FROM TinifierFileSystemProviderSettings");
            return _database.FirstOrDefault<TFileSystemProviderSettings>(query);
        }

        public void Delete()
        {
            var query = new Sql("DELETE FROM TinifierFileSystemProviderSettings");
            _database.Execute(query);
        }
    }
}
