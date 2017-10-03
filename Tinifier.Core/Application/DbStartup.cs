using System;
using System.Collections.Generic;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Application
{
    public class DbStartup : ApplicationEventHandler
    {
        /// <summary>
        /// Add custom tables to Umbraco database
        /// </summary>
        /// <param name="umbracoApplication">UmbracoApplicationBase</param>
        /// <param name="applicationContext">ApplicationContext</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var logger = LoggerResolver.Current.Logger;
            var dbContext = applicationContext.DatabaseContext;
            var dbHelper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);

            var tables = new Dictionary<string, Type>
            {
                { PackageConstants.DbSettingsTable, typeof(TSetting) },
                { PackageConstants.DbHistoryTable, typeof(TinyPNGResponseHistory) },
                { PackageConstants.DbStatisticTable, typeof(TImageStatistic) },
                { PackageConstants.DbStateTable, typeof(TState) }
            };

            for (var i = 0; i < tables.Count; i++)
            {
                if (!dbHelper.TableExist(tables.ElementAt(i).Key))
                {
                    dbHelper.CreateTable(false, tables.ElementAt(i).Value);
                }
            }

            // migrations
            foreach(var migration in Migrations.MigrationsHelper.GetAllMigrations())
            {
                migration.Resolve(dbContext);
            }

            base.ApplicationStarted(umbracoApplication, applicationContext);
        }
    }
}
