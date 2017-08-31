using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Settings
{
    public class TSettingsRepository : IEntityCreator<TSetting>, ISettingsRepository<TSetting>
    {
        private readonly UmbracoDatabase _database;

        public TSettingsRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        public TSetting GetSettings()
        {
            var query = new Sql("SELECT * FROM TinifierUserSettings ORDER BY Id DESC");
            return _database.FirstOrDefault<TSetting>(query);
        }

        /// <summary>
        /// Create settings
        /// </summary>
        /// <param name="entity">TSetting</param>
        public void Create(TSetting entity)
        {
            _database.Insert(entity);
        }

        /// <summary>
        /// Update currentMonthRequests in settings
        /// </summary>
        /// <param name="currentMonthRequests">currentMonthRequests</param>
        public void Update(int currentMonthRequests)
        {
            var query = new Sql("UPDATE TinifierUserSettings SET CurrentMonthRequests = @0", currentMonthRequests);
            _database.Execute(query);
        }
    }
}
