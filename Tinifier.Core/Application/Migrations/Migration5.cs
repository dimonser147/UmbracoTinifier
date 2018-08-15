using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    public class Migration5 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            var n = dbContext.Database.ExecuteScalar<int>(@"SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE [TABLE_NAME] = 'TinifierUserSettings' AND [COLUMN_NAME] = 'EnableCropsOptimization'");
            if (n == 0)
            {
                dbContext.Database.Execute(" ALTER TABLE[TinifierUserSettings] ADD EnableCropsOptimization bit not null default(0)");
            }
        }
    }
}
