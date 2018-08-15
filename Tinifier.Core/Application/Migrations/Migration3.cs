using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    class Migration3 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            var n = dbContext.Database.ExecuteScalar<int>(@"SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE [TABLE_NAME] = 'TinifierUserSettings' AND [COLUMN_NAME] = 'PreserveMetadata'");
            if (n == 0)
            {
                dbContext.Database.Execute(" ALTER TABLE[TinifierUserSettings] ADD PreserveMetadata bit not null default(0)");
            }
        }
    }
}