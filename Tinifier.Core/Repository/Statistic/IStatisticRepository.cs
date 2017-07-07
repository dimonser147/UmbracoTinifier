namespace Tinifier.Core.Repository.Statistic
{
    // Repository for statistic with custom methods
    public interface IStatisticRepository<TImageStatistic>
    {
        TImageStatistic GetStatistic();
    }
}
