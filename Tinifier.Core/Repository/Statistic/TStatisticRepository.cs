using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.Statistic
{
    public class TStatisticRepository 
        : IEntityCreator<TImageStatistic>, IEntityUpdater<TImageStatistic>, IStatisticRepository<TImageStatistic>
    {
        private readonly UmbracoDatabase _database;

        public TStatisticRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Create Statistic
        /// </summary>
        /// <param name="entity">TImageStatistic</param>
        public void Create(TImageStatistic entity)
        {
            _database.Insert(entity);
        }

        /// <summary>
        /// Get Statistic
        /// </summary>
        /// <returns>TImageStatistic</returns>
        public TImageStatistic GetStatistic()
        {
            var query = new Sql("SELECT * FROM TinifierImagesStatistic");
            return _database.FirstOrDefault<TImageStatistic>(query);
        }

        public long GetTotalSavedBytes()
        {
            var query = new Sql("select sum(originsize) - sum(optimizedsize) from [TinifierResponseHistory]");
            return _database.ExecuteScalar<long>(query);
        }

        /// <summary>
        /// Update Statistic
        /// </summary>
        /// <param name="entity">TImageStatistic</param>
        public void Update(TImageStatistic entity)
        {
            var query = new Sql("UPDATE TinifierImagesStatistic SET NumberOfOptimizedImages = @0, TotalNumberOfImages = @1, TotalSavedBytes = @2", entity.NumberOfOptimizedImages, entity.TotalNumberOfImages, entity.TotalSavedBytes);
            _database.Execute(query);
        }
    }
}
