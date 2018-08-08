using System.ComponentModel.DataAnnotations;
using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models.Db
{
    [TableName(PackageConstants.MediaHistoryTable)]
    public class TinifierMediaHistory
    {
        [PrimaryKeyColumn(AutoIncrement = false, OnColumns = "MediaId,OrganizationRootFolderId")]
        [Required]
        public int MediaId { get; set; }

        [Required]
        public string FormerPath { get; set; }

        [PrimaryKeyColumn(AutoIncrement = false, OnColumns = "MediaId,OrganizationRootFolderId")]
        [Required]
        public int OrganizationRootFolderId { get; set; }
    }
}
