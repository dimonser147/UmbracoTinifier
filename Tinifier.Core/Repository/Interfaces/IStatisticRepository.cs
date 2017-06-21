namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for statistic with custom methods
    public interface IStatisticRepository<TEntity> where TEntity : class
    {
        TEntity GetStatistic();
    }
}
