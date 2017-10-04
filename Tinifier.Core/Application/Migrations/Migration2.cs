using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    class Migration2 : IMigration
    {
        public void Resolve(DatabaseContext dbContext)
        {
            int n = dbContext.Database.ExecuteScalar<int>(@"SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE [TABLE_NAME] = 'TinifierUserSettings' AND [COLUMN_NAME] = 'HideLeftPanel'");
            if(n == 0)
            {
                dbContext.Database.Execute(" ALTER TABLE[TinifierUserSettings] ADD HideLeftPanel bit not null default(0)");
            }
        }
    }
}
