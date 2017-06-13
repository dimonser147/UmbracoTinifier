using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models
{
    [TableName(PackageConstants.DbHistoryTable)]
    public class THistoryItem
    {
        [PrimaryKeyColumn(AutoIncrement = true, Clustered = true)]
        public int Id { get; set; }

        [Index(IndexTypes.NonClustered)]
        public int ImageId { get; set; }

        public DateTime OccuredAt { get; set; }

        public string Error { get; set; }

        public bool IsOptimized { get; set; }

        public long OriginSize { get; set; }

        public long OptimizedSize { get; set; }
    }
}
