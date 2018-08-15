using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    class Migration1 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            dbContext.Database.Execute("alter table TinifierImagesStatistic alter column TotalSavedBytes bigint");
        }
    }
}
