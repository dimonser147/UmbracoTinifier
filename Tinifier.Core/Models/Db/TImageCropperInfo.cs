using System.ComponentModel.DataAnnotations;
using Tinifier.Core.Infrastructure;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Models.Db
{
    [TableName(PackageConstants.DbTImageCropperInfoTable)]
    public class TImageCropperInfo
    {
        [Required]
        public string Key { get; set; }

        public string ImageId { get; set; }
    }
}
