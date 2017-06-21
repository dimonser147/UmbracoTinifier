using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for statistic with custom methods
    public interface IStatisticRepository<TEntity> where TEntity : class
    {
        TEntity GetStatistic();

        int Count();

        void Update(TImageStatistic entity);

        void UpdateCount(TImageStatistic entity);
    }
}
