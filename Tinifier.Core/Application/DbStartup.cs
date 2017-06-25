using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Repository;
using Tinifier.Core.Services.Services;
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
            var imagesService = new ImageService();
            var statisticRepository = new TStatisticRepository();

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

            if(dbHelper.TableExist(PackageConstants.DbStatisticTable))
            {
                if(statisticRepository.Count() == 0)
                {
                    var statistic = new TImageStatistic
                    {
                        TotalNumberOfImages = imagesService.GetAllImages().ToList().Count,
                        NumberOfOptimizedImages = imagesService.GetAllOptimizedImages().ToList().Count
                    };
                    statisticRepository.Create(statistic);
                }                
            }

            base.ApplicationStarted(umbracoApplication, applicationContext);
        }
    }
}
