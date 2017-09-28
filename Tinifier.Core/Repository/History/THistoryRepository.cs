using System.Collections.Generic;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.History
{
    public class THistoryRepository : IEntityReader<TinyPNGResponseHistory>, IEntityCreator<TinyPNGResponseHistory>, IEntityRemover<TinyPNGResponseHistory>
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
            var select = new Sql("SELECT * FROM TinifierResponseHistory WHERE OccuredAt >= DATEADD(day,-10,GETDATE()) AND IsOptimized = 'true'");
            return _database.Fetch<TinyPNGResponseHistory>(select);
        }

        /// <summary>
        /// Get history by Id
        /// </summary>
        /// <param name="id">history Id</param>
        /// <returns>TinyPNGResponseHistory</returns>
        public TinyPNGResponseHistory Get(int id)
        {
            var query = new Sql("SELECT * FROM TinifierResponseHistory WHERE ImageId = @0", id);
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

        /// <summary>
        /// Delete history for image
        /// </summary>
        /// <param name="imageId">Image Id</param>
        public void Delete(int imageId)
        {
            var query = new Sql("DELETE FROM TinifierResponseHistory WHERE ImageId = @0", imageId);
            _database.Execute(query);
        }
    }
}
