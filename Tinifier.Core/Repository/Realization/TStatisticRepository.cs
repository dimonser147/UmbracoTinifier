using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Realization
{
    public class TStatisticRepository : IEntityCreator<TImageStatistic>, IEntityUpdater<TImageStatistic>, IStatisticRepository<TImageStatistic>
    {
        private UmbracoDatabase _database;

        public TStatisticRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public void Create(TImageStatistic entity)
        {
            _database.Insert(entity);
        }

        public TImageStatistic GetStatistic()
        {
            var query = new Sql("SELECT * FROM TinifierImagesStatistic");

            var statistic = _database.FirstOrDefault<TImageStatistic>(query);

            return statistic;
        }

        public void Update(TImageStatistic entity)
        {
            _database.Update(entity);
        }
    }
}
