using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
