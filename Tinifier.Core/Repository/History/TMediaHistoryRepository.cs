using System.Collections.Generic;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.History
{
    public class TMediaHistoryRepository :
        IEntityReader<TinifierMediaHistory>, IEntityCreator<TinifierMediaHistory>, IEntityRemover<TinifierMediaHistory>
    {
        private readonly UmbracoDatabase _database;
        private readonly string _tableName = "TinifierMediaHistories";

        public TMediaHistoryRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Insert new record into the DB
        /// </summary>
        /// <param name="entity">Model to inserting</param>
        public void Create(TinifierMediaHistory entity)
        {
            _database.Insert(entity);
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        /// <param name="id">Id of a media</param>
        public void Delete(int id)
        {
            var query = new Sql($"DELETE FROM {_tableName} WHERE MediaId = {id}");
            _database.Execute(query);
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        public void DeleteAll()
        {
            var query = new Sql($"DELETE FROM {_tableName}");
            _database.Execute(query);
        }

        /// <summary>
        /// Get former state of a certain media
        /// </summary>
        /// <param name="id">Id of a media</param>
        /// <returns>TinifierMediaHistory</returns>
        public TinifierMediaHistory Get(int id)
        {
            var query = new Sql($"SELECT MediaId, FormerPath FROM {_tableName} WHERE MediaId = {id}");
            return _database.FirstOrDefault<TinifierMediaHistory>(query);
        }

        /// <summary>
        /// Get former state of all media
        /// </summary>
        /// <returns>IEnumerable of TinifierMediaHistory</returns>
        public IEnumerable<TinifierMediaHistory> GetAll()
        {
            var query = new Sql($"SELECT MediaId, FormerPath FROM {_tableName}");
            return _database.Fetch<TinifierMediaHistory>(query);
        }
    }
}
