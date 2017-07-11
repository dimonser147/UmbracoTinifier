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
        /// Update Statistic in database
        /// </summary>
        void UpdateStatistic();

        /// <summary>
        /// Create Statistic in database
        /// </summary>
        void CreateStatistic();
    }
}
