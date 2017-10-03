using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Models.Db
{
    /// <summary>
    /// Information about images for database
    /// </summary>
    [TableName(PackageConstants.DbStatisticTable)]
    public class TImageStatistic
    {
        public int TotalNumberOfImages { get; set; }

        public int NumberOfOptimizedImages { get; set; }

        public long TotalSavedBytes { get; set; }
    }
}
