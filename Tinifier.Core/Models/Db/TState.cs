using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Enums;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Tinifier.Core.Models.Db
{
    /// <summary>
    /// State table with information about current tinifing
    /// </summary>
    [TableName(PackageConstants.DbStateTable)]
    [PrimaryKey("Id", autoIncrement = true)]
    public class TState
    {
        [PrimaryKeyColumn(AutoIncrement = true, Clustered = true)]
        public int Id { get; set; }

        public int CurrentImage { get; set; }

        public int AmounthOfImages { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Ignore]
        public Statuses StatusType
        {
            get
            {
                return (Statuses)Status;
            }
            set
            {
                Status = (int)value;
            }
        }
    }
}
