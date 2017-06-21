using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        // Create statistic in database
        void CreateStatistic(TImageStatistic statistic);

        // Update statistic
        void UpdateStatistic(TImageStatistic statistic);

        // Get statistic and display with chart
        TImageStatistic GetStatistic();
    }
}
