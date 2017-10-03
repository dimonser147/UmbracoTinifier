namespace Tinifier.Core.Repository.Statistic
{
    /// <summary>
    /// Repository for statistic with custom methods
    /// </summary>
    /// <typeparam name="TImageStatistic">class</typeparam>
    public interface IStatisticRepository<TImageStatistic>
    {
        TImageStatistic GetStatistic();


        long GetTotalSavedBytes();
    }
}
