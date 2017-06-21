using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Realization
{
    public class TSettingsRepository : IEntityCreator<TSetting>, ISettingsRepository<TSetting>
    {
        private UmbracoDatabase _database;

        public TSettingsRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public TSetting GetSettings()
        {
            var query = new Sql("SELECT * FROM TinifierUserSettings ORDER BY Id DESC");

            var setting = _database.FirstOrDefault<TSetting>(query);

            return setting;
        }

        public void Create(TSetting entity)
        {
            _database.Insert(entity);
        }
    }
}
