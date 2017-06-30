namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for statistic with custom methods
    public interface IStatisticRepository<TImageStatistic>
    {
        TImageStatistic GetStatistic();

        int Count();

        void Update(TImageStatistic entity);
    }
}
