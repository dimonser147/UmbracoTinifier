using Tinifier.Core.Models.API;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        // Get statistic and display with chart
        TinifyImageStatistic GetStatistic();

        // Update Statistic in database
        void UpdateStatistic();

        // Create Statistic in database
        void CreateStatistic();
    }
}
