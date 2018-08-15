using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    class Migration2 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            var n = dbContext.Database.ExecuteScalar<int>(@"SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE [TABLE_NAME] = 'TinifierUserSettings' AND [COLUMN_NAME] = 'HideLeftPanel'");
            if(n == 0)
            {
                dbContext.Database.Execute(" ALTER TABLE[TinifierUserSettings] ADD HideLeftPanel bit not null default(0)");
            }
        }
    }
}
