using System.Collections.Generic;
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

        public void Create(TState entity)
        {
            _database.Insert(entity);
        }

        public IEnumerable<TState> GetAll()
        {
            var query = new Sql("SELECT * FROM TinifierState");
            var state = _database.Fetch<TState>(query);

            return state;
        }

        public TState GetByKey(int status)
        {
            var query = new Sql($"SELECT * FROM TinifierState WHERE Status = {status}");
            var state = _database.FirstOrDefault<TState>(query);

            return state;
        }

        public void Update(TState entity)
        {
            var query = new Sql($"UPDATE TinifierState SET CurrentImage = {entity.CurrentImage}, AmounthOfImages = {entity.AmounthOfImages}, Status = {entity.Status} WHERE Id = {entity.Id}");

            _database.Execute(query);
        }
    }
}
