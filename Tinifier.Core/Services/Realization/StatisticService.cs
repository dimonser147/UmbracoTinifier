using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Realization;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.Core.Services.Realization
{
    public class StatisticService : IStatisticService
    {
        private TStatisticRepository _statisticRepository;

        public StatisticService()
        {
            _statisticRepository = new TStatisticRepository();
        }

        public void CreateStatistic(TImageStatistic statistic)
        {
            _statisticRepository.Create(statistic);
        }

        public TImageStatistic GetStatistic()
        {
            var statistic = _statisticRepository.GetStatistic();

            return statistic;
        }

        public void UpdateStatistic(TImageStatistic statistic)
        {
            _statisticRepository.Update(statistic);
        }
    }
}
