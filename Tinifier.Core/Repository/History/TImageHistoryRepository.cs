using System.Collections.Generic;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.History
{
    public class TImageHistoryRepository : IEntityReader<TinifierImagesHistory>, IEntityCreator<TinifierImagesHistory>, IEntityRemover<TinifierImagesHistory>
    {
        private readonly UmbracoDatabase _database;

        public TImageHistoryRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public IEnumerable<TinifierImagesHistory> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public TinifierImagesHistory Get(int id)
        {
            var query = new Sql("SELECT * FROM TinifierImageHistory WHERE [ImageId] = @0", id.ToString());
            return _database.FirstOrDefault<TinifierImagesHistory>(query);
        }

        public void Create(TinifierImagesHistory entity)
        {
            _database.Insert(entity);
        }

        public void Delete(int id)
        {
            var query = new Sql("DELETE FROM TinifierImageHistory WHERE [ImageId] = @0", id.ToString());
            _database.Execute(query);
        }
    }
}
