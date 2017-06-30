using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
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
                dbHelper.CreateTable<TinyPNGResponseHistory>(false);
            }

            if (!dbHelper.TableExist(PackageConstants.DbStatisticTable))
            {
                dbHelper.CreateTable<TImageStatistic>(false);
            }

            if(!dbHelper.TableExist(PackageConstants.DBStateTable))
            {
                dbHelper.CreateTable<TState>(false);
            }

            base.ApplicationStarted(umbracoApplication, applicationContext);
        }
    }
}
