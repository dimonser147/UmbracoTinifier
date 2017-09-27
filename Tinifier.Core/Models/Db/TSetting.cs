using System.ComponentModel.DataAnnotations;
using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models.Db
{
    /// <summary>
    /// User settings for database
    /// </summary>
    [TableName(PackageConstants.DbSettingsTable)]
    [PrimaryKey("Id", autoIncrement = true)]
    public class TSetting
    {
        [PrimaryKeyColumn(AutoIncrement = true, Clustered = true)]
        public int Id { get; set; }

        [Required]
        public string ApiKey { get; set; }

        [Required]
        public bool EnableOptimizationOnUpload { get; set; }

        [Required]
        public bool HideLeftPanel { get; set; }

        public int CurrentMonthRequests { get; set; }
    }
}
