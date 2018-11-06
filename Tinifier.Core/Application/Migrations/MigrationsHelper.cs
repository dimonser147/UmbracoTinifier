using System.Collections.Generic;

namespace Tinifier.Core.Application.Migrations
{
    static class MigrationsHelper
    {
        public static IEnumerable<IMigration> GetAllMigrations()
        {
            yield return new Migration1();
            yield return new Migration2();
            yield return new Migration3();
            yield return new Migration4();
            yield return new Migration5();
        }
    }
}
