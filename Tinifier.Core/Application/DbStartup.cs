using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinifier.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Application
{
    public class DbStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var logger = LoggerResolver.Current.Logger;
            var dbContext = applicationContext.DatabaseContext;
            var dbHelper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);

            if (!dbHelper.TableExist(PackageConstants.DbSettingsTable))
            {
                dbHelper.CreateTable<TSetting>(false);
            }

            if (!dbHelper.TableExist(PackageConstants.DbHistoryTable))
            {
                dbHelper.CreateTable<THistoryItem>(false);
            }
        }
    }
}
