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
               NumberOfOptimizedImages = _imageRepository.AmounthOfOptimizedItems()
           };

           _statisticRepository.Create(newStat);           
        }

        public TinifyImageStatistic GetStatistic()
        {
            var optimizedImages = _imageRepository.AmounthOfOptimizedItems();
            var totalImages = _imageRepository.AmounthOfItems();

            var tImageStatistic = new TinifyImageStatistic
            {
                TotalOptimizedImages = optimizedImages,
                TotalOriginalImages = totalImages - optimizedImages
            };

            return tImageStatistic;
        }

        public void UpdateStatistic()
        {
            var statistic = _statisticRepository.GetStatistic();

            if (statistic == null)
            {
                CreateStatistic();
            }

            statistic.TotalNumberOfImages = _imageRepository.AmounthOfItems();
            statistic.NumberOfOptimizedImages = _imageRepository.AmounthOfOptimizedItems();
            _statisticRepository.Update(statistic);
        }
    }
}
