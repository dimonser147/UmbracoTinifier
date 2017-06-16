using System.Collections.Generic;
using Tinifier.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository
{
    public class THistoryRepository : IRepository<TinyPNGResponseHistory>
    {
        private UmbracoDatabase _database;

        public THistoryRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public IEnumerable<TinyPNGResponseHistory> GetAllItems()
        {
            var select = new Sql("SELECT * FROM TinifierResponseHistory");
            var histories = _database.Fetch<TinyPNGResponseHistory>(select);

            return histories;
        }

        public TinyPNGResponseHistory GetItemById(int timageId)
        {
            var query = new Sql($"SELECT * FROM TinifierResponseHistory WHERE ImageId = {timageId}");
            var history = _database.FirstOrDefault<TinyPNGResponseHistory>(query);

            return history;
        }

        public void CreateItem(TinyPNGResponseHistory newItem)
        {
            _database.Insert(newItem);
        }
    }
}
