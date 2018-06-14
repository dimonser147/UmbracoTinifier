using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinifier.Core.Application.Migrations
{
    static class MigrationsHelper
    {
        public static IEnumerable<IMigration> GetAllMigrations()
        {
            yield return new Migration1();
            yield return new Migration2();

        }
    }
}
