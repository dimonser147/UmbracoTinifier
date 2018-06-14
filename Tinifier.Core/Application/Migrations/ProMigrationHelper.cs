using System.Collections.Generic;
using System.Linq;

namespace Tinifier.Core.Application.Migrations
{
    static class ProMigrationHelper
    {
        public static IEnumerable<IMigration> GetAllMigrations()
        {
            return Enumerable.Empty<IMigration>();
        }
    }
}
