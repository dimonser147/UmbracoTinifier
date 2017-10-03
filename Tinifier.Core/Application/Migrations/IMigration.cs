using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace Tinifier.Core.Application.Migrations
{
    interface IMigration
    {
        void Resolve(DatabaseContext dbContext);
    }
}
