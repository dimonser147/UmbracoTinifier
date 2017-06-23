using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Repository
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
            var query = new Sql($"UPDATE TinifierImagesStatistic SET NumberOfOptimizedImages = {entity.NumberOfOptimizedImages}, TotalNumberOfImages = {entity.TotalNumberOfImages}");

            _database.Execute(query);
        }

        public int Count()
        {
            var query = new Sql("SELECT * FROM TinifierImagesStatistic");

            var numberOfElements = _database.Fetch<TImageStatistic>(query);

            return numberOfElements.Count;
        }
    }
}
