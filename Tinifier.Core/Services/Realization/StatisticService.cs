using System.Linq;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Realization;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.Core.Services.Realization
{
    public class StatisticService : IStatisticService
    {
        private ImageService _imageService;
        private TStatisticRepository _statisticRepository;

        public StatisticService()
        {
            _statisticRepository = new TStatisticRepository();
            _imageService = new ImageService();
        }

        public void CreateStatistic(TImageStatistic statistic)
        {
            _statisticRepository.Create(statistic);
        }

        public TImageStatistic GetStatistic()
        {
            var statistic = _statisticRepository.GetStatistic();
            statistic.TotalNumberOfImages = _imageService.GetAllImages().ToList().Count;
            statistic.NumberOfOptimizedImages = _imageService.GetAllOptimizedImages().ToList().Count;

            return statistic;
        }

        public void UpdateStatistic()
        {
            var statistic = GetStatistic();

            if(statistic != null)
            {
                statistic.NumberOfOptimizedImages++;
                _statisticRepository.Update(statistic);
            }           
        }

        public void UpdateNumberOfImages()
        {
            var statistic = GetStatistic();

            if (statistic != null)
            {
                _statisticRepository.UpdateCount(statistic);
            }
        }
    }
}
