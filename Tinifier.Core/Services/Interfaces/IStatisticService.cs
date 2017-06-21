using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        // Create statistic in database
        void CreateStatistic(TImageStatistic statistic);

        // Update statistic
        void UpdateStatistic();

        // Get statistic and display with chart
        TImageStatistic GetStatistic();

        // Increase number of images
        void UpdateNumberOfImages();
    }
}
