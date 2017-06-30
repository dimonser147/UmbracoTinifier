using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Models.Db
{
    // Information about images for database
    [TableName(PackageConstants.DbStatisticTable)]
    public class TImageStatistic
    {
        public int TotalNumberOfImages { get; set; }

        public int NumberOfOptimizedImages { get; set; }
    }
}
