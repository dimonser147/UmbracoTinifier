using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models.Db
{
    [TableName(PackageConstants.DbTFileSystemProviderSettings)]
    [PrimaryKey("Id", autoIncrement = true)]
    public class TFileSystemProviderSettings
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public string Type { get; set; }
    }
}
