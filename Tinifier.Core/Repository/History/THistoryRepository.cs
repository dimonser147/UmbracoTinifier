using System.Collections.Generic;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.History
{
    public class THistoryRepository : IEntityReader<TinyPNGResponseHistory>, IEntityCreator<TinyPNGResponseHistory>
    {
        private readonly UmbracoDatabase _database;

        public THistoryRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Get all tinifing histories from database
        /// </summary>
        /// <returns>IEnumerable of TinyPNGResponseHistory</returns>
        public IEnumerable<TinyPNGResponseHistory> GetAll()
        {
            var select = new Sql("SELECT * FROM TinifierResponseHistory");
            return _database.Fetch<TinyPNGResponseHistory>(select);
        }

        /// <summary>
        /// Get history by Id
        /// </summary>
        /// <param name="id">history Id</param>
        /// <returns>TinyPNGResponseHistory</returns>
        public TinyPNGResponseHistory GetByKey(int id)
        {
            var query = new Sql($"SELECT * FROM TinifierResponseHistory WHERE ImageId = {id}");
            return _database.FirstOrDefault<TinyPNGResponseHistory>(query);
        }

        /// <summary>
        /// Create history
        /// </summary>
        /// <param name="newItem">TinyPNGResponseHistory</param>
        public void Create(TinyPNGResponseHistory newItem)
        {
            _database.Insert(newItem);
        }
    }
}
