using Tinifier.Core.Models.API;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        // Get statistic and display with chart
        TImageStatistic GetStatistic();

        // Update Statistic in database
        void UpdateStatistic();
    }
}
