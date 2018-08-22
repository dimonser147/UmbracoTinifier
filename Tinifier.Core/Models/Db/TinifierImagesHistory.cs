using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models.Db
{
    [TableName(PackageConstants.DbTinifierImageHistoryTable)]
    public class TinifierImagesHistory
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public string ImageId { get; set; }

        public string OriginFile { get; set; }
    }
}
