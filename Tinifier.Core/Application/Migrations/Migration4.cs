using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    public class Migration4 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            string n = dbContext.Database.ExecuteScalar<string>(@"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
                WHERE [COLUMN_NAME]='ImageId' AND [TABLE_NAME] = 'TinifierResponseHistory'");

            if(n != "nvarchar")
            {
                dbContext.Database.Execute("DROP INDEX TinifierResponseHistory.IX_TinifierResponseHistory_ImageId");
                dbContext.Database.Execute("ALTER TABLE [TinifierResponseHistory] ALTER COLUMN ImageId NVARCHAR(4000) NOT NULL");
                dbContext.Database.Execute("CREATE INDEX IX_TinifierResponseHistory_ImageId ON [TinifierResponseHistory](ImageId)");
            }          
        }
    }
}
