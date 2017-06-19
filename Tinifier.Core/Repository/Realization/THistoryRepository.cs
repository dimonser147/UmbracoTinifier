using System.Collections.Generic;
using Tinifier.Core.Models;
using Tinifier.Core.Repository.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository
{
    public class THistoryRepository : IEntityReader<TinyPNGResponseHistory>, IEntityCreator<TinyPNGResponseHistory>
    {
        private UmbracoDatabase _database;

        public THistoryRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public IEnumerable<TinyPNGResponseHistory> GetAll()
        {
            var select = new Sql("SELECT * FROM TinifierResponseHistory");
            var histories = _database.Fetch<TinyPNGResponseHistory>(select);

            return histories;
        }

        public TinyPNGResponseHistory GetByKey(int Id)
        {
            var query = new Sql($"SELECT * FROM TinifierResponseHistory WHERE ImageId = {Id}");
            var history = _database.FirstOrDefault<TinyPNGResponseHistory>(query);

            return history;
        }

        public void Create(TinyPNGResponseHistory newItem)
        {
            _database.Insert(newItem);
        }
    }
}
