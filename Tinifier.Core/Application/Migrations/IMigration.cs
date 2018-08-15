using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    interface IMigration
    {
        void Resolve(DatabaseContext dbContext);
    }
}
