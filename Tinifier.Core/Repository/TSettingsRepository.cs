using System;
using System.Collections.Generic;
using Tinifier.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository
{
    public class TSettingsRepository : IRepository<TSetting>
    {
        private UmbracoDatabase _database;

        public TSettingsRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public IEnumerable<TSetting> GetAllItems()
        {
            throw new NotImplementedException();
        }

        public TSetting GetItemById(int Id)
        {
            throw new NotImplementedException();
        }

        public void CreateSettings(TSetting setting)
        {
            _database.Insert(setting);
        }

        public TSetting GetSettings()
        {
            var query = new Sql($"SELECT * FROM TinifierUserSettings ORDER BY Id DESC");

            var setting = _database.FirstOrDefault<TSetting>(query);

            return setting;
        }
    }
}
