using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Repository.Statistic;

namespace Tinifier.Core.Services.Statistic
{
    public class StatisticService : IStatisticService
    {
        private readonly TImageRepository _imageRepository;
        private readonly TStatisticRepository _statisticRepository;

        public StatisticService()
        {
            _statisticRepository = new TStatisticRepository();
            _imageRepository = new TImageRepository();
        }

        public void CreateStatistic()
        {
           var newStat = new TImageStatistic
           {
               TotalNumberOfImages = _imageRepository.AmounthOfItems(),
               NumberOfOptimizedImages = _imageRepository.AmounthOfOptimizedItems(),
               TotalSavedBytes = 0
           };

           _statisticRepository.Create(newStat);           
        }

        public TinifyImageStatistic GetStatistic()
        {
            var statistic = _statisticRepository.GetStatistic();

            if (statistic == null)
            {
                CreateStatistic();
                statistic = _statisticRepository.GetStatistic();
            }

            var tImageStatistic = new TinifyImageStatistic
            {
                TotalOptimizedImages = statistic.NumberOfOptimizedImages,
                TotalOriginalImages = statistic.TotalNumberOfImages - statistic.NumberOfOptimizedImages,
                TotalSavedBytes = statistic.TotalSavedBytes
            };

            return tImageStatistic;
        }

        public void UpdateStatistic(int savedBytes = 0)
        {
            var statistic = _statisticRepository.GetStatistic();

            if (statistic == null)
            {
                CreateStatistic();
                statistic = _statisticRepository.GetStatistic();
            }

            statistic.TotalNumberOfImages = _imageRepository.AmounthOfItems();
            statistic.NumberOfOptimizedImages = _imageRepository.AmounthOfOptimizedItems();
            statistic.TotalSavedBytes += savedBytes;
            _statisticRepository.Update(statistic);
        }
    }
}
