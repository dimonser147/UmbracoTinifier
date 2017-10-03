namespace Tinifier.Core.Models.API
{
    public class TinifyImageStatistic
    {
        /// <summary>
        /// Total of nonOptimized images
        /// </summary>
        public int TotalOriginalImages { get; set; }

        /// <summary>
        /// Total of optimized images 
        /// </summary>
        public int TotalOptimizedImages { get; set; }

        /// <summary>
        /// Total saved bytes for user
        /// </summary>
        public long TotalSavedBytes { get; set; }
    }
}
