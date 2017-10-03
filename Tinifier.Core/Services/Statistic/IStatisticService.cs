using Tinifier.Core.Models.API;

namespace Tinifier.Core.Services.Statistic
{
    public interface IStatisticService
    {
        /// <summary>
        /// Get statistic and display with chart
        /// </summary>
        /// <returns>TinifyImageStatistic</returns>
        TinifyImageStatistic GetStatistic();

        /// <summary>
        /// Update statistic
        /// </summary>
        /// <param name="savedBytes">SavedBytes for each request</param>
        void UpdateStatistic();
    }
}
