using System.Collections.Generic;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.State
{
    public class TStateRepository : IRepository<TState>
    {
        private readonly UmbracoDatabase _database;

        public TStateRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Create state
        /// </summary>
        /// <param name="entity">TState</param>
        public void Create(TState entity)
        {
            _database.Insert(entity);
        }

        /// <summary>
        /// Get all states
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TState> GetAll()
        {
            var query = new Sql("SELECT * FROM TinifierState");
            return _database.Fetch<TState>(query);
        }

        /// <summary>
        /// Get state with status
        /// </summary>
        /// <param name="status">status Id</param>
        /// <returns>TState</returns>
        public TState Get(int status)
        {
            var query = new Sql("SELECT * FROM TinifierState WHERE Status = @0", status);
            return _database.FirstOrDefault<TState>(query);
        }

        /// <summary>
        /// Update State
        /// </summary>
        /// <param name="entity">TState</param>
        public void Update(TState entity)
        {
            var query = new Sql("UPDATE TinifierState SET CurrentImage = @0, AmounthOfImages = @1, Status = @2 WHERE Id = @3", entity.CurrentImage, entity.AmounthOfImages, entity.Status, entity.Id);
            _database.Execute(query);
        }

        public void Delete()
        {
            var query = new Sql("UPDATE TinifierState SET Status = @0 WHERE Status = @1", (int) Statuses.Done, (int) Statuses.InProgress);
            _database.Execute(query);
        }
    }
}
