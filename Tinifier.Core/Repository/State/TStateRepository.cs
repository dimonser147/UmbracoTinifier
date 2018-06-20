using System;
using System.Collections.Generic;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.State
{
    public class TStateRepository : IRepository<TState>
    {
        private readonly UmbracoDatabase _database;

        public TStateRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        /// <summary>
        /// Create state
        /// </summary>
        /// <param name="entity">TState</param>
        public void Create(TState entity)
        {
            _database.Insert(entity);
        }

        /// <summary>
        /// Get all states
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TState> GetAll()
        {
            var query = new Sql("SELECT * FROM TinifierState");
            return _database.Fetch<TState>(query);
        }

        /// <summary>
        /// Get state with status
        /// </summary>
        /// <param name="status">status Id</param>
        /// <returns>TState</returns>
        public TState Get(int status)
        {
            var query = new Sql("SELECT * FROM TinifierState WHERE Status = @0", status);
            return _database.FirstOrDefault<TState>(query);
        }

        /// <summary>
        /// Update State
        /// </summary>
        /// <param name="entity">TState</param>
        public void Update(TState entity)
        {
            var query = new Sql("UPDATE TinifierState SET CurrentImage = @0, AmounthOfImages = @1, Status = @2 WHERE Id = @3", entity.CurrentImage, entity.AmounthOfImages, entity.Status, entity.Id);
            _database.Execute(query);
        }

        public void Delete()
        {
            var query = new Sql("UPDATE TinifierState SET Status = @0 WHERE Status = @1", (int) Statuses.Done, (int) Statuses.InProgress);
            _database.Execute(query);
            //CreateDB();
        }

        #region Private
        private void CreateDB()
        {
            var logger = LoggerResolver.Current.Logger;
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var dbHelper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);

            var tables = new Dictionary<string, Type>
            {
                { PackageConstants.DbSettingsTable, typeof(TSetting) },
                { PackageConstants.DbHistoryTable, typeof(TinyPNGResponseHistory) },
                { PackageConstants.DbStatisticTable, typeof(TImageStatistic) },
                { PackageConstants.DbStateTable, typeof(TState) },
                { PackageConstants.MediaHistoryTable, typeof(TinifierMediaHistory) }
            };

            for (var i = 0; i < tables.Count; i++)
            {
                if (!dbHelper.TableExist(tables.ElementAt(i).Key))
                {
                    dbHelper.CreateTable(false, tables.ElementAt(i).Value);
                }
            }

            // migrations
            foreach (var migration in Application.Migrations.MigrationsHelper.GetAllMigrations())
            {
                migration?.Resolve(dbContext);
            }
        }
        #endregion
    }
}
